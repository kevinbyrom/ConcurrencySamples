using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace ConcurrencySamples
{
    class Program
    {
        static void Main(string[] args)
        {
            //ThreadExample();
            //MultithreadExample();
            //ThreadPoolExample();
            //TaskExample();
            ParallelLoopExample();
            //FuturesExample();
        }


        static void ThreadExample()
        {
            var thread = new Thread(new ThreadStart(SomeLongRunningMethod));
            thread.Start();
            thread.Join();
        }


        static void MultithreadExample()
        {
            var threads = new List<Thread>();

            for (int i = 0; i < 10; i++)
            {
                var thread = new Thread(new ThreadStart(SomeLongRunningMethod));
                thread.Start();
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }


        static void ThreadPoolExample()
        {
            Console.WriteLine("ThreadPoolExample");
            var handles = new List<ManualResetEvent>();

            for (int i = 0; i < 10; i++)
            {
                var handle = new ManualResetEvent(false);

                ThreadPool.QueueUserWorkItem(data =>
                {
                   SomeLongRunningMethod();
                   ((ManualResetEvent)data).Set();
                }, handle); 

                handles.Add(handle);
            }


            WaitHandle.WaitAll(handles.ToArray());
        }


        static void TaskExample()
        {
            var task = new TaskFactory().StartNew(() => 
            { 
                SomeLongRunningMethod(); 
            });

            task.Wait();
        }


        static void ParallelLoopExample()
        {
            var items = new string[] { "1", "2", "3", "4" };

            Parallel.ForEach(items, item =>
            {
                SomeLongRunningMethod(item);
            });    
        }


        static void FuturesExample()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                var task = new TaskFactory().StartNew(() =>
               {
                   SomeLongRunningMethod();
               });

                task.ContinueWith(t =>
                {
                    SomeFutureMethod();
                });

                tasks.Add(task);
            }

            Console.WriteLine("Waiting for tasks...");

            Task.WaitAll(tasks.ToArray());
        }


        static void SomeLongRunningMethod()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"Some long running process {threadId}...");
            Thread.Sleep(1000);
            Console.WriteLine($"Done with {threadId}");
        }


        static void SomeLongRunningMethod(string id)
        {
            Console.WriteLine($"Some long running process {id}...");
            Thread.Sleep(1000);
            Console.WriteLine($"Done with {id}");
        }



        static void SomeFutureMethod()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"Some future running process {threadId}...");
            Thread.Sleep(2000);
            Console.WriteLine($"Done with {threadId}");
        }
    }
}
