using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace NetLibrary.SimpleKafka
{
    public class SimpleKafka
    {
        static SimpleKafka instance = null;
        private KafkaProducer producer = null;
        private Dictionary<string, KafkaConsumer> consumers = null;

        private SimpleKafka()
        {
            consumers = new Dictionary<string, KafkaConsumer>();
        }

        public static SimpleKafka GetInstance()
        {
            if (instance == null)
                instance = new SimpleKafka();

            return instance;
        }

        public void CreateProducer(string bootstrap_servers)
        {
            var config = new ProducerConfig
            {
                //BootstrapServers = "localhost:9092",
                BootstrapServers = bootstrap_servers,
                Acks = Acks.Leader
            };

            producer = new KafkaProducer(config);
            producer.Start();
        }

        public void CreateConsumer(string topic, string consumer_group, string bootstrap_servers)
        {
            var config = new ConsumerConfig
            {
                GroupId = consumer_group,
                //BootstrapServers = "localhost:9092",
                BootstrapServers = bootstrap_servers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            KafkaConsumer consumer = new KafkaConsumer(topic, config);
            consumers.Add(topic, consumer);
        }

        public void EnqueuePubMsg(Msg msg)
        {
            producer.EnquequeMsg(msg);
        }

        public bool DequequeSubMsg(string topic, out Msg msg)
        {
            msg = null;

            if (consumers.ContainsKey(topic) == false)
                return false;

            if (consumers[topic].DequeueMsg(out msg) == false)
                return false;

            return true;
        }
    }
}
