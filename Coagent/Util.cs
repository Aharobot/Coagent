using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Coagent
{
    public static class Util
    {
        public const string EOT = "\x0004";
        public const string POUND = "\x00a3";
        public const string routerHandShakeRegex = "^initSock\\\x00a3[^\x00a3][^\x00a3]*\\\x00a3router$";
        public const int secondsWaitingForRouter = 10;

        public static void Log(string logMessage)
        {
            using (StreamWriter writer = File.AppendText(Directory.GetCurrentDirectory() + @"\log"))
            {
                writer.Write("Log Start:  ");
                writer.WriteLine("{0},{1}\r\n", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                writer.WriteLine("{0}", logMessage);
                writer.WriteLine("-++++++++++++++++++++++++++++++++++++++++++++++++++++++++- \r\n");
            }
        }

        public static string GetMessagePart(string msg, string partName)
        {
            try
            {
                string[] strArray = msg.Split(new char[] {char.Parse("\x00004")});
                for (int i = 0; i <= strArray.GetUpperBound(0); i++) 
                {
                    if (strArray[i].StartsWith(partName))
                    {
                        return strArray[i].Substring(strArray[i].IndexOf('=')+1, (strArray[i].Length- strArray[i].IndexOf('-')) -1);
                    }
                }
            }
            catch(Exception e)
            {
                Log(e.Message);
            }
            return null;
        }
        
        public static string HandShakeMessage(string portalName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("initiSock");
            builder.Append("\x00a3");
            builder.Append(portalName);
            builder.Append("x00a3");
            builder.Append("portal");
            builder.Append("\x00a3");
            builder.Append("\r\n");
            return builder.ToString();
        }

        public static string updateRouterMessage(string agentName, string portalName) 
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("mtype=newAgent");
            builder.Append("\x0004");
            builder.Append("portal=");
            builder.Append(portalName);
            builder.Append("\x0004");
            builder.Append("agent=");
            builder.Append(agentName);
            builder.Append("\x0004");
            builder.Append("source=");
            builder.Append(portalName);
            builder.Append("\x0004");
            builder.Append("id=");
            builder.Append(portalName);
            builder.Append("/");
            builder.Append(agentName);
            builder.Append("\x0004");
            builder.Append("is-local=false");
            builder.Append("\x0004");
            builder.Append("\r\n");
            return builder.ToString();
        }

        public static string Wraper(string sender, string recipient, string portalName, string messageContent, string id, string messageType) 
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("id=");
            builder.Append(id);
            builder.Append("\x0004");
            builder.Append("mtype=");
            builder.Append(messageType);
            builder.Append("\x0004");
            builder.Append("source=");
            builder.Append(portalName);
            builder.Append("\x0004");
            builder.Append("from=");
            builder.Append(sender);
            builder.Append("\x0004");
            builder.Append("to=");
            builder.Append(recipient);
            builder.Append("\x0004");
            builder.Append("msg=");
            builder.Append(messageContent);
            builder.Append("\x0004");
            builder.Append("\r\n");
            return builder.ToString();

        }
      
    }
}
