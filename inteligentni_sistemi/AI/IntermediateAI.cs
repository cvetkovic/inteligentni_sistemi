using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace etf.dotsandboxes.cl160127d.AI
{
    class IntermediateAI : BasePlayer
    {
        public IntermediateAI(List<LineBetweenCircles> existingMoves,
            List<LineBetweenCircles> nonExistingMoves,
            List<Box> boxes) : base(existingMoves, nonExistingMoves, boxes) { }

        protected override LineBetweenCircles TurnAction()
        {
            LineBetweenCircles closingEdge;

            if ((closingEdge = AICommon.FindBoxClosingEdge(existingMoves, nonExistingMoves, currentGame, boxes)) != null)
                return closingEdge;
            else
                return AICommon.Minimax();
        }
    }
}
