using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace etf.dotsandboxes.cl160127d.AI.Minimax
{
    class IntermediateMinimax : Minimax
    {
        public IntermediateMinimax(List<LineBetweenCircles> existingLines,
                                   List<LineBetweenCircles> nonExistingLines,
                                   int maxTreeDepth) : base(existingLines, nonExistingLines, maxTreeDepth) { }

        protected override int EstimationFunction(TreeNode node)
        {
            return 0;
        }
    }
}