using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        private class CurrentGame
        {
            public CurrentGame(int tableSizeX, int tableSizeY)
            {
                TableSizeX = tableSizeX;
                TableSizeY = tableSizeY;
            }

            public int TableSizeX { get; set; }
            public int TableSizeY { get; set; }

            public bool GameOver { get; set; }
        }

        private class LineBetweenCircles
        {
            public Point From { get; set; }
            public Point To { get; set; }

            public VTuple<int, int> CoordinateFrom { get; set; }
            public VTuple<int, int> CoordinateTo { get; set; }

            public PLAYER WhoDrew { get; set; }
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

        private PLAYER turn;
        private int[] score = new int[2];

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

        private void SaveGameState_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Gamesave (*.gs) | *.gs";
            saveFileDialog.Title = "Čuvanje stanje igre";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveGameState(saveFileDialog.FileName);
                MessageBox.Show("Igra je uspešno sačuvana.");
            }
        }

        private void LoadGameState_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Gamesave (*.gs) | *.gs";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Učitavanje stanje igre";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadGameState(openFileDialog.FileName);
                MessageBox.Show("Igra je uspešno učitana.");
            }
        }

        public void UpdateGUI()
        {
            if (currentGame.GameOver)
                tableSizeX.Enabled = tableSizeY.Enabled = humanVsHumanRadio.Enabled = humanVsPcRadio.Enabled = pcVsPcRadio.Enabled = true;

            blueTurnIndicator.Visible = (turn == PLAYER.BLUE);
            redTurnIndicator.Visible = (turn == PLAYER.RED);

            scoreLabel.Text = score[(int)PLAYER.BLUE] + " : " + score[(int)PLAYER.RED];
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
                Rectangle rectangle = new Rectangle(boxes[i].TopLeft, new Size(Math.Abs(boxes[i].TopRight.X - boxes[i].TopLeft.X), Math.Abs(boxes[i].BottomLeft.Y - boxes[i].TopLeft.Y)));
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
                            line.WhoDrew = turn;

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
            if (mouseHoverLine != null && !currentGame.GameOver)
            {
                // TODO: MANDATORY CHECK -------------> if not already added
                existingCanvasLines.Add(mouseHoverLine);

                ///////////////////////////////////
                string log = GenerateLogMessage(mouseHoverLine);

                int startPosition = turnRichTextBox.Text.Length;
                string textToAppend = log + Environment.NewLine;

                turnRichTextBox.AppendText(textToAppend);
                turnRichTextBox.SelectionStart = startPosition;
                turnRichTextBox.SelectionLength = log.Length;
                turnRichTextBox.SelectionColor = (turn == PLAYER.BLUE ? Color.Blue : Color.Red);
                turnRichTextBox.ScrollToCaret();
                ///////////////////////////////////

                int numberOfNewBoxes = TryClosingBoxes(mouseHoverLine);
                score[(int)turn] += numberOfNewBoxes;

                if (numberOfNewBoxes == 0)
                    SwitchTurn();

                canvas.Refresh();

                mouseHoverLine = null;

                if (boxes.Count == currentGame.TableSizeX * currentGame.TableSizeY / 2)
                    currentGame.GameOver = true;

                // has to be below game over set true to enable GUI controls
                UpdateGUI();

                if (currentGame.GameOver)
                    MessageBox.Show("Igra je završena");
            }
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            currentGame = new CurrentGame((int)tableSizeX.Value, (int)tableSizeY.Value);

            turn = PLAYER.BLUE;
            score[0] = score[1] = 0;
            mouseHoverLine = null;
            existingCanvasLines.Clear();
            boxes.Clear();
            circleCenters.Clear();

            GUI_GameSettingChanged(null, null);

            if (tableSizeX.Value <= 4 && tableSizeY.Value <= 4)
                circleDiameter = 35;
            else if ((tableSizeX.Value > 4 && tableSizeY.Value > 4) && (tableSizeX.Value < 10 && tableSizeY.Value < 10))
                circleDiameter = 25;
            else if (tableSizeX.Value >= 10 && tableSizeY.Value >= 10)
                circleDiameter = 15;
            else
                circleDiameter = 25;

            UpdateGUI();
            canvas.Refresh();
        }

        #endregion

        #region Game Logic

        private void SwitchTurn()
        {
            if (turn == PLAYER.BLUE)
                turn = PLAYER.RED;
            else
                turn = PLAYER.BLUE;
        }

        private int TryClosingBoxes(LineBetweenCircles line)
        {
            int createdBoxes = 0;

            int coordinateFromX = line.CoordinateFrom.Item1;
            int coordinateFromY = line.CoordinateFrom.Item2;
            int coordinateToX = line.CoordinateTo.Item1;
            int coordinateToY = line.CoordinateTo.Item2;

            if (coordinateFromX == coordinateToX)
            {
                // horizontal line

                for (int direction = 0; direction < 2; direction++)
                {
                    // DIRECTION(0) -> ABOVE
                    // DIRECTION(1) -> BELOW

                    // (line where x = 0 doesn't have upper element
                    if (coordinateFromX == 0 && direction == 0)
                        continue;

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

                                continue;
                            }

                            // 'to' is right edge
                            if (!upperRight &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX + dx, coordinateToY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX + dx, coordinateToY))))
                            {
                                upperRight = true;
                                box.Right = existingCanvasLines[i];

                                continue;
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

                                continue;
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

                                continue;
                            }

                            // 'to' is left edge
                            if (!upperLeft &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX + dx, coordinateToY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX + dx, coordinateToY))))
                            {
                                upperLeft = true;
                                box.Left = existingCanvasLines[i];

                                continue;
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

                                continue;
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
                        createdBoxes++;
                    }
                }
            }
            else if (coordinateFromY == coordinateToY)
            {
                // vectical line

                for (int direction = 0; direction < 2; direction++)
                {
                    // DIRECTION(0) -> LEFT
                    // DIRECTION(1) -> RIGHT

                    if (coordinateFromY == 0 && direction == 0)
                        continue;

                    int dy = (direction == 0 ? -1 : 1);

                    Box box = new Box();

                    bool upper = false;
                    bool left = false;
                    bool bottom = false;

                    for (int i = 0; i < existingCanvasLines.Count; i++)
                    {
                        if (coordinateFromX < coordinateToX)
                        {
                            if (direction == 0)
                            {
                                box.TopRight = line.From;
                                box.BottomRight = line.To;
                            }
                            else
                            {
                                box.TopLeft = line.From;
                                box.BottomLeft = line.To;
                            }

                            // 'from' is up
                            // 'to' is down
                            if (!bottom &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY)) || 
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY))))
                            {
                                bottom = true;
                                box.Lower = existingCanvasLines[i];

                                continue;
                            }

                            if (!upper &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY))))
                            {
                                upper = true;
                                box.Upper = existingCanvasLines[i];

                                continue;
                            }

                            if (!left &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy))))
                            {
                                left = true;
                                if (direction == 0)
                                    box.Left = existingCanvasLines[i];
                                else
                                    box.Right = existingCanvasLines[i];

                                continue;
                            }
                        }
                        else
                        {
                            if (direction == 0)
                            {
                                box.TopRight = line.To;
                                box.BottomRight = line.From;
                            }
                            else
                            {
                                box.TopLeft = line.To;
                                box.BottomLeft = line.From;
                            }

                            // 'from' is down
                            // 'to' is up
                            if (!bottom &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY))))
                            {
                                bottom = true;
                                box.Lower = existingCanvasLines[i];

                                continue;
                            }

                            if (!upper &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY))))
                            {
                                upper = true;
                                box.Upper = existingCanvasLines[i];

                                continue;
                            }

                            if (!left &&
                                ((existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateToX, coordinateToY + dy)) ||
                                (existingCanvasLines[i].CoordinateTo == new VTuple<int, int>(coordinateFromX, coordinateFromY + dy) && existingCanvasLines[i].CoordinateFrom == new VTuple<int, int>(coordinateToX, coordinateToY + dy))))
                            {
                                left = true;
                                if (direction == 0)
                                    box.Left = existingCanvasLines[i];
                                else
                                    box.Right = existingCanvasLines[i];

                                continue;
                            }
                        }
                    }

                    if (upper && left && bottom)
                    {
                        box.ClosingPlayer = turn;

                        if (direction == 0)
                        {
                            box.Right = line;

                            box.TopLeft = (box.Upper.From.X < box.Upper.To.X ? box.Upper.From : box.Upper.To);
                            box.BottomLeft = (box.Lower.From.X < box.Lower.To.X ? box.Lower.From : box.Lower.To);
                        }
                        else
                        {
                            box.Left = line;

                            box.TopRight = (box.Upper.From.X > box.Upper.To.X ? box.Upper.From : box.Upper.To);
                            box.BottomRight = (box.Lower.From.X > box.Lower.To.X ? box.Lower.From : box.Lower.To);
                        }

                        boxes.Add(box);
                        createdBoxes++;
                    }
                }
            }
            else
                throw new Exception("Diagonal connections now allowed.");

            return createdBoxes;
        }

        #endregion

        #region Game State Load/Save

        private char TranslateAxisToLetter(int coordinate)
        {
            return (char)(coordinate + 65);
        }

        private void SaveGameState(string location)
        {
            using (StreamWriter file = new StreamWriter(location))
            {
                // writing table size
                file.WriteLine(String.Format("{0} {1}", currentGame.TableSizeX, currentGame.TableSizeY));

                // writing moves
                foreach (LineBetweenCircles line in existingCanvasLines)
                    file.WriteLine(GenerateLogMessage(line));
            }
        }

        private string GenerateLogMessage(LineBetweenCircles line)
        {
            string s = "";

            if (line.From.X == line.To.X) // vertical line
                s = string.Format("{0}{1}", TranslateAxisToLetter(line.CoordinateFrom.Item1 < line.CoordinateTo.Item1 ? line.CoordinateFrom.Item1 : line.CoordinateTo.Item1), (line.CoordinateFrom.Item2 < line.CoordinateTo.Item2 ? line.CoordinateFrom.Item2 : line.CoordinateTo.Item2));
            else                        // horizontal line
                s = string.Format("{0}{1}", (line.CoordinateFrom.Item1 < line.CoordinateTo.Item1 ? line.CoordinateFrom.Item1 : line.CoordinateTo.Item1), TranslateAxisToLetter(line.CoordinateFrom.Item2 < line.CoordinateTo.Item2 ? line.CoordinateFrom.Item2 : line.CoordinateTo.Item2)); 

            return s;
        }

        private void LoadGameState(string location)
        {

        }

        #endregion

        #region Artificial Intelligence



        #endregion

    }
}