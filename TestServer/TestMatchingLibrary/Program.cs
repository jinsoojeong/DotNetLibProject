using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleMatchMaking;

namespace TestMatchingLibrary
{
    class TestMatchMaking : MatchMaking
    {
        public TestMatchMaking() : base()
        {

        }
    }

    class TestCompleteMatchMaking : CompleteMatchMaking
    {
        public TestCompleteMatchMaking() : base()
        {

        }

        protected override bool GameStart()
        {
            foreach (var match_making in match_makings)
            {
                Console.WriteLine("game start : {0}", match_making.Key);
            }

            return true;
        }
    }

    class Program
    {
        static public CompleteMatchMaking complete_callback()
        {
            return new TestCompleteMatchMaking();
        }

        static void Main(string[] args)
        {
            CSimpleMatching.complete_callback += complete_callback;

            MatchMaking match_making1 = new TestMatchMaking();
            match_making1.rating_point = 100;

            MatchMaking match_making2 = new TestMatchMaking();
            match_making2.rating_point = 110;

            CSimpleMatching.Update();

            CSimpleMatching.RegistEntry(match_making1);

            CSimpleMatching.Update();

            CSimpleMatching.RegistEntry(match_making2);

            CSimpleMatching.Update();

            return;
        }
    }
}
