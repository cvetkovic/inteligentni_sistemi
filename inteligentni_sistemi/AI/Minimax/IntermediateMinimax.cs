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
                                   int maxTreeDepth) : base(existingLines, nonExistingLines, maxTreeDepth) { }

        protected override int EstimationFunction(TreeNode node)
        {
            // select terminating game state
            if (node.NonExistingLines.Count == 0)
                return int.MaxValue;

            int canCloseBoxes = AICommon.TryClosingBoxes(node.ExistingLines, null, null, node.DeltaMove, out int[] surroundingEdges);

            // select state in which user can close boxes
            if (canCloseBoxes > 0)
                return canCloseBoxes;

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