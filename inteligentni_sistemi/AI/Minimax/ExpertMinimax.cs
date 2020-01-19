using etf.dotsandboxes.cl160127d.AI.Minimax;
using etf.dotsandboxes.cl160127d.Game;
using System;
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
            int blue, red;

            // winning condition for me
            if (WhoHasMoreBoxes(node, out blue, out red) == whoAmI && node.NonExistingLines.Count == 0)
                return int.MaxValue;
            // winning condition for opponent
            else if (WhoHasMoreBoxes(node, out blue, out red) != whoAmI && node.NonExistingLines.Count == 0)
                return int.MinValue;

            List<Box> newBoxes = AICommon.TryClosingBoxes(node.ExistingLines,
                                                          (node.Player == MinimaxPlayerType.MAX ? Player.BLUE : Player.RED),
                                                          node.DeltaMove,
                                                          out int[] surroundingEdges);

            // box closing
            if (newBoxes.Count == 1)
                return int.MaxValue - 2;    // will close one box
            else if (newBoxes.Count == 2)
                return int.MaxValue - 1;    // will close two boxes

            // keep state where I will have more boxes
            if ((blue > red && whoAmI == Player.BLUE) || (red > blue && whoAmI == Player.RED))
                return int.MaxValue / 2 - Math.Abs(blue - red);

            // this state could lead opponent to close the box, hence negative estimation
            if (surroundingEdges[0] == 2 && surroundingEdges[1] == 2)
                return -2;
            else if (surroundingEdges[0] == 2 || surroundingEdges[1] == 2)
                return -1;

            if ((blue < red && whoAmI == Player.BLUE) || (red < blue && whoAmI == Player.RED))
                return int.MinValue / 2 + Math.Abs(blue - red);

            // state is neutral for result for the provided tree depth
            return 0;
        }
    }
}