using etf.dotsandboxes.cl160127d.AI;
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
                                   List<Box> boxes,
                                   Player whoAmI,
                                   int maxTreeDepth) : base(existingLines, nonExistingLines, boxes, whoAmI, maxTreeDepth) { }

        protected override int EstimationFunction(MinimaxTreeNode node)
        {
            // winning condition for me
            if (WhoHasMoreBoxes(node) == whoAmI && node.NonExistingLines.Count == 0)
                return int.MaxValue;

            List<Box> newBoxes = AICommon.TryClosingBoxes(node.ExistingLines, 
                                                          (node.Player == MinimaxPlayerType.MAX ? Player.BLUE : Player.RED), 
                                                          node.DeltaMove, 
                                                          out int[] surroundingEdges);

            // select state in which user can close boxes
            if (newBoxes.Count > 0)
                return int.MaxValue - 1;

            // this state could lead opponent to close the box, hence negative estimation
            if (surroundingEdges[0] == 2 && surroundingEdges[1] == 2)
                return -2;
            else if (surroundingEdges[0] == 2 || surroundingEdges[1] == 2)
                return -1;

            // state is neutral for result for the provided tree depth
            return 0;
        }
    }
}