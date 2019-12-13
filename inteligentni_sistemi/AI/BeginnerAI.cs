using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI
{
    /// <summary>
    /// Beginner AI player first tries to close all possible boxes. If that is not possible it chooses to add 
    /// line to the board chosen from list of non existing lines randomly
    /// </summary>
    class BeginnerAI : IPlayer
    {
        private List<LineBetweenCircles> existingMoves;
        private List<LineBetweenCircles> nonExistingMoves;
        private CurrentGame currentGame;
        private bool currentGameSet = false;
        private List<Box> boxes;

        private Random random;

        public BeginnerAI(List<LineBetweenCircles> existingMoves, 
            List<LineBetweenCircles> nonExistingMoves,
            List<Box> boxes)
        {
            this.existingMoves = existingMoves;
            this.nonExistingMoves = nonExistingMoves;
            this.boxes = boxes;

            this.random = new Random((int)DateTime.Now.Ticks);
        }

        public LineBetweenCircles MakeTurn()
        {
            if (!currentGameSet)
                throw new Exception("Current game not set.");

            LineBetweenCircles closingEdge;

            if ((closingEdge = AICommon.FindBoxClosingEdge(existingMoves, nonExistingMoves, currentGame, boxes)) != null)
                return closingEdge;
            else
            {
                int randomIndex = (int)(random.NextDouble() * nonExistingMoves.Count);
                return nonExistingMoves[randomIndex];
            }
        }

        public void SetCurrentGame(CurrentGame currentGame)
        {
            if (currentGameSet)
                throw new Exception("Current game has already been set.");

            this.currentGame = currentGame;
            currentGameSet = true;
        }
    }
}