using etf.dotsandboxes.cl160127d.AI.Minimax;
using etf.dotsandboxes.cl160127d.Game;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI.Minimax
{
    class ExpertMinimax : Minimax
    {
        public ExpertMinimax(List<LineBetweenCircles> existingLines,
                             List<LineBetweenCircles> nonExistingLines,
                             int maxTreeDepth) : base(existingLines, nonExistingLines, maxTreeDepth) { }

        protected override int EstimationFunction(TreeNode node)
        {
            return 0;
        }
    }
}