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
        }

        #endregion

        #region Class Variables

        private delegate void GameStateChanged();
        private event GameStateChanged gameStateChangedEvent;

        private bool blueTurn;
        private int[] score = new int[2];
        private List<GameTurn> gameTurnsList = new List<GameTurn>();

        private int horizontalSpacingBetweenCircles = 93;
        private int verticalSpacingBetweenCircles = 77;
        private int circleDiameter = 25;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            blueTurn = true;
            score[(int)PLAYER.BLUE] = score[(int)PLAYER.RED] = 0;
            currentGame = new CurrentGame((int)tableSizeX.Value, (int)tableSizeY.Value);
            GUI_GameSettingChanged(null, null);
        }

        #endregion

        #region GUI

        public void UpdateGUI()
        {
            blueTurnIndicator.Visible = blueTurn;
            redTurnIndicator.Visible = !blueTurn;

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

        #endregion

        private CurrentGame currentGame;
        private LineBetweenCircles mouseHoverLine;

        private List<LineBetweenCircles> existingCanvasLines = new List<LineBetweenCircles>();

        private Hashtable circleCenters = new Hashtable();

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush circleBrush = Brushes.White;
            Brush existingLineBrush = Brushes.Black;

            circleCenters.Clear();
            g.Clear(Color.SkyBlue);

            if (mouseHoverLine != null)
                g.DrawLine(Pens.Black, mouseHoverLine.From, mouseHoverLine.To);

            // drawing existing connections
            for (int i = 0; i < existingCanvasLines.Count; i++)
                g.DrawLine(Pens.Black, existingCanvasLines[i].From, existingCanvasLines[i].To);

            // drawing circles
            for (int i = 0; i < currentGame.TableSizeX; i++)        // for each row
            {
                for (int j = 0; j < currentGame.TableSizeY; j++)    // for each column
                {
                    int xStart = (j + 1) * (horizontalSpacingBetweenCircles) + j * circleDiameter;
                    int yStart = (i + 1) * (verticalSpacingBetweenCircles) + i * circleDiameter;

                    circleCenters.Add(new Tuple<int, int>(i, j), new Point(xStart + circleDiameter / 2, yStart + circleDiameter / 2));
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
                    Tuple<int, int> coordinatesFrom = (Tuple<int, int>)((DictionaryEntry)min).Key;
                    Debug.WriteLine("min coordinates (" + coordinatesFrom.Item1 + ", " + coordinatesFrom.Item2 + ")");

                    int dx = 0, dy = 0;
                    bool valid = false;

                    if ((e.Y > (minPoint.Y - verticalSpacingBetweenCircles / 2)) && (e.Y < minPoint.Y) &&
                        (Math.Abs(e.X - minPoint.X) <= 10))
                    {
                        // up
                        if (coordinatesFrom.Item1 != 0)
                        {
                            valid = true;
                            dx = -1;
                        }
                    }
                    else if ((e.Y > minPoint.Y) && (e.Y < (minPoint.Y + verticalSpacingBetweenCircles / 2)) &&
                        (Math.Abs(e.X - minPoint.X) <= 10))
                    {
                        // down
                        if (coordinatesFrom.Item1 != currentGame.TableSizeX - 1)
                        {
                            valid = true;
                            dx = 1;
                        }
                    }
                    else if ((e.X > (minPoint.X - horizontalSpacingBetweenCircles / 2)) && (e.X < minPoint.X) &&
                        (Math.Abs(e.Y - minPoint.Y) <= 10))
                    {
                        // left
                        if (coordinatesFrom.Item2 != 0)
                        {
                            valid = true;
                            dy = -1;
                        }
                    }
                    else if ((e.X > minPoint.X) && (e.X < (minPoint.X + horizontalSpacingBetweenCircles / 2)) &&
                        (Math.Abs(e.Y - minPoint.Y) <= 10))
                    {
                        // right
                        if (coordinatesFrom.Item2 != currentGame.TableSizeY - 1)
                        {
                            valid = true;
                            dy = 1;
                        }
                    }

                    if (valid)
                    {
                        Tuple<int, int> lookingFor = new Tuple<int, int>(coordinatesFrom.Item1 + dx, coordinatesFrom.Item2 + dy);

                        Debug.WriteLine("Looking for (" + (coordinatesFrom.Item1 + dx) + ", " + (coordinatesFrom.Item2 + dy) + ")");

                        if (circleCenters[lookingFor] != null)
                        {
                            coordinateTo = (Point)circleCenters[lookingFor];

                            LineBetweenCircles line = new LineBetweenCircles();
                            line.From = minPoint;
                            line.To = (Point)coordinateTo;

                            if (mouseHoverLine != null && mouseHoverLine.From == line.From && mouseHoverLine.To == line.To)
                                return;
                            else {
                                mouseHoverLine = line;
                                canvas.Refresh();
                            }
                        }
                    }
                }
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (mouseHoverLine != null)
            {
                // TODO: check here if not already added
                existingCanvasLines.Add(mouseHoverLine);
            }
        }
    }
}