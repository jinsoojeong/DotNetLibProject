using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLibrary.SimpleKafka
{
    public static class MsgFactory
    {
        public static Msg Create(string topic, string msg)
        {
            return new Msg
            {
                Topic = topic,
                Message = msg
            };
        }
    }

    public sealed class Msg
    {
        public string Topic { get; set; }
        public string Message { get; set; }
    }
}
