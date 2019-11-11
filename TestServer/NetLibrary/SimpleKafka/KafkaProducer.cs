using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Threading;

namespace NetLibrary.SimpleKafka
{
    class KafkaProducer
    {
        private IProducer<Null, string> producer;
        private Task worker;
        private bool shutdown = false;
        private BlockingCollection<Msg> msg_queue;
        private CancellationTokenSource cancel_token;

        internal KafkaProducer(ProducerConfig config)
        {
            cancel_token = new CancellationTokenSource();
            producer = new ProducerBuilder<Null, string>(config).Build();
            msg_queue = new BlockingCollection<Msg>();
            shutdown = false;

            worker = new Task(new Action<object>(Dowork), this);
        }

        internal void Start()
        {
            shutdown = false;
            worker.Start();
        }

        internal void Stop()
        {
            shutdown = true;
            cancel_token.Cancel();
            worker.Wait();
        }

        private void Dowork(object obj)
        {
            KafkaProducer kafka_producer = (KafkaProducer)obj;

            Msg msg;
            while (!shutdown)
            {
                if (msg_queue.TryTake(out msg, -1, cancel_token.Token) == false)
                {
                    if (msg.Topic == "end")
                        break;

                    PublishMsg(msg);
                }
            }
        }

        internal void EnquequeMsg(Msg msg)
        {
            msg_queue.Add(msg);
        }

        private async void PublishMsg(Msg msg)
        {
            try
            {
                await producer.ProduceAsync(msg.Topic, new Message<Null, string> { Value = msg.Message });
                Console.WriteLine("delivered");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine("delivered failed - " + e.Message);
                return;
            }
        }
    }
}
