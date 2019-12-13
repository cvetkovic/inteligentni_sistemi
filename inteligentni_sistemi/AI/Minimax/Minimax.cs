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
            node.Player = MinimaxPlayerType.MAX;

            node.ExistingLines.AddRange(initialExistingLines);
            node.NonExistingLines.AddRange(initialNonExistingLines);

            ConstructTree(node, 0);

            return node;
        }

        private void ConstructTree(TreeNode parentNode, int depth)
        {
            if (depth == maxTreeDepth)
                return;

            for (int i = 0; i < parentNode.NonExistingLines.Count; i++)
            {
                TreeNode node = new TreeNode();

                node.EstimationScore = EstimationFunction(node);
                node.Player = (parentNode.Player == MinimaxPlayerType.MAX ? MinimaxPlayerType.MIN : MinimaxPlayerType.MAX);

                node.ExistingLines.AddRange(parentNode.ExistingLines);
                node.ExistingLines.Add(parentNode.NonExistingLines[i]);

                node.NonExistingLines.AddRange(parentNode.NonExistingLines);
                node.NonExistingLines.Remove(parentNode.NonExistingLines[i]);

                node.DeltaMove = parentNode.NonExistingLines[i];
                parentNode.Children.Add(node);

                ConstructTree(node, depth + 1);
            }
        }

        public LineBetweenCircles GetBestMove()
        {
            return null;
        }

        protected abstract int EstimationFunction(TreeNode node);
    }
}