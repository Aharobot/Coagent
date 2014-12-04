using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Coagent;
namespace CoagentTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Portal port1 = new Portal("NII");
            MetaAgent agent1 = new MetaAgent("Dive1");
            MetaAgent agent2 = new MetaAgent("Dive2");
            MetaAgent agent3 = new MetaAgent("Dive3");
            port1.addAgent(agent1);
            port1.addAgent(agent2);
            port1.addAgent(agent3);
            agent1.MessageReceived += new MetaAgent.MessageReceiveHandler(al_MessageReceived);
            agent2.MessageReceived += new MetaAgent.MessageReceiveHandler(a2_MessageReceived);
            agent3.MessageReceived += new MetaAgent.MessageReceiveHandler(a3_MessageReceived);
            while(true)
            {
                Console.WriteLine("Display on agent1 \n");
                agent1.SendMessage("Dive2", Console.ReadLine());
                Console.WriteLine("Display on agent2 \n");
                agent2.SendMessage("Dive1", Console.ReadLine());
                Console.WriteLine("Display on agent3 \n");
                string mess = Console.ReadLine();
                Console.WriteLine("Display on agent1 \n");
                agent3.SendMessage("Dive1", mess);
                Console.WriteLine("Display on agent2 \n");
                agent3.SendMessage("Dive2", mess);
            }

        }

        public static void al_MessageReceived(Communication message) 
        {
            Console.WriteLine("{0} says: {1}", message.Sender, message.Content);
        }
        public static void a2_MessageReceived(Communication message) 
        {
            Console.WriteLine("{0} says: {1}", message.Sender, message.Content);
        }
        public static void a3_MessageReceived(Communication message)
        {
            Console.WriteLine("{0} says: {1}", message.Sender, message.Content);
        }

    }
}
