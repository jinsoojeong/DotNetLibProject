using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleMatchMaking;
using NetLibrary.SimpleNet;
using MsgProtocol;

namespace SimpleServer
{
    public class TestMatchMaking : MatchMaking
    {
        List<User> users;
        User host_user;
        public byte team { get; set; }

        public TestMatchMaking(User user, int rating)
        {
            base.rating_point = rating;

            users = new List<User>();
            users.Add(user);

            this.host_user = user;
        }

        public void BroadCast(CPacket packet)
        {
            foreach (var user in users)
            {
                user.send(packet);
            }
        }
    }

    public class TestCompleteMatchMaking : CompleteMatchMaking
    {
        protected override bool GameStart()
        {
            foreach (var itor in match_makings)
            {
                foreach (var match_making in itor.Value)
                {
                    TestMatchMaking test_match_making = match_making as TestMatchMaking;
                    test_match_making.team = (byte)itor.Key;

                    CPacket packet = CPacket.create((short)PROTOCOL.ENTRY_MATCHING_ACK);
                    packet.push((byte)itor.Key);

                    test_match_making.BroadCast(packet);
                }
            }

            return true;
        }
    }
}
