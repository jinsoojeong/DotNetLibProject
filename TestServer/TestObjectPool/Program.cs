using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleObjectPool;

namespace TestObjectPool
{
    class Msg : IObject
    {
        static int serial_number = 0;

        public int id { get; private set; }
        public string content { get; set; }

        public Msg()
        {
            id = ++serial_number;
        }

        void IObject.Reset()
        {
            content = "";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CObjectPool<Msg> pool = new CObjectPool<Msg>();

            Msg new_msg = pool.Alloc();
            new_msg.content = "text";
            
            Console.WriteLine("id : " + new_msg.id + "text : " + new_msg.content);
            pool.Free(new_msg);

            return;
        }
    }
}
