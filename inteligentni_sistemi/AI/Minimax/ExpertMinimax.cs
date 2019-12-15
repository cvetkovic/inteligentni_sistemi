using etf.dotsandboxes.cl160127d.AI.Minimax;
using etf.dotsandboxes.cl160127d.Game;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI.Minimax
{
    class ExpertMinimax : Minimax
    {
        public ExpertMinimax(List<LineBetweenCircles> existingLines,
                             List<LineBetweenCircles> nonExistingLines,
                             List<Box> boxes,
                             Player whoAmI,
                             int maxTreeDepth) : base(existingLines, nonExistingLines, boxes, whoAmI, maxTreeDepth) { }

        protected override int EstimationFunction(MinimaxTreeNode node)
        {
            return 0;
        }
    }
}