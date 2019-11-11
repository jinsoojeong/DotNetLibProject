using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace NetLibrary.SimpleKafka
{
    public class KafkaConsumer
    {
        private IConsumer<Ignore, string> consumer;
        private CancellationTokenSource cancel_token;
        private BlockingCollection<Msg> msg_queue;
        private string topic;
        private Task worker;
        private bool shutdown;

        public KafkaConsumer(string topic, ConsumerConfig config)
        {
            this.topic = topic;

            cancel_token = new CancellationTokenSource();
            consumer = new ConsumerBuilder<Ignore, string>(config).Build();
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
            KafkaConsumer kafka_consumer = (KafkaConsumer)obj;
            consumer.Subscribe(kafka_consumer.topic);

            while (!shutdown)
            {
                try
                {
                    var cr = consumer.Consume(kafka_consumer.cancel_token.Token);
                    if (cr != null)
                    {
                        Msg msg = MsgFactory.Create(cr.Topic, cr.Value);
                        msg_queue.Add(msg);
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }

                consumer.Close();
            }
        }

        internal bool DequeueMsg(out Msg msg)
        {
            msg = null;

            if (msg_queue.TryTake(out msg) == false)
                return false;

            return true;
        }
    }
}