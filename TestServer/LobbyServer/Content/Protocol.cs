using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgProtocol
{
    public enum PROTOCOL : short
    {
        BEGIN = 0,

        CHAT_MSG_REQ = 1,
        CHAT_MSG_ACK = 2,
        ENTRY_MATCHING_REQ = 3,
        ENTRY_MATCHING_ACK = 4,
        PIECE_MOVE_REQ = 5,
        PIECE_MOVE_ACK = 6,
        PIECE_MOVE_NOTIFY = 7,
        TURN_OVER_REQ,
        TURN_OVER_ACK,
        TURN_OVER,

        END
    }
}
