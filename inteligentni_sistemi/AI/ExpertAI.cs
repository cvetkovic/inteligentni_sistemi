using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using etf.dotsandboxes.cl160127d.Game;

namespace etf.dotsandboxes.cl160127d.AI
{
    class ExpertAI : BasePlayer
    {
        public ExpertAI(List<LineBetweenCircles> existingMoves,
            List<LineBetweenCircles> nonExistingMoves,
            List<Box> boxes) : base(existingMoves, nonExistingMoves, boxes) { }

        protected override LineBetweenCircles TurnAction()
        {
            throw new NotImplementedException();
        }
    }
}
