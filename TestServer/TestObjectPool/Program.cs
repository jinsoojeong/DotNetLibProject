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
            // Msg 클래스 오브젝트 풀 생성
            CObjectPool<Msg> pool = new CObjectPool<Msg>();

            // 오브젝트 풀로 부터 Msg 클래스 할당
            Msg new_msg = pool.Alloc();
            new_msg.content = "text";
            
            Console.WriteLine("id : " + new_msg.id + "text : " + new_msg.content);

            // 완료된 msg 클래스 오브젝트 풀로 반환
            pool.Free(new_msg);

            return;
        }
    }
}
