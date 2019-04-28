using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleMatchMaking;
using NetLibrary.SimpleNet;
using LobbyServer;
using LobbyServer.Game;
using CommonEnum;

namespace LobbyServer
{
    public class TestMatchMaking : MatchMaking
    {
        
        public byte team { get; set; }
        public User user;

        public TestMatchMaking(User user, int rating)
        {
            base.rating_point = rating;
            this.user = user;
        }

        public void BroadCast(CPacket packet)
        {
            user.send(packet);
        }
    }
    public class TestCompleteMatchMaking : CompleteMatchMaking
    {
        GameRoom game_room = new GameRoom();

        protected override bool GameStart()
        {
            foreach (var itor in match_makings)
            {
                foreach (var match_making in itor.Value)
                {
                    TestMatchMaking test_match_making = match_making as TestMatchMaking;
                    test_match_making.team = (byte)itor.Key;

                    game_room.Regist(test_match_making.team, test_match_making.user);                    
                }
            }
            
            game_room.BroadCast((short)PROTOCOL.ENTRY_MATCHING_ACK);
            Program.game_server.GetLobby().RegistGameRoom(game_room);

            return true;
        }
    }
}
