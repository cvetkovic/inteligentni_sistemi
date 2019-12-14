using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.AI.Minimax
{
    abstract class Minimax
    {
        #region Class Variables

        private readonly List<LineBetweenCircles> initialExistingLines;
        private readonly List<LineBetweenCircles> initialNonExistingLines;

        private TreeNode rootNode;
        private int maxTreeDepth;

        #endregion

        #region Internal Classes

        public enum MinimaxPlayerType
        {
            MAX = 0,
            MIN = 1
        }

        public class TreeNode
        {
            private List<LineBetweenCircles> existingLines = new List<LineBetweenCircles>();
            private List<LineBetweenCircles> nonExistingLines = new List<LineBetweenCircles>();
            private List<TreeNode> children = new List<TreeNode>();

            public MinimaxPlayerType Player { get; set; }
            public List<TreeNode> Children
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
        }

        #endregion

        #region Base Class Constructor

        protected Minimax(List<LineBetweenCircles> existingLines,
                          List<LineBetweenCircles> nonExistingLines,
                          int maxTreeDepth)
        {
            this.initialExistingLines = existingLines;
            this.initialNonExistingLines = nonExistingLines;
            this.maxTreeDepth = maxTreeDepth;

            rootNode = ConstructRootNode();
        }

        #endregion

        private TreeNode ConstructRootNode()
        {
            TreeNode node = new TreeNode();

            node.EstimationScore = 0;
            node.Player = MinimaxPlayerType.MIN;

            node.ExistingLines.AddRange(initialExistingLines);
            node.NonExistingLines.AddRange(initialNonExistingLines);

            ConstructTree(node, 0, MinimaxPlayerType.MIN, int.MinValue, int.MaxValue);

            return node;
        }

        private int ConstructTree(TreeNode parentNode, int depth, MinimaxPlayerType minimaxPlayer, int alpha, int beta)
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
                    TreeNode node = new TreeNode();

                    // calculate estimation function only for leaf nodes
                    node.Player = (parentNode.Player == MinimaxPlayerType.MAX ? MinimaxPlayerType.MIN : MinimaxPlayerType.MAX);

                    node.ExistingLines.AddRange(parentNode.ExistingLines);
                    node.ExistingLines.Add(parentNode.NonExistingLines[i]);

                    node.NonExistingLines.AddRange(parentNode.NonExistingLines);
                    node.NonExistingLines.Remove(parentNode.NonExistingLines[i]);

                    node.DeltaMove = parentNode.NonExistingLines[i];
                    #endregion

                    node.EstimationScore = ConstructTree(node, depth + 1, MinimaxPlayerType.MIN, alpha, beta);
                    if (this is IntermediateMinimax && (node.EstimationScore == -1) || (node.EstimationScore == -2))
                        continue;

                    bestValue = (bestValue > node.EstimationScore ? bestValue : node.EstimationScore);
                    alpha = (alpha > bestValue ? alpha : bestValue);
                    if (beta <= alpha)
                        break;

                    parentNode.Children.Add(node);
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
                    TreeNode node = new TreeNode();

                    // calculate estimation function only for leaf nodes
                    node.Player = (parentNode.Player == MinimaxPlayerType.MAX ? MinimaxPlayerType.MIN : MinimaxPlayerType.MAX);

                    node.ExistingLines.AddRange(parentNode.ExistingLines);
                    node.ExistingLines.Add(parentNode.NonExistingLines[i]);

                    node.NonExistingLines.AddRange(parentNode.NonExistingLines);
                    node.NonExistingLines.Remove(parentNode.NonExistingLines[i]);

                    node.DeltaMove = parentNode.NonExistingLines[i];
                    #endregion

                    node.EstimationScore = ConstructTree(node, depth + 1, MinimaxPlayerType.MAX, alpha, beta);
                    if (this is IntermediateMinimax && (node.EstimationScore == -1) || (node.EstimationScore == -2))
                        continue;

                    bestValue = (bestValue < node.EstimationScore ? bestValue : node.EstimationScore);
                    beta = (beta < bestValue ? beta : bestValue);
                    if (beta <= alpha)
                        break;

                    parentNode.Children.Add(node);
                }

                parentNode.EstimationScore = bestValue;
                return bestValue;
            }

            throw new Exception("Something wrong in minimax happened");
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

        protected abstract int EstimationFunction(TreeNode node);
    }
}