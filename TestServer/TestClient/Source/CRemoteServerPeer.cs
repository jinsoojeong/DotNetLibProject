using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleNet;
using MsgProtocol;

namespace TestClient
{
    class CRemoteServerPeer : IPeer
    {
        public CUserToken token { get; private set; }

        public CRemoteServerPeer(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        void IPeer.on_message(Const<byte[]> buffer)
        {
            CPacket msg = new CPacket(buffer.Value, this);
            PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();
            switch (protocol_id)
            {
                case PROTOCOL.CHAT_MSG_ACK:
                    {
                        string text = msg.pop_string();
                        MainWindow.ChatReport(text);
                    }
                    break;
                case PROTOCOL.ENTRY_MATCHING_ACK:
                    {
                        byte team = msg.pop_byte();

                        MainWindow.SetTeam((TEAM)team);
                        MainWindow.ChangeState(GAME_STATE.INITIALIZE);
                    }
                    break;
                case PROTOCOL.PIECE_MOVE_ACK:
                    {
                        int start_x = msg.pop_int32();
                        int start_y = msg.pop_int32();
                        int target_x = msg.pop_int32();
                        int target_y = msg.pop_int32();
                        byte team = msg.pop_byte();

                        MainWindow.ChangeTrun((TEAM)team);
                        MainWindow.PieceMove(new Position(start_x, start_y), new Position(target_x, target_y));
                    }
                    break;
                case PROTOCOL.PIECE_MOVE_NOTIFY:
                    {
                        int start_x = msg.pop_int32();
                        int start_y = msg.pop_int32();
                        int target_x = msg.pop_int32();
                        int target_y = msg.pop_int32();
                        byte team = msg.pop_byte();

                        MainWindow.ChangeTrun((TEAM)team);
                        MainWindow.PieceMove(new Position(start_x, start_y), new Position(target_x, target_y), false);
                    }
                    break;
            }
        }

        void IPeer.on_removed()
        {
            MainWindow.LogReport("Server removed.");
        }

        void IPeer.send(CPacket msg)
        {
            this.token.send(msg);
        }

        void IPeer.disconnect()
        {
            this.token.socket.Disconnect(false);
            MainWindow.LogReport("Server Disconnect");
        }

        void IPeer.process_user_operation(CPacket msg)
        {
        }
    }
}
