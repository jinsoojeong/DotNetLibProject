using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLibrary.SimpleMatchMaking
{
    using CompleteMatchMakings = Dictionary<Byte, List<MatchMaking>>;

    public abstract class MatchMaking
    {
        public MatchMaking()
        {

        }

        public int rating_point { get; set; }
        public CompleteMatchMaking complete_match_making;
    }

    public abstract class CompleteMatchMaking
    {
        public CompleteMatchMaking()
        {
            match_makings = new Dictionary<Byte, List<MatchMaking>>();
        }

        internal bool GameStart(CompleteMatchMakings regist_match_makings)
        {
            match_makings = regist_match_makings;
            foreach (var itor in match_makings)
            {
                foreach (var match_making in itor.Value)
                {
                    match_making.complete_match_making = this;
                }
            }

            // 게임 시작
            GameStart();

            return true;
        }

        protected abstract bool GameStart();

        public CompleteMatchMakings match_makings { get; set; }
    }
}
