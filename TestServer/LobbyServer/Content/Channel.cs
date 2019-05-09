using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleNet;
using NetLibrary.SimpleMatchMaking;
using NetLibrary.SimpleObjectPool;
using LobbyServer.Game;
using CommonEnum;

namespace LobbyServer
{
    public class Lobby
    {
        Dictionary<int, GameRoom> game_rooms;
        List<User> connect_users;

        public Lobby()
        {
            game_rooms = new Dictionary<int, GameRoom>();
            connect_users = new List<User>();
        }

        public bool Add(User user)
        {
            connect_users.Add(user);
            user.lobby = this;
            user.state = UserState.LobbyState;

            return true;
        }

        public void Remove(User user)
        {
            lock (connect_users)
            {
                connect_users.Remove(user);
            }
        }

        public bool StartMatch(MatchMaking match_making)
        {
            if (CSimpleMatching.complete_callback == null)
                CSimpleMatching.complete_callback = complete_callback;

            CSimpleMatching.RegistEntry(match_making);
            CSimpleMatching.Update();

            return true;
        }

        public CompleteMatchMaking complete_callback()
        {
            return new TestCompleteMatchMaking();
        }

        public void Update()
        {
            foreach (var itor in game_rooms)
            {
                GameRoom game_room = itor.Value;
                game_room.Update();
            }
        }

        public bool RegistGameRoom(GameRoom game_room)
        {
            game_rooms.Add(game_room.number, game_room);

            return true;
        }

        public void BoardCast(CPacket packet, User except_user = null)
        {
            foreach (var user in connect_users)
            {
                if (user == except_user)
                    continue;

                user.send(packet);
            }
        }
    }
}
