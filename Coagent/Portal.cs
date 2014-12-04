using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace Coagent
{
    public class Portal : MetaAgent
    {
        private System.Threading.Semaphore _semaphore;
        private bool connected;
        private Hashtable routingTable;
        private bool socket;
        private SocketAgent socketAgent;
        private List<MetaAgent> unregisteredAgent;

        public Portal(string portalName):base(portalName) 
        {
            this.routingTable = new Hashtable();
            this._semaphore = new System.Threading.Semaphore(0, 0x7fffffff);
            this.connected = false;
            this.socket = false;
            this.unregisteredAgent = new List<MetaAgent>();
        }

        public void addAgent(MetaAgent agent) 
        {
            agent.AgentPortal = this;
            try 
            {
                this.routingTable.Add(agent.Name, agent);
                if(agent.GetType()==typeof(MetaAgent))
                {
                    if(this.connected)
                    {
                        this.registerAgent(agent);
                    }
                    else
                    {
                        this.unregisteredAgent.Add(agent);
                    }
                }
            }
            catch(Exception e)
            {
                Util.Log(e.Message);
            }
        }

        public void Connect(Portal portal) 
        {
            this.connected = true;
            this.routingTable.Add(base.Name, this);
            foreach(MetaAgent agent in this.unregisteredAgent)
            {
                portal.routingTable.Add(agent.Name, agent);
            }
        }

        public void Connect(int port)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            this.Handshake(ip, port);
        }

        public void Connect(IPAddress ip, int port)
        {
            this.Handshake(ip, port);
        }

        public void Connect(string ip, int port)
        {
            try
            {
                IPAddress address = IPAddress.Parse(ip);
                this.Handshake(address, port);
            } 
            catch(Exception e)
            {
                Util.Log(e.Message);
            }
        }

        private void Handshake(IPAddress ip, int port)
        {
            this.socket = true;
            this.socketAgent = new SocketAgent(base.Name + "Socket");
            this.addAgent(this.socketAgent);
            bool flag = true;
            if(this.socketAgent.TcpClientMaker(ip, port))
            {
                this.socketAgent.MessageSender(Util.HandShakeMessage(base.Name));
                this.socketAgent.StartMessageReceiver();
                flag = !this._semaphore.WaitOne(0x2710, false);
            }
            if (flag)
            {
                Util.Log("Application abortted");
                Thread.CurrentThread.Abort();
            }
            else 
            {
                this.connected = true;
                foreach(MetaAgent agent in this.unregisteredAgent)
                {
                    this.registerAgent(agent);
                }
            }
        }
        public override void MessageController(Communication message)
        {
            try
            {
                if(message.Recipient == null)
                {
                    Regex regex = new Regex("^initSock\\\x00a3[^\x00a3][^\x00a3]*\\\x00a3router$", RegexOptions.IgnoreCase);
                    if(regex.Match(message.Content).Success)
                    {
                        this._semaphore.Release();
                    }
                    else
                    {
                        Util.Log("Cannot connect to the router.");
                        this._semaphore.Release();
                    }
                }
                else if (this.routingTable[message.Recipient] != null)
                {
                    (this.routingTable[message.Recipient] as MetaAgent).Enqueue(message);
                }
                else if((this.routingTable[message.Sender] != null)& this.socket)
                {
                    this.socketAgent.Enqueue(message);
                }
                else if(!this.socket)
                {
                    this.FindSuperPortal().Enqueue(message);
                }
                else 
                {
                    Util.Log("Message is for an unknown sendoer or recipient");
                }
            } 
            catch(Exception e)
            {
                Util.Log(e.Message);
            }
        }
        private void registerAgent(MetaAgent agent) 
        {
            try
            {
                if (this.socket)
                {
                    this.socketAgent.MessageSender(Util.updateRouterMessage(agent.Name, base.Name));
                }
                else 
                {
                    this.FindSuperPortal().routingTable.Add(agent.Name, agent);
                }
            }
            catch(Exception e)
            {
                Util.Log(e.Message);
            }

        }
        private Portal FindSuperPortal()
        {
            ICollection values = this.routingTable.Values;
            foreach(object objs in values)
            {
                return (Portal)objs;
            }
            return null;
        }


    }
}
