using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleAsyncManager;
using System.Threading;

namespace TestAsyncManager
{
    class Program
    {
        class Test : IAsyncJob
        {
            public int id { get; }

            public Test(int i)
            {
                id = i + 1;
            }

            public override bool Init()
            {
                Console.WriteLine("init = " + id);
                return true;
            }

            public override bool Dowork()
            {
                Console.WriteLine("dwork = " + id);
                Random r = new Random();
                Thread.Sleep(r.Next() % 100);

                return true;
            }

            public override void Commit()
            {
                Console.WriteLine("commit = " + id);
            }

            public override void Rallback()
            {
                Console.WriteLine("rallback = " + id);
            }
        }

        static void Main(string[] args)
        {
            CSimpleAsyncManager async_manager = new CSimpleAsyncManager();

            for (int i = 0; i < 100; ++i)
            {
                Test test = new Test(i);
                async_manager.Excute(test);
                async_manager.Update();
            }

            async_manager.Stop();

            return;
        }
    }
}
