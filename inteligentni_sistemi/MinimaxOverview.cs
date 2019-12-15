using static etf.dotsandboxes.cl160127d.AI.Minimax.Minimax;
using etf.dotsandboxes.cl160127d.Game;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace etf.dotsandboxes.cl160127d
{
    partial class MinimaxOverview : Form
    {
        private MinimaxTreeNode rootNode;

        private LineBetweenCircles deltaLine;
        private MinimaxTreeNode selectedNode;

        private int tableSizeX, tableSizeY;

        private class ModifiedTreeNode : TreeNode
        {
            public ModifiedTreeNode(MinimaxTreeNode minimaxTreeNode, 
                                    string text) : base(text)
            {
                MinimaxTreeNode = minimaxTreeNode;
            }

            public MinimaxTreeNode MinimaxTreeNode { get; set; }
        }

        public MinimaxOverview(MinimaxTreeNode rootNode,
                               int tableSizeX,
                               int tableSizeY)
        {
            InitializeComponent();

            this.rootNode = rootNode;
            this.tableSizeX = tableSizeX;
            this.tableSizeY = tableSizeY;

            MakeTreeStructure(rootNode, null);
        }

        private void MakeTreeStructure(MinimaxTreeNode parentMinimaxNode, TreeNode parentTreeNode)
        {
            if (parentMinimaxNode == null)
                throw new Exception("Minimax tree not provided.");

            for (int i = 0; i < parentMinimaxNode.Children.Count; i++)
            {
                ModifiedTreeNode tree = new ModifiedTreeNode(parentMinimaxNode.Children[i], parentMinimaxNode.Children[i].ToString());
                if (parentTreeNode == null)
                    minimaxTree.Nodes.Add(tree);
                else
                    parentTreeNode.Nodes.Add(tree);

                MakeTreeStructure(parentMinimaxNode.Children[i], tree);
            }
        }

        private void MinimaxTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                ModifiedTreeNode node = (ModifiedTreeNode)e.Node;
                selectedNode = node.MinimaxTreeNode;

                deltaLine = selectedNode.DeltaMove;

                canvas.Refresh();
            }
        }

        private int horizontalSpacingBetweenCircles, verticalSpacingBetweenCircles, circleDiameter;

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            if (selectedNode == null)
                return;

            Graphics g = e.Graphics;
            Brush circleBrush = Brushes.White;
            Pen existingLinePen = new Pen(Brushes.Black, MainWindow.LINE_WIDTH);
            Pen hoverLine = new Pen(Brushes.Gray, MainWindow.LINE_WIDTH);
            Brush[] rectangleBrush = { Brushes.Blue, Brushes.Red };

            // paint the background
            Logic.CalculateCanvasParameters(tableSizeX,
                                            tableSizeY,
                                            canvas.Width,
                                            canvas.Height,
                                            out horizontalSpacingBetweenCircles,
                                            out verticalSpacingBetweenCircles,
                                            out circleDiameter);
            g.Clear(Color.SkyBlue);

            // draw temporary user-friendly hover line
            if (deltaLine != null)
                g.DrawLine(hoverLine, deltaLine.From, deltaLine.To);

            // painting closed rectangles
            for (int i = 0; i < selectedNode.Boxes.Count; i++)
            {
                Rectangle rectangle = new Rectangle(selectedNode.Boxes[i].TopLeft, new Size(Math.Abs(selectedNode.Boxes[i].TopRight.X - selectedNode.Boxes[i].TopLeft.X), Math.Abs(selectedNode.Boxes[i].BottomLeft.Y - selectedNode.Boxes[i].TopLeft.Y)));
                g.FillRectangle(rectangleBrush[(int)selectedNode.Boxes[i].ClosingPlayer], rectangle);
            }

            // drawing existing lines
            for (int i = 0; i < selectedNode.ExistingLines.Count; i++)
                g.DrawLine(existingLinePen, selectedNode.ExistingLines[i].From, selectedNode.ExistingLines[i].To);

            // drawing circles
            for (int i = 0; i < tableSizeX; i++)        // for each row
            {
                for (int j = 0; j < tableSizeY; j++)    // for each column
                {
                    int xStart = (j + 1) * (horizontalSpacingBetweenCircles) + j * circleDiameter;
                    int yStart = (i + 1) * (verticalSpacingBetweenCircles) + i * circleDiameter;

                    // painting
                    g.FillEllipse(circleBrush, new Rectangle(new Point(xStart, yStart), new Size(circleDiameter, circleDiameter)));
                }
            }
        }
    }
}