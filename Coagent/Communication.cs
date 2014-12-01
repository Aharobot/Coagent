using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coagent
{
    public class Communication
    {
        public Communication() 
        {
        }
        public Communication(string rawMessage) 
        {
            this.Content = Util.GetMessagePart(rawMessage, "msg");
            if (this.Content != null)
            {
                this.ID = Util.GetMessagePart(rawMessage, "id");
                this.Sender = Util.GetMessagePart(rawMessage, "from");
                this.Recipient = Util.GetMessagePart(rawMessage, "to");
                this.Type = Util.GetMessagePart(rawMessage, "type");
                this.SenderPort = Util.GetMessagePart(rawMessage, "portal");
            }
            else 
            {
                this.Content = rawMessage;
            }
        }

        public string rawMessage() 
        {
            return Util.Wraper(this.Sender, this.Recipient, this.SenderPort, this.Content, this.ID, this.Type);
        }

        public string Content { get; set; }
        public string ID { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string SenderPort { get; set; }
        public string Type { get; set; }
    }

}
