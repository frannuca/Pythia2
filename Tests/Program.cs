using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueuingSystem;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {

            Action task = () =>
            {
                System.Threading.Thread.Sleep(5000);
            };

            Action taskerror = () =>
            {
                System.Threading.Thread.Sleep(5000);
                throw new SystemException("AHA, this is an error");
            };

            var token = new System.Threading.CancellationTokenSource();
            var consumer = new ConsumerSystem(4, token);

            var msg1 = System.Threading.Tasks.Task<JOBRESULT>.Factory
                .StartNew(() => consumer.AddJob(4, new System.Action(task)), token.Token);

            var msg2 = System.Threading.Tasks.Task<JOBRESULT>.Factory
                .StartNew(() => consumer.AddJob(4, new System.Action(taskerror)), token.Token);

            var result1 = msg1.Result;
            var resutl2 = msg2.Result;

            consumer.Stop();

            Console.WriteLine("Hello World!");

        }
    }
}
