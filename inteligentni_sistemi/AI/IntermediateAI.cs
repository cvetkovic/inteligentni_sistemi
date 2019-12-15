using etf.dotsandboxes.cl160127d.AI.Minimax;
using etf.dotsandboxes.cl160127d.Game;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI
{
    /// <summary>
    /// Beginner AI player first tries to close all possible boxes. If that is not possible it chooses the best move
    /// using the minimax algorithm with simple estimation function
    /// </summary>
    class IntermediateAI : BasePlayer
    {
        private int minimaxDepth;
        private IntermediateMinimax minimax;
        private Player whoAmI;

        public IntermediateAI(List<LineBetweenCircles> existingLines,
                                   List<LineBetweenCircles> nonExistingLines,
                                   List<Box> boxes,
                                   Player whoAmI,
                                   int minimaxDepth) : base(existingLines, nonExistingLines, boxes)
        {
            this.minimaxDepth = minimaxDepth;
            this.whoAmI = whoAmI;
        }

        protected override LineBetweenCircles TurnAction()
        {
            LineBetweenCircles closingEdge;

            if ((closingEdge = AICommon.FindBoxClosingEdge(existingMoves, nonExistingMoves, Player.RED)) != null)
                return closingEdge;
            else
            {
                minimax = new IntermediateMinimax(existingMoves, nonExistingMoves, boxes, whoAmI, minimaxDepth);

                return minimax.GetBestMove();
            }
        }

        public override Minimax.Minimax.MinimaxTreeNode GetMinimaxTreeNode()
        {
            minimax = new IntermediateMinimax(existingMoves, nonExistingMoves, boxes, whoAmI, minimaxDepth);

            return minimax.RootNode;
        }
    }
}