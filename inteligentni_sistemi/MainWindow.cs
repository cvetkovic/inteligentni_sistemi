using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace etf.dotsandboxes.cl160127d
{
    public partial class MainWindow : Form
    {

        #region Internal Class Structures

        private enum PLAYER : int
        {
            BLUE = 0,
            RED = 1
        }

        private class GameTurn
        {
            PLAYER player;
            string field;

            public PLAYER Player
            {
                get { return player; }
                set { player = value; }
            }

            public String Field
            {
                get { return field; }
                set { field = value; }
            }
        }

        private class CurrentGame
        {
            public CurrentGame(int tableSizeX, int tableSizeY)
            {
                TableSizeX = tableSizeX;
                TableSizeY = tableSizeY;
            }

            public int TableSizeX { get; set; }
            public int TableSizeY { get; set; }
        }

        private class LineBetweenCircles
        {
            public Point From { get; set; }
            public Point To { get; set; }

            public VTuple<int, int> CoordinateFrom { get; set; }
            public VTuple<int, int> CoordinateTo { get; set; }
        }

        private class Box
        {
            public LineBetweenCircles Upper { get; set; }
            public LineBetweenCircles Lower { get; set; }
            public LineBetweenCircles Left { get; set; }
            public LineBetweenCircles Right { get; set; }

            public PLAYER ClosingPlayer { get; set; }

            public Point TopLeft { get; set; }
            public Point TopRight { get; set; }
            public Point BottomLeft { get; set; }
            public Point BottomRight { get; set; }
        }

        #endregion

        #region Class Variables

        private delegate void GameStateChanged();
        private event GameStateChanged gameStateChangedEvent;

        private PLAYER turn;
        private int[] score = new int[2];
        private List<GameTurn> gameTurnsList = new List<GameTurn>();

        private int horizontalSpacingBetweenCircles = 93;
        private int verticalSpacingBetweenCircles = 77;
        private int circleDiameter = 25;
        private const int LINE_WIDTH = 8;

        private CurrentGame currentGame;
        private LineBetweenCircles mouseHoverLine;

        private List<LineBetweenCircles> existingCanvasLines = new List<LineBetweenCircles>();
        private List<Box> boxes = new List<Box>();

        private Hashtable circleCenters = new Hashtable();

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            turn = PLAYER.BLUE;
            score[(int)PLAYER.BLUE] = score[(int)PLAYER.RED] = 0;
            currentGame = new CurrentGame((int)tableSizeX.Value, (int)tableSizeY.Value);
            GUI_GameSettingChanged(null, null);
        }

        #endregion

        #region GUI

        public void UpdateGUI()
        {
            blueTurnIndicator.Visible = (turn == PLAYER.BLUE);
            redTurnIndicator.Visible = (turn == PLAYER.RED);

            scoreLabel.Text = score[(int)PLAYER.BLUE] + " : " + score[(int)PLAYER.RED];

            turnRichTextBox.Clear();
            int currentPosition = 0;
            foreach (GameTurn t in gameTurnsList)
            {
                String textToAppend = t.Field + t.Player + Environment.NewLine;
                int length = textToAppend.Length;

                turnRichTextBox.AppendText(textToAppend);

                turnRichTextBox.SelectionStart = currentPosition;
                turnRichTextBox.SelectionLength = length;
                turnRichTextBox.SelectionColor = (t.Player == PLAYER.BLUE ? Color.Blue : Color.Red);
            }
        }

        private void GUI_GameSettingChanged(object sender, EventArgs e)
        {
            if (humanVsHumanRadio.Checked)
            {
                aiDifficulty.Enabled = false;
                aiTreeDepth.Enabled = false;
                aiMode.Enabled = false;

            }
            else if (humanVsPcRadio.Checked || pcVsPcRadio.Checked)
            {
                aiDifficulty.Enabled = true;
                aiTreeDepth.Enabled = true;
                aiMode.Enabled = true;
            }

            horizontalSpacingBetweenCircles = (canvas.Width - currentGame.TableSizeY * circleDiameter) / (currentGame.TableSizeY + 1);
            verticalSpacingBetweenCircles = (canvas.Height - currentGame.TableSizeX * circleDiameter) / (currentGame.TableSizeX + 1);
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush circleBrush = Brushes.White;
            Pen existingLinePen = new Pen(Brushes.Black, LINE_WIDTH);
            Pen hoverLine = new Pen(Brushes.Gray, LINE_WIDTH);
            Brush[] rectangleBrush = { Brushes.Blue, Brushes.Red };

            circleCenters.Clear();
            g.Clear(Color.SkyBlue);

            if (mouseHoverLine != null)
                g.DrawLine(hoverLine, mouseHoverLine.From, mouseHoverLine.To);

            // drawing colored rectangles
            for (int i = 0; i < boxes.Count; i++)
            {
                Rectangle rectangle = new Rectangle(boxes[i].TopLeft, new Size(boxes[i].TopRight.X - boxes[i].TopLeft.X, boxes[i].BottomLeft.Y - boxes[i].TopLeft.Y));
                g.FillRectangle(rectangleBrush[(int)boxes[i].ClosingPlayer], rectangle);
            }

            // drawing existing connections
            for (int i = 0; i < existingCanvasLines.Count; i++)
                g.DrawLine(existingLinePen, existingCanvasLines[i].From, existingCanvasLines[i].To);

            // drawing circles
            for (int i = 0; i < currentGame.TableSizeX; i++)        // for each row
            {
                for (int j = 0; j < currentGame.TableSizeY; j++)    // for each column
                {
                    int xStart = (j + 1) * (horizontalSpacingBetweenCircles) + j * circleDiameter;
                    int yStart = (i + 1) * (verticalSpacingBetweenCircles) + i * circleDiameter;

                    circleCenters.Add(new VTuple<int, int>(i, j), new Point(xStart + circleDiameter / 2, yStart + circleDiameter / 2));
                    g.FillEllipse(circleBrush, new Rectangle(new Point(xStart, yStart), new Size(circleDiameter, circleDiameter)));
                }
            }
        }

        private void TableSize_ValueChanged(object sender, EventArgs e)
        {
            currentGame = new CurrentGame((int)tableSizeX.Value, (int)tableSizeY.Value);
            GUI_GameSettingChanged(null, null);

            if (tableSizeX.Value <= 4 && tableSizeY.Value <= 4)
                circleDiameter = 35;
            else if ((tableSizeX.Value > 4 && tableSizeY.Value > 4) && (tableSizeX.Value < 10 && tableSizeY.Value < 10))
                circleDiameter = 25;
            else if (tableSizeX.Value >= 10 && tableSizeY.Value >= 10)
                circleDiameter = 15;
            else
                circleDiameter = 25;

            canvas.Refresh();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            object min = null;
            foreach (DictionaryEntry pair in circleCenters)
            {
                Point p = (Point)pair.Value;

                if (min == null)
                    min = pair;
                else if ((Math.Abs(e.X - p.X) <= (horizontalSpacingBetweenCircles + circleDiameter) / 2) &&
                        (Math.Abs(e.Y - p.Y) <= (verticalSpacingBetweenCircles + circleDiameter) / 2))
                    min = pair;
                else
                    continue;
            }

            if (min != null)
            {
                Point minPoint = (Point)(((DictionaryEntry)min).Value);
                Point? coordinateTo = null;

                if ((Math.Abs(e.X - minPoint.X) <= (horizontalSpacingBetweenCircles + circleDiameter) / 2) &&
                    (Math.Abs(e.Y - minPoint.Y) <= (verticalSpacingBetweenCircles + circleDiameter) / 2))
                {
                    VTuple<int, int> coordinatesFrom = (VTuple<int, int>)((DictionaryEntry)min).Key;
                    Debug.WriteLine("min coordinates (" + coordinatesFrom.Item1 + ", " + coordinatesFrom.Item2 + ")");

                    int dx = 0, dy = 0;
                    bool valid = false;

                    if ((e.Y >= (minPoint.Y - verticalSpacingBetweenCircles / 2 - circleDiameter / 2)) && (e.Y <= minPoint.Y - circleDiameter / 2) &&
                        (Math.Abs(e.X - minPoint.X) <= LINE_WIDTH))
                    {
                        // up
                        if (coordinatesFrom.Item1 != 0)
                        {
                            valid = true;
                            dx = -1;
                        }
                    }
                    else if ((e.Y >= minPoint.Y + circleDiameter / 2) && (e.Y <= (minPoint.Y + verticalSpacingBetweenCircles / 2 + circleDiameter / 2)) &&
                        (Math.Abs(e.X - minPoint.X) <= LINE_WIDTH))
                    {
                        // down
                        if (coordinatesFrom.Item1 != currentGame.TableSizeX - 1)
                        {
                            valid = true;
                            dx = 1;
                        }
                    }
                    else if ((e.X >= (minPoint.X - horizontalSpacingBetweenCircles / 2 - circleDiameter / 2)) && (e.X <= minPoint.X - circleDiameter / 2) &&
                        (Math.Abs(e.Y - minPoint.Y) <= LINE_WIDTH))
                    {
                        // left
                        if (coordinatesFrom.Item2 != 0)
                        {
                            valid = true;
                            dy = -1;
                        }
                    }
                    else if ((e.X >= minPoint.X + circleDiameter / 2) && (e.X <= (minPoint.X + horizontalSpacingBetweenCircles / 2 + circleDiameter / 2)) &&
                        (Math.Abs(e.Y - minPoint.Y) <= LINE_WIDTH))
                    {
                        // right
                        if (coordinatesFrom.Item2 != currentGame.TableSizeY - 1)
                        {
                            valid = true;
                            dy = 1;
                        }
                    }
                    else
                        valid = false;

                    if (valid)
                    {
                        VTuple<int, int> lookingFor = new VTuple<int, int>(coordinatesFrom.Item1 + dx, coordinatesFrom.Item2 + dy);

                        Debug.WriteLine("Looking for (" + (coordinatesFrom.Item1 + dx) + ", " + (coordinatesFrom.Item2 + dy) + ")");

                        if (circleCenters[lookingFor] != null)
                        {
                            coordinateTo = (Point)circleCenters[lookingFor];

                            LineBetweenCircles line = new LineBetweenCircles();
                            line.From = minPoint;
                            line.To = (Point)coordinateTo;
                            line.CoordinateFrom = coordinatesFrom;
                            line.CoordinateTo = lookingFor;

                            if (mouseHoverLine != null && mouseHoverLine.From == line.From && mouseHoverLine.To == line.To)
                                return;
                            else
                            {
                                mouseHoverLine = line;
                                canvas.Refresh();
                            }
                        }
                    }
                    else
                    {
                        // if introduced to reduce canvas refreshing
                        if (mouseHoverLine != null)
                        {
                            mouseHoverLine = null;
                            canvas.Refresh();
                        }
                    }
                }
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (mouseHoverLine != null)
            {
                // TODO: MANDATORY CHECK -------------> if not already added
                existingCanvasLines.Add(mouseHoverLine);
                TryClosingBoxes(mouseHoverLine);

                // TODO: change game stats
                //gameStateChangedEvent.Invoke();
                canvas.Refresh();

                mouseHoverLine = null;
            }
        }

        #endregion

        #region Game Logic

        private void TryClosingBoxes(LineBetweenCircles line)
        {
            int coordinateFromX = line.CoordinateFrom.Item1;
            int coordinateFromY = line.CoordinateFrom.Item2;
            int coordinateToX = line.CoordinateTo.Item1;
            int coordinateToY = line.CoordinateTo.Item2;

            if (coordinateFromX == coordinateToX)
            {
                // horizontal line

                // close upper box if line is not on the upper edge
                if (coordinateFromX != 0)
                {

                    for (int direction = 0; direction < 2; direction++)
                    {
                        int dx = (direction == 0 ? -1 : 1); // check for clojure both above and below the line

                        Box box = new Box();

                        bool upperLeft = false;
                        bool upperUpper = false;
                        bool upperRight = false;

                        for (int i = 0; i < existingCanvasLines.Count; i++)
                        {
                            // looking for left and right
                            if (coordinateFromY < coordinateToY)
                            {
                                if (direction == 0)
                                {
                                    box.BottomLeft = line.From;
                                    box.BottomRight = line.To;
                                }
                                else
                                {
                                    box.TopLeft = line.From;
                                    box.TopRight = line.To;
                                }

                                // 'from' is left edge
                                if (!upperLeft &&
                                    ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY)) ||
                                    (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY))))
                                {
                                    upperLeft = true;
                                    box.Left = existingCanvasLines[i];
                                }

                                // 'to' is right edge
                                if (!upperRight &&
                                    ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX + dx, coordinateToY)) ||
                                    (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX + dx, coordinateToY))))
                                {
                                    upperRight = true;
                                    box.Right = existingCanvasLines[i];
                                }

                                if (!upperUpper &&
                                    ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY + 1)) ||
                                    (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY + 1))))
                                {
                                    upperUpper = true;
                                    if (direction == 0)
                                        box.Upper = existingCanvasLines[i];
                                    else
                                        box.Lower = existingCanvasLines[i];
                                }
                            }
                            else
                            {
                                if (direction == 0)
                                {
                                    box.BottomLeft = line.To;
                                    box.BottomRight = line.From;
                                }
                                else
                                {
                                    box.TopLeft = line.To;
                                    box.TopRight = line.From;
                                }

                                // 'from' is right edge
                                if (!upperRight &&
                                    ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY)) ||
                                    (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY))))
                                {
                                    upperRight = true;
                                    box.Right = existingCanvasLines[i];
                                }

                                // 'to' is left edge
                                if (!upperLeft &&
                                    ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX + dx, coordinateToY)) ||
                                    (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX + dx, coordinateToY))))
                                {
                                    upperLeft = true;
                                    box.Left = existingCanvasLines[i];
                                }

                                if (!upperUpper &&
                                    ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX + dx, coordinateToY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY)) ||
                                    (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX + dx, coordinateToY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX + dx, coordinateFromY))))
                                {
                                    upperUpper = true;
                                    if (direction == 0)
                                        box.Upper = existingCanvasLines[i];
                                    else
                                        box.Lower = existingCanvasLines[i];
                                }
                            }

                            if (upperLeft && upperUpper && upperRight)
                                break;
                        }

                        // add to list of boxes
                        if (upperLeft && upperUpper && upperRight)
                        {
                            box.ClosingPlayer = turn;

                            if (direction == 0)
                            {
                                box.Lower = line;

                                box.TopLeft = (box.Upper.From.X < box.Upper.To.X ? box.Upper.From : box.Upper.To);
                                box.TopRight = (box.Upper.From.X > box.Upper.To.X ? box.Upper.From : box.Upper.To);
                            }
                            else
                            {
                                box.Upper = line;
                                box.BottomLeft = (box.Lower.From.X < box.Lower.To.X ? box.Lower.From : box.Lower.To);
                                box.BottomRight = (box.Lower.From.X > box.Lower.To.X ? box.Lower.From : box.Lower.To);
                            }

                            boxes.Add(box);
                        }
                    }
                }

            }
            else if (coordinateFromY == coordinateToY)
            {
                // vectical line

                // TODO: do vertical clojure













            }
            else
                throw new Exception("Diagonal connections now allowed.");
        }

        #endregion

        #region Artificial Intelligence



        #endregion

    }
}