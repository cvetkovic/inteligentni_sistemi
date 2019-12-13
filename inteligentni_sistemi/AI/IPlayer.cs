using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace etf.dotsandboxes.cl160127d.AI
{
    interface IPlayer
    {
        LineBetweenCircles MakeTurn();
        void SetCurrentGame(CurrentGame currentGame);
    }
}