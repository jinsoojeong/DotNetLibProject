using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonEnum
{
    public enum PROTOCOL : short
    {
        BEGIN = 0,

        CERTIFY_REQ = 1,
        CERTIFY_ACK,
        CHAT_MSG_REQ,
        CHAT_MSG_ACK,
        ENTRY_MATCHING_REQ,
        ENTRY_MATCHING_ACK,
        PIECE_MOVE_REQ,
        PIECE_MOVE_ACK,
        PIECE_MOVE_NOTIFY,
        TURN_OVER_REQ,
        TURN_OVER_ACK,
        TURN_OVER,

        END
    }
}
