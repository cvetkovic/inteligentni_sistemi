using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI
{
    abstract class BasePlayer
    {
        protected List<LineBetweenCircles> existingMoves;
        protected List<LineBetweenCircles> nonExistingMoves;
        protected List<Box> boxes;

        protected Random random;

        protected CurrentGame currentGame;
        protected bool currentGameSet = false;

        protected BasePlayer(List<LineBetweenCircles> existingMoves, List<LineBetweenCircles> nonExistingMoves, List<Box> boxes)
        {
            this.existingMoves = existingMoves;
            this.nonExistingMoves = nonExistingMoves;
            this.boxes = boxes;

            random = new Random((int)DateTime.Now.Ticks);
        }

        public LineBetweenCircles MakeTurn()
        {
            if (!currentGameSet)
                throw new Exception("Current game not set.");

            return TurnAction();
        }

        public void SetCurrentGame(CurrentGame currentGame)
        {
            if (currentGameSet)
                throw new Exception("Current game has already been set.");

            this.currentGame = currentGame;
            currentGameSet = true;
        }

        protected abstract LineBetweenCircles TurnAction();
    }
}