using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetLibrary.SimpleKafka;

namespace TestKafka
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleKafka.GetInstance().CreateProducer("localhost:9092");
            SimpleKafka.GetInstance().CreateConsumer("topic_name", "consumer_group", "localhost:9092");

            Msg msg = MsgFactory.Create("pub_topic", "msg content");
            SimpleKafka.GetInstance().EnqueuePubMsg(msg);

            Msg sub_msg;
            SimpleKafka.GetInstance().DequequeSubMsg("sub_topic_name", out sub_msg);

            return;
        }
    }
}
