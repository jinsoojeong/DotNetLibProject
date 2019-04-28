using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleNet;
using NetLibrary.SimpleMatchMaking;

namespace SimpleServer
{
    public class Channel
    {
        static List<User> channel_users = new List<User>();
        static object sync = new object();

        // mt 함수
        public static bool Add(User user)
        {
            lock (channel_users)
            {
                channel_users.Add(user);
            }

            return true;
        }

        public static void Remove(User user)
        {
            lock (channel_users)
            {
                channel_users.Remove(user);
            }
        }

        public static bool StartMatch(MatchMaking match_making)
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

        public static CompleteMatchMaking complete_callback()
        {
            return new TestCompleteMatchMaking();
        }

        public static void BoardCast(CPacket packet, User except_user = null)
        {
            lock (channel_users)
            {
                foreach ( var user in channel_users )
                {
                    if (user == except_user)
                        continue;

                    user.send(packet);
                }
            }
        }
    }
}
