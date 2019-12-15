using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI
{
    /// <summary>
    /// Beginner AI player first tries to close all possible boxes. If that is not possible it chooses to add 
    /// line to the board chosen from list of non existing lines randomly
    /// </summary>
    class BeginnerAI : BasePlayer
    {
        public BeginnerAI(List<LineBetweenCircles> existingMoves,
            List<LineBetweenCircles> nonExistingMoves,
            List<Box> boxes) : base(existingMoves, nonExistingMoves, boxes) { }

        protected override LineBetweenCircles TurnAction()
        {
            LineBetweenCircles closingEdge;

            if ((closingEdge = AICommon.FindBoxClosingEdge(existingMoves, nonExistingMoves, Player.RED)) != null)
                return closingEdge;
            else
            {
                int randomIndex = (int)(random.NextDouble() * nonExistingMoves.Count);
                return nonExistingMoves[randomIndex];
            }
        }
    }
}