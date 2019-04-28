using System;
using System.Collections.Generic;
using System.Text;

using NetLibrary.SimpleNet;
using LobbyServer;
using MsgProtocol;

namespace LobbyServer.Game
{
    public class GameRoom
    {
        static object sync = new object();
        static int room_number = 1;

        public int number; 
        User[] game_users = new User[2];

        GameLogic game_logic;

        public GameRoom()
        {
            lock (sync)
            {
                room_number++;
                number = room_number;
            }

            game_logic = new GameLogic();
            game_logic.Initialize();
        }
        public void Update()
        {
            game_logic.Update();
        }

        public void Regist(Byte team, User user)
        {
            game_users[team] = user;
        }

        public void BroadCast(short protocol)
        {
            for (Byte team = 0; team < 2 ; ++ team)
            {
                CPacket packet = CPacket.create((short)PROTOCOL.ENTRY_MATCHING_ACK);
                packet.push((byte)team);

                game_users[team].send(packet);
            }
        }
    }
}
