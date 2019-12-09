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

            // drawing existing connections
            /*for (int i = 0; i < existingCanvasLines.Count; i++)
            {
                Rectangle r = new Rectangle();
                g.FillRectangle(existingLineBrush, r);
            }*/

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

                if ((Math.Abs(e.X - minPoint.X) <= (horizontalSpacingBetweenCircles + circleDiameter) / 2) &&
                    (Math.Abs(e.Y - minPoint.Y) <= (verticalSpacingBetweenCircles + circleDiameter) / 2))
                {
                    Tuple<int, int> coordinatesFrom = (Tuple<int, int>)((DictionaryEntry)min).Key;
                    Debug.WriteLine("min coordinates (" + coordinatesFrom.Item1 + ", " + coordinatesFrom.Item2 + ")");

                    Point? coordinateTo = null;
                    int dx = 0, dy = 0;

                    if (e.X < minPoint.X)
                    {
                        // left border check
                        if (coordinatesFrom.Item2 <= 0)
                        {
                            coordinateTo = null;
                            return;
                        }
                        else
                        {
                            Debug.WriteLine("LEFT");

                            dx = -1;

                            if (e.Y < minPoint.Y)
                            {
                                // upper border check
                                if (coordinatesFrom.Item1 <= 0)
                                {
                                    coordinateTo = null;
                                    return;
                                }
                                else
                                {
                                    Debug.WriteLine("UP");

                                    dy = -1;
                                }
                            }
                            else if (e.Y > minPoint.Y)
                            {
                                // bottom border check
                                if (coordinatesFrom.Item1 >= currentGame.TableSizeX - 1)
                                {
                                    coordinateTo = null;
                                    return;
                                }
                                else
                                {
                                    Debug.WriteLine("DOWN");

                                    dy = +1;
                                }
                            }
                        }
                    }
                    else if (e.X > minPoint.X)
                    {
                        // right border check
                        if (coordinatesFrom.Item2 >= currentGame.TableSizeY - 1)
                        {
                            coordinateTo = null;
                            return;
                        }
                        else
                        {
                            Debug.WriteLine("RIGHT");

                            dx = 1;

                            if (e.Y < minPoint.Y)
                            {
                                // upper border check
                                if (coordinatesFrom.Item1 <= 0)
                                {
                                    coordinateTo = null;
                                    return;
                                }
                                else
                                {
                                    Debug.WriteLine("UP");

                                    dy = -1;
                                }
                            }
                            else if (e.Y > minPoint.Y)
                            {
                                // bottom border check
                                if (coordinatesFrom.Item1 >= currentGame.TableSizeX - 1)
                                {
                                    coordinateTo = null;
                                    return;
                                }
                                else
                                {
                                    Debug.WriteLine("DOWN");

                                    dy = +1;
                                }
                            }
                        }
                    }

                    Tuple<int, int> lookingFor = new Tuple<int, int>(coordinatesFrom.Item1 + dx, coordinatesFrom.Item2 + dy);
                    coordinateTo = (Point)circleCenters[lookingFor];

                    if (coordinateTo == null)
                        mouseHoverLine = null;
                    else
                    {

                    }
                }
            }
        }
    }
}