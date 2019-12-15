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
        private ExpertMinimax minimax;
        private Player whoAmI;

        public ExpertAI(List<LineBetweenCircles> existingLines,
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
            minimax = new ExpertMinimax(existingMoves, nonExistingMoves, boxes, whoAmI, minimaxDepth);

            return minimax.GetBestMove();
        }

        public override Minimax.Minimax.MinimaxTreeNode GetMinimaxTreeNode()
        {
            minimax = new ExpertMinimax(existingMoves, nonExistingMoves, boxes, whoAmI, minimaxDepth);

            return minimax.RootNode;
        }
    }
}