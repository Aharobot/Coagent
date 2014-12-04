using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Coagent
{
    public class Spooler: BlockingQueue<Communication>
    {
        private Thread startSpoolerThread;
        protected Spooler(string usedName) 
        {
            this.Name = usedName;
        }
        private void getMessage() 
        {
            while(true)
            {
                this.ProcessData(base.Dequeue());
            }
        }

        protected void StartSpooler() 
        {
            ThreadStart start = () => this.getMessage();
            this.startSpoolerThread = new Thread(start);
            this.startSpoolerThread.Name = "StartSpooler";
            this.startSpoolerThread.IsBackground = true;
            this.startSpoolerThread.Start();
        }
        protected virtual void ProcessData(Communication message) { }
        public string Name { get; set; }
    }
}
