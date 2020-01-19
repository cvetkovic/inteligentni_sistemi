using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace etf.dotsandboxes.cl160127d.AI.Minimax
{
    abstract class Minimax
    {
        #region Class Variables

        private readonly List<LineBetweenCircles> initialExistingLines;
        private readonly List<LineBetweenCircles> initialNonExistingLines;
        private readonly List<Box> initialBoxes;

        protected Player whoAmI;

        private MinimaxTreeNode rootNode;
        private int maxTreeDepth;

        #endregion

        #region Internal Classes

        public enum MinimaxPlayerType
        {
            MAX = 0,
            MIN = 1
        }

        public class MinimaxTreeNode
        {
            private List<LineBetweenCircles> existingLines = new List<LineBetweenCircles>();
            private List<LineBetweenCircles> nonExistingLines = new List<LineBetweenCircles>();
            private List<Box> boxes = new List<Box>();
            private List<MinimaxTreeNode> children = new List<MinimaxTreeNode>();

            public MinimaxPlayerType Player { get; set; }
            public List<MinimaxTreeNode> Children
            {
                get
                {
                    return children;
                }
            }
            public int EstimationScore { get; set; }

            public LineBetweenCircles DeltaMove { get; set; }

            public List<LineBetweenCircles> ExistingLines
            {
                get
                {
                    return existingLines;
                }
            }

            public List<LineBetweenCircles> NonExistingLines
            {
                get
                {
                    return nonExistingLines;
                }
            }

            public List<Box> Boxes
            {
                get
                {
                    return boxes;
                }
            }

            public override string ToString()
            {
                return DeltaMove.ToString() + " - " + PlayerName() + " - (" + EstimationScore + ")";
            }

            private string PlayerName()
            {
                return (Player == MinimaxPlayerType.MAX ? "MAX" : "MIN");
            }
        }

        #endregion

        #region Base Class Constructor

        protected Minimax(List<LineBetweenCircles> existingLines,
                          List<LineBetweenCircles> nonExistingLines,
                          List<Box> boxes,
                          Player whoAmI,
                          int maxTreeDepth)
        {
            this.initialExistingLines = existingLines;
            this.initialNonExistingLines = nonExistingLines;
            this.initialBoxes = boxes;
            this.whoAmI = whoAmI;
            this.maxTreeDepth = maxTreeDepth;

            rootNode = ConstructRootNode();
        }

        #endregion

        #region Base Class Methods

        private MinimaxTreeNode ConstructRootNode()
        {
            MinimaxTreeNode node = new MinimaxTreeNode();

            node.EstimationScore = 0;
            node.Player = MinimaxPlayerType.MIN;

            node.ExistingLines.AddRange(initialExistingLines);
            node.NonExistingLines.AddRange(initialNonExistingLines);
            node.Boxes.AddRange(initialBoxes);

            ConstructTree(node, 0, MinimaxPlayerType.MIN, int.MinValue, int.MaxValue);

            return node;
        }

        private int ConstructTree(MinimaxTreeNode parentNode, int depth, MinimaxPlayerType minimaxPlayer, int alpha, int beta)
        {
            // return estimation function if max tree depth reached or game is in finished state
            if (depth == maxTreeDepth || parentNode.NonExistingLines.Count == 0)
                return EstimationFunction(parentNode);

            if (minimaxPlayer == MinimaxPlayerType.MAX)
            {
                int bestValue = int.MinValue;

                for (int i = 0; i < parentNode.NonExistingLines.Count; i++)
                {
                    #region New Node Creation
                    MinimaxTreeNode node = new MinimaxTreeNode();

                    // calculate estimation function only for leaf nodes
                    node.Player = (parentNode.Player == MinimaxPlayerType.MAX ? MinimaxPlayerType.MIN : MinimaxPlayerType.MAX);

                    node.ExistingLines.AddRange(parentNode.ExistingLines);
                    node.ExistingLines.Add(parentNode.NonExistingLines[i]);

                    node.NonExistingLines.AddRange(parentNode.NonExistingLines);
                    node.NonExistingLines.Remove(parentNode.NonExistingLines[i]);

                    node.Boxes.AddRange(parentNode.Boxes);
                    List<Box> newBoxes = AICommon.TryClosingBoxes(parentNode.ExistingLines, DeterminePlayerColour(node.Player), parentNode.NonExistingLines[i], out int[] notUsed);
                    node.Boxes.AddRange(newBoxes);

                    node.DeltaMove = parentNode.NonExistingLines[i];
                    #endregion

                    /* don't go through this subtree if the delta move is drawing third edge
                     * unless it the only move
                     */
                    AICommon.TryClosingBoxes(node.ExistingLines,
                                            DeterminePlayerColour(node.Player),
                                            node.DeltaMove,
                                            out int[] surroundingEdges);

                    if ((surroundingEdges[0] == 2 || surroundingEdges[1] == 2))
                    {
                        if (!(i == parentNode.NonExistingLines.Count - 1 && parentNode.Children.Count == 0))
                        {
                            Debug.WriteLine(node.DeltaMove.ToString() + " was thrown.");
                            continue;
                        }
                    }

                    node.EstimationScore = ConstructTree(node, depth + 1, MinimaxPlayerType.MIN, alpha, beta);

                    bestValue = (bestValue > node.EstimationScore ? bestValue : node.EstimationScore);
                    alpha = (alpha > bestValue ? alpha : bestValue);

                    parentNode.Children.Add(node);

                    if (beta <= alpha)
                        break;
                }

                parentNode.EstimationScore = bestValue;
                return bestValue;
            }
            else
            {
                int bestValue = int.MaxValue;

                for (int i = 0; i < parentNode.NonExistingLines.Count; i++)
                {
                    #region New Node Creation
                    MinimaxTreeNode node = new MinimaxTreeNode();

                    // calculate estimation function only for leaf nodes
                    node.Player = (parentNode.Player == MinimaxPlayerType.MAX ? MinimaxPlayerType.MIN : MinimaxPlayerType.MAX);

                    node.ExistingLines.AddRange(parentNode.ExistingLines);
                    node.ExistingLines.Add(parentNode.NonExistingLines[i]);

                    node.NonExistingLines.AddRange(parentNode.NonExistingLines);
                    node.NonExistingLines.Remove(parentNode.NonExistingLines[i]);

                    node.Boxes.AddRange(parentNode.Boxes);
                    List<Box> newBoxes = AICommon.TryClosingBoxes(parentNode.ExistingLines, DeterminePlayerColour(node.Player), parentNode.NonExistingLines[i], out int[] notUsed);
                    node.Boxes.AddRange(newBoxes);

                    node.DeltaMove = parentNode.NonExistingLines[i];
                    #endregion

                    /* don't go through this subtree if the delta move is drawing third edge
                     * unless it the only move
                     */

                    AICommon.TryClosingBoxes(node.ExistingLines,
                                            DeterminePlayerColour(node.Player),
                                            node.DeltaMove,
                                            out int[] surroundingEdges);

                    if ((surroundingEdges[0] == 2 || surroundingEdges[1] == 2))
                    {
                        if (!(i == parentNode.NonExistingLines.Count - 1 && parentNode.Children.Count == 0))
                        {
                            Debug.WriteLine(node.DeltaMove.ToString() + " was thrown.");
                            continue;
                        }
                    }

                    node.EstimationScore = ConstructTree(node, depth + 1, MinimaxPlayerType.MAX, alpha, beta);

                    bestValue = (bestValue < node.EstimationScore ? bestValue : node.EstimationScore);
                    beta = (beta < bestValue ? beta : bestValue);

                    parentNode.Children.Add(node);

                    if (beta <= alpha)
                        break;
                }

                parentNode.EstimationScore = bestValue;
                return bestValue;
            }

            throw new Exception("Something wrong in minimax happened");
        }

        private Player DeterminePlayerColour(MinimaxPlayerType minimaxPlayerType)
        {
            if (whoAmI == Player.BLUE)
            {
                if (minimaxPlayerType == MinimaxPlayerType.MAX)
                    return Player.BLUE;
                else
                    return Player.RED;
            }
            else
            {
                if (minimaxPlayerType == MinimaxPlayerType.MAX)
                    return Player.RED;
                else
                    return Player.BLUE;
            }
        }

        public LineBetweenCircles GetBestMove()
        {
            if (rootNode != null && rootNode.Children.Count > 0)
            {
                int indexMax = 0;
                int max = rootNode.Children[0].EstimationScore;

                for (int i = 1; i < rootNode.Children.Count; i++)
                {
                    if (rootNode.Children[i].EstimationScore > max)
                    {
                        indexMax = i;
                        max = rootNode.Children[i].EstimationScore;
                    }
                }

                return rootNode.Children[indexMax].DeltaMove;
            }
            else
                return null;
        }

        public MinimaxTreeNode RootNode
        {
            get
            {
                return rootNode;
            }
        }

        protected Player WhoHasMoreBoxes(MinimaxTreeNode node, out int blue, out int red)
        {
            blue = red = 0;

            for (int i = 0; i < node.Boxes.Count; i++)
                if (node.Boxes[i].ClosingPlayer == Player.BLUE)
                    blue++;
                else
                    red++;

            return (blue > red ? Player.BLUE : Player.RED);
        }

        #endregion

        #region Abstract Methods

        protected abstract int EstimationFunction(MinimaxTreeNode node);

        #endregion
    }
}