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
            // 프로듀서 생성
            // kafka의 broker server 주소 값 할당
            // producer를 통해 해당 카프카 서버로 메시지를 토픽에 pub
            SimpleKafka.GetInstance().CreateProducer("localhost:9092");

            // 컨슈머 생성
            // 생성 시 kafka 주소, subscribe 할 토픽의 명칭과 구독자 그룹을 명시
            SimpleKafka.GetInstance().CreateConsumer("topic_name", "consumer_group", "localhost:9092");

            // kafka게 전송할 객체를 Msg 형태로 일반화 하여 전송할 수 있음
            // 어떤 토픽에 던질지 사용가능
            Msg msg = MsgFactory.Create("pub_topic", "msg content");
            SimpleKafka.GetInstance().EnqueuePubMsg(msg);

            Msg sub_msg;
            SimpleKafka.GetInstance().DequequeSubMsg("sub_topic_name", out sub_msg);

            SimpleKafka.GetInstance().StopProducer();
            SimpleKafka.GetInstance().StopConsumer();

            return;
        }
    }
}
