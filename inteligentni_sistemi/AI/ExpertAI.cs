using etf.dotsandboxes.cl160127d.AI.Minimax;
using etf.dotsandboxes.cl160127d.Game;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI
{
    /// <summary>
    /// Beginner AI player first tries to close all possible boxes. If that is not possible it chooses the best move
    /// using the minimax algorithm with complex estimation function
    /// </summary>
    class ExpertAI : BasePlayer
    {
        private int minimaxDepth;

        public ExpertAI(List<LineBetweenCircles> existingMoves,
                        List<LineBetweenCircles> nonExistingMoves,
                        List<Box> boxes,
                        int minimaxDepth) : base(existingMoves, nonExistingMoves, boxes)
        {
            this.minimaxDepth = minimaxDepth;
        }

        protected override LineBetweenCircles TurnAction()
        {
            LineBetweenCircles closingEdge;

            if ((closingEdge = AICommon.FindBoxClosingEdge(existingMoves, nonExistingMoves, currentGame, boxes)) != null)
                return closingEdge;
            else
            {
                ExpertMinimax minimax = new ExpertMinimax(existingMoves, nonExistingMoves, minimaxDepth);
                return minimax.GetBestMove();
            }
        }
    }
}
