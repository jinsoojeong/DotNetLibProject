using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleNet;
using NetLibrary.SimpleMatchMaking;
using MsgProtocol;

namespace SimpleServer
{
    public class User : IPeer
    {
        CUserToken token;
        MatchMaking match_making;

        public delegate int Func(int x);

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

                        CPacket packet = CPacket.create((short)PROTOCOL.CHAT_MSG_ACK);
                        packet.push(text);

                        Channel.BoardCast(packet);
                    }
                    break;
                case PROTOCOL.ENTRY_MATCHING_REQ:
                    {
                        match_making = new TestMatchMaking(this, 100);
                        Channel.StartMatch(match_making);
                    }
                    break;
                case PROTOCOL.PIECE_MOVE_REQ:
                    {
                        int select_x = msg.pop_int32();
                        int select_y = msg.pop_int32();
                        int target_x = msg.pop_int32();
                        int target_y = msg.pop_int32();

                        byte team = ((TestMatchMaking)match_making).team;
                        byte next_turn_team = (team == 0) ? (byte)1 : (byte)0;

                        CPacket packet = CPacket.create((short)PROTOCOL.PIECE_MOVE_ACK);
                        packet.push(select_x);
                        packet.push(select_y);
                        packet.push(target_x);
                        packet.push(target_y);
                        packet.push(next_turn_team);

                        Func func = (int x) =>
                        {
                            if (x == 1) return 4;
                            else if (x == 2) return 3;
                            else if (x == 3) return 2;
                            else return 1;
                        };

                        CPacket versus_packet = CPacket.create((short)PROTOCOL.PIECE_MOVE_NOTIFY);
                        versus_packet.push(func(select_x));
                        versus_packet.push(func(select_y));
                        versus_packet.push(func(target_x));
                        versus_packet.push(func(target_y));
                        versus_packet.push(next_turn_team);

                        CompleteMatchMaking complete_match_making = match_making.complete_match_making;
                        foreach (var itor in complete_match_making.match_makings)
                        {
                            MatchMaking team_match_making = itor.Value[0];
                            ((TestMatchMaking)team_match_making).BroadCast((team == itor.Key) ? packet : versus_packet);
                        }
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
