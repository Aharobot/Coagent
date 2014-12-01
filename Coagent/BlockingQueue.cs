using System;
using System.Collections.Generic;
using System.Threading;

namespace Coagent
{
    public class BlockingQueue<T>
    {
        private Queue<T> _queue;
        private Semaphore _semaphore;

        public BlockingQueue() 
        {
            this._queue = new Queue<T>();
            this._semaphore = new Semaphore(0, 0x7fffffff);
        }
        public T Dequeue() 
        {
            this._semaphore.WaitOne();
            lock (this._queue) 
            {
                return this._queue.Dequeue();
            }
        }
        public void Enqueue(T data) 
        {
            try
            {
                if (data == null)
                {
                    throw new ArgumentNullException();
                }
                lock (this._queue)
                {
                    this._queue.Enqueue(data);
                }
                this._semaphore.Release();
            }
            catch (Exception e) 
            {
                Util.Log(e.Message);
            }
        }

    }
}
