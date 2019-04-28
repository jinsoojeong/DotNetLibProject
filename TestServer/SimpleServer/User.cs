using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeNet;

namespace WpfTest
{
    public class User : IPeer
    {
        CUserToken token;

        public User(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        public void send(CPacket msg)
        {
            this.token.send(msg);
        }

        void IPeer.on_message(Const<byte[]> buffer)
        {
            CPacket msg = new CPacket(buffer.Value, this);
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            MainWindow.LogReport("protocol_id : {0}", protocol);

            switch (protocol)
            {
                case PROTOCOL.CHAT_MSG_REQ:
                    {
                        string text = msg.pop_string();
                        MainWindow.LogReport("protocol msg : {0}", text);

                        CPacket respon = CPacket.create((short)PROTOCOL.CHAT_MSG_ACK);
                        respon.push(text);
                        send(respon);
                    }
                    break;
            }
        }

        void IPeer.on_removed()
        {
            MainWindow.LogReport("client disconnect!");
            MainWindow.remove_user(this);
        }

        void IPeer.disconnect()
        {
            this.token.socket.Disconnect(false);
        }

        void IPeer.process_user_operation(CPacket msg)
        {

        }
    }
}
