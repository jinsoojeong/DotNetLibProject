using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleNet;
using NetLibrary.SimpleMatchMaking;
using NetLibrary.SimpleObjectPool;
using LobbyServer.Game;

namespace LobbyServer
{
    public class Lobby
    {
        Dictionary<int, GameRoom> game_rooms;
        List<User> connect_users;
        object sync;

        //static CObjectPool<User> user_object_pool = new CObjectPool<User>();

        public Lobby()
        {
            game_rooms = new Dictionary<int, GameRoom>();
            connect_users = new List<User>();
            sync = new object();
        }

        //public bool CreateUser(CUserToken token)
        //{
        //    User new_user = null;
        //    lock (user_object_pool)
        //    {
        //        new_user = user_object_pool.Alloc();
        //        new_user.Initialize(token);
        //    }

        //    if (Add(new_user) == false)
        //        return false;

        //    return true;
        //}

        public bool Add(User user)
        {
            lock (connect_users)
            {
                connect_users.Add(user);
                user.lobby = this;
            }

            return true;
        }

        public void Remove(User user)
        {
            lock (connect_users)
            {
                connect_users.Remove(user);
            }

            //user_object_pool.Free(user);
        }

        public bool StartMatch(MatchMaking match_making)
        {
            lock (sync)
            {
                if (CSimpleMatching.complete_callback == null)
                    CSimpleMatching.complete_callback = complete_callback;

                CSimpleMatching.RegistEntry(match_making);
                CSimpleMatching.Update();
            }

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
            lock (connect_users)
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
}
