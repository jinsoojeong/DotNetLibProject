using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLibrary.SimpleMatchMaking
{
    using EntryQueue = List<MatchMaking>;
    using CompleteMatchMakings = Dictionary<Byte, List<MatchMaking>>;

    public class CSimpleMatching
    {
        public delegate CompleteMatchMaking CompleteCallback();

        static EntryQueue entry_queue = new EntryQueue();
        static EntryQueue remove_match_entrys = new EntryQueue();
        static public CompleteCallback complete_callback { get; set; }

        static public bool RegistEntry(MatchMaking match_making)
        {
            if (complete_callback == null)
                return false;

            entry_queue.Add(match_making);

            return true;
        }

        static public bool RemoveEntry(MatchMaking match_making)
        {
            if (complete_callback == null)
                return false;

            entry_queue.Remove(match_making);

            return true;
        }

        static public bool CompleteMatchMaking(CompleteMatchMakings complete_match_makings)
        {
            CompleteMatchMaking complete_match_making = complete_callback();

            if (complete_match_making == null)
                return false;

            if (complete_match_making.GameStart(complete_match_makings) == false)
                return false;

            return true;
        }

        static public void Update()
        {
            if (complete_callback == null)
                return;

            foreach (var standard_match_making in entry_queue)
            {
                if (remove_match_entrys.Contains(standard_match_making))
                    continue;

                var range = entry_queue.Where(x => (standard_match_making != x) && (Math.Abs(x.rating_point - standard_match_making.rating_point)) < 100).Select(x => x);

                if (range.Count() == 0)
                    continue;

                int count = range.Count();
                MatchMaking target_match_making = range.First();

                CompleteMatchMakings complete_match_makings = new CompleteMatchMakings();

                EntryQueue team_1 = new EntryQueue();
                team_1.Add(standard_match_making);

                EntryQueue team_2 = new EntryQueue();
                team_2.Add(target_match_making);

                complete_match_makings.Add(0, team_1);
                complete_match_makings.Add(1, team_2);

                if (CompleteMatchMaking(complete_match_makings) == false)
                    continue;

                remove_match_entrys.Add(standard_match_making);
                remove_match_entrys.Add(target_match_making);
            }

            if (remove_match_entrys.Count() == 0)
                return;

            foreach (var entry in remove_match_entrys)
            {
                RemoveEntry(entry);
            }
        }
    }
}
