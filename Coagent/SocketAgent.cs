using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Coagent
{
    public class SocketAgent: MetaAgent
    {
        private StreamReader messageReader;
        private StreamWriter messageWriter;
        private TcpClient outputClient;
        
        public SocketAgent(string socketName):base(socketName) 
        {
 
        }

        public override void MessageController(Communication message)
        {
            this.MessageSender(message.rawMessage());
        }
        public void MessageReceiver()
        {
            while (true) 
            {
                try
                {
                    this.messageReader = new StreamReader(this.outputClient.GetStream(), Encoding.Default);
                    this.SendToPortal(this.messageReader.ReadLine().Trim(new char[1]));
                }
                catch(Exception e)
                {
                    Util.Log(e.Message);
                }
            }
        }
        public void MessageSender(string messageContent) 
        {
            try
            {
                this.messageWriter = new StreamWriter(this.outputClient.GetStream(), Encoding.Default);
                this.messageWriter.Write(messageContent);
                this.messageWriter.Flush();
            } 
            catch(Exception e)
            {
                Util.Log(e.Message);
            }

        }
        public void SendToPortal(string response) 
        {
            Communication data = new Communication(response);
            base.AgentPortal.Enqueue(data);
        }

        public void StartMessageReceiver()
        {
            ThreadStart start = () => this.MessageReceiver();
            new Thread(start) { Name = "StartMessageReceiver", IsBackground = true }.Start();
        }
        public bool TcpClientMaker(IPAddress hostIP, int port)
        {
            bool flag = false;
            try
            {
                this.outputClient = new TcpClient(hostIP.ToString(), port);
                flag = true;
            }
            catch (SocketException)
            {
                Util.Log("Router is not available");
            }
            catch (Exception e) 
            {
                Util.Log(e.Message);
            }
            return flag; 
        }

    }
}
