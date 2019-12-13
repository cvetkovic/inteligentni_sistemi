using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace etf.dotsandboxes.cl160127d.AI
{
    /// <summary>
    /// 
    /// </summary>
    class BeginnerAI : IPlayer
    {
        private List<LineBetweenCircles> existingMoves, nonExistingMoves;
        private Random random;

        public BeginnerAI(List<LineBetweenCircles> existingMoves, List<LineBetweenCircles> nonExistingMoves)
        {
            this.existingMoves = existingMoves;
            this.nonExistingMoves = nonExistingMoves;

            this.random = new Random(Guid.NewGuid().GetHashCode());
        }

        public LineBetweenCircles MakeTurn()
        {
            LineBetweenCircles closingEdge;

            if ((closingEdge = AICommon.FindBoxClosingEdge(existingMoves)) != null)
                return closingEdge;
            else
            {
                int randomIndex = (int)(random.NextDouble() * nonExistingMoves.Count);
                return nonExistingMoves[randomIndex];
            }
        }
    }
}
