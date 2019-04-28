using System;
using System.Collections.Generic;
using System.Text;

namespace LobbyServer.Game
{
    public enum TEAM : Byte
    {
        VC,
        PM
    }

    public enum PLAY : Byte
    {
        READY,
        PLAYING,
        FINISH
    }

    public class GameLogic
    {
        TEAM turn_team;
        long turn_limit_time;
        int[] team_hp = new int[2];

        public bool Initialize()
        {
            turn_team = TEAM.VC;
            turn_limit_time = 0;

            team_hp[0] = 30;
            team_hp[1] = 30;

            return true;
        }

        public bool IsTurn(TEAM team)
        {
            if (turn_team != team)
                return false;

            return true;
        }

        public void TurnChange()
        {
            turn_team = (turn_team == TEAM.PM) ? TEAM.VC : TEAM.PM;
            turn_limit_time = DateTime.Now.Ticks + 6000 * 100000;

            Program.LogReport("Trun Update");
        }

        public void Update()
        {
            if (DateTime.Now.Ticks >= turn_limit_time)
            {
                TurnChange();
            }
        }
    }
}
