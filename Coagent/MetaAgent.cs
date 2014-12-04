using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Coagent
{
    public class MetaAgent: Spooler
    {
        public event MessageReceiveHandler MessageReceived;
        public MetaAgent(string agentName) : base(agentName) 
        {
            base.StartSpooler();
        }
        public virtual void MessageController(Communication message)
        {
            if (this.MessageReceived != null) 
            {
                this.MessageReceived(message);
            }
        }
        protected override void ProcessData(Communication message)
        {
            this.MessageController(message);
        }
        public virtual void SendMessage(string recipient, string message) 
        {
            Communication data = new Communication
            {
                Sender= base.Name,
                Recipient = recipient,
                SenderPort = this.AgentPortal.Name,
                Content = message,
                ID = "msg ID",
                Type = "stdMsg"
            };
            this.AgentPortal.Enqueue(data);
        }


        public delegate void MessageReceiveHandler(Communication message);
        public MetaAgent AgentPortal { get; set; }
    }
}
