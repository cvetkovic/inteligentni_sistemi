using etf.dotsandboxes.cl160127d.AI;
using etf.dotsandboxes.cl160127d.Game;
using etf.dotsandboxes.cl160127d.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace etf.dotsandboxes.cl160127d
{
    public partial class MainWindow : Form
    {

        #region Class Variables

        // settings
        private int horizontalSpacingBetweenCircles = 93;
        private int verticalSpacingBetweenCircles = 77;
        private int circleDiameter = 25;
        private const int LINE_WIDTH = 8;

        // not game data structures
        private LineBetweenCircles mouseHoverLine;

        // game data structures
        private CurrentGame currentGame;

        private Hashtable circleCenters = new Hashtable();
        
        private List<LineBetweenCircles> existingCanvasLines = new List<LineBetweenCircles>();
        private List<LineBetweenCircles> nonExistingLines = new List<LineBetweenCircles>();
        private List<Box> boxes = new List<Box>();

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            // TODO: for DEBUG only
            IntermediateAI intermediateAI = new IntermediateAI(existingCanvasLines, nonExistingLines, boxes, 1);
            currentGame = new CurrentGame((int)tableSizeX.Value, (int)tableSizeY.Value, intermediateAI);
            intermediateAI.SetCurrentGame(currentGame);

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
            tableSizeX.Enabled = tableSizeY.Enabled = humanVsHumanRadio.Enabled = humanVsPcRadio.Enabled = pcVsPcRadio.Enabled = (currentGame.GameOver);

            blueTurnIndicator.Visible = (currentGame.Turn == Player.BLUE);
            redTurnIndicator.Visible = (currentGame.Turn == Player.RED);

            scoreLabel.Text = currentGame.Score[(int)Player.BLUE] + " : " + currentGame.Score[(int)Player.RED];
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
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush circleBrush = Brushes.White;
            Pen existingLinePen = new Pen(Brushes.Black, LINE_WIDTH);
            Pen hoverLine = new Pen(Brushes.Gray, LINE_WIDTH);
            Brush[] rectangleBrush = { Brushes.Blue, Brushes.Red };

            // paint the background
            circleCenters.Clear();
            g.Clear(Color.SkyBlue);

            // draw temporary user-friendly hover line
            if (mouseHoverLine != null)
                g.DrawLine(hoverLine, mouseHoverLine.From, mouseHoverLine.To);

            // painting closed rectangles
            for (int i = 0; i < boxes.Count; i++)
            {
                Rectangle rectangle = new Rectangle(boxes[i].TopLeft, new Size(Math.Abs(boxes[i].TopRight.X - boxes[i].TopLeft.X), Math.Abs(boxes[i].BottomLeft.Y - boxes[i].TopLeft.Y)));
                g.FillRectangle(rectangleBrush[(int)boxes[i].ClosingPlayer], rectangle);
            }

            // drawing existing lines
            for (int i = 0; i < existingCanvasLines.Count; i++)
                g.DrawLine(existingLinePen, existingCanvasLines[i].From, existingCanvasLines[i].To);

            // drawing circles
            for (int i = 0; i < currentGame.TableSizeX; i++)        // for each row
            {
                for (int j = 0; j < currentGame.TableSizeY; j++)    // for each column
                {
                    int xStart = (j + 1) * (horizontalSpacingBetweenCircles) + j * circleDiameter;
                    int yStart = (i + 1) * (verticalSpacingBetweenCircles) + i * circleDiameter;

                    // adding to dictionary all circles
                    circleCenters.Add(new VTuple<int, int>(i, j), new Point(xStart + circleDiameter / 2, yStart + circleDiameter / 2));
                    // painting
                    g.FillEllipse(circleBrush, new Rectangle(new Point(xStart, yStart), new Size(circleDiameter, circleDiameter)));
                }
            }

            // has to be done after initial drawing
            if (nonExistingLines.Count == 0 && !currentGame.GameOver)
                CreateNonExistingMovesList();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // finding the closes circle
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
                // closest circle found
                Point minPoint = (Point)(((DictionaryEntry)min).Value);
                Point? coordinateTo = null;

                // if in designated line area
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

                    // signal drawing of temporary line
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
                            line.WhoDrew = currentGame.Turn;

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
                        // clear non-existing hover line because user has exited the area
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
                FinishTurn(mouseHoverLine);

                mouseHoverLine = null;
                // refreshing done inside FinishTurn(...)
            }
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            BasePlayer opponent = null;

            if (humanVsHumanRadio.Checked)
            {
                opponent = null;
            }
            else if (humanVsPcRadio.Checked)
            {
                switch (aiDifficulty.SelectedIndex)
                {
                    case 0:
                        opponent = new BeginnerAI(existingCanvasLines, nonExistingLines, boxes);

                        break;
                    case 1:
                        opponent = new IntermediateAI(existingCanvasLines, nonExistingLines, boxes, (int)aiTreeDepth.Value);

                        break;
                    case 2:
                        opponent = new ExpertAI(existingCanvasLines, nonExistingLines, boxes, (int)aiTreeDepth.Value);

                        break;
                    default:
                        throw new Exception("Required AI difficulty level doesn't exist.");
                }
            }
            else // TODO: make simulation mode (two PCs playing one against another)
                throw new NotImplementedException();
            
            // creating new game
            currentGame = new CurrentGame((int)tableSizeX.Value, (int)tableSizeY.Value, opponent);
            if (currentGame.Opponent != null)
                currentGame.Opponent.SetCurrentGame(currentGame);

            CalculateCanvasParameters();
            CreateNonExistingMovesList();

            // clearing existing data structures
            mouseHoverLine = null;
            existingCanvasLines.Clear();
            boxes.Clear();
            circleCenters.Clear();

            UpdateGUI();
            canvas.Refresh();
        }

        #endregion

        #region Game Logic

        private void SwitchTurn()
        {
            if (currentGame.Turn == Player.BLUE)
                currentGame.Turn = Player.RED;
            else
                currentGame.Turn = Player.BLUE;
        }

        private void CalculateCanvasParameters()
        {
            horizontalSpacingBetweenCircles = (canvas.Width - currentGame.TableSizeY * circleDiameter) / (currentGame.TableSizeY + 1);
            verticalSpacingBetweenCircles = (canvas.Height - currentGame.TableSizeX * circleDiameter) / (currentGame.TableSizeX + 1);

            if (tableSizeX.Value <= 4 && tableSizeY.Value <= 4)
                circleDiameter = 35;
            else if ((tableSizeX.Value > 4 && tableSizeY.Value > 4) && (tableSizeX.Value < 10 && tableSizeY.Value < 10))
                circleDiameter = 25;
            else if (tableSizeX.Value >= 10 && tableSizeY.Value >= 10)
                circleDiameter = 15;
            else
                circleDiameter = 25;
        }

        private void FinishTurn(LineBetweenCircles line)
        {
            // will return false if clicked on line that had already been drawn on the canvas
            if (!TransferFromNonExistingToExisting(line))
                return;

            ///////////////////////////////////
            string log = GenerateLogMessage(line);

            int startPosition = turnRichTextBox.Text.Length;
            string textToAppend = log + Environment.NewLine;

            turnRichTextBox.AppendText(textToAppend);
            turnRichTextBox.SelectionStart = startPosition;
            turnRichTextBox.SelectionLength = log.Length;
            turnRichTextBox.SelectionColor = (currentGame.Turn == Player.BLUE ? Color.Blue : Color.Red);
            turnRichTextBox.ScrollToCaret();
            ///////////////////////////////////

            int numberOfNewBoxes = AICommon.TryClosingBoxes(existingCanvasLines, currentGame, boxes, line, true);
            currentGame.Score[(int)currentGame.Turn] += numberOfNewBoxes;

            if (numberOfNewBoxes > 0)
            {
                canvas.Refresh();
                UpdateGUI();
            }

            if (boxes.Count == currentGame.TableSizeX * currentGame.TableSizeY / 2)
                currentGame.GameOver = true;

            if (currentGame.GameOver)
            {
                MessageBox.Show("Igra je završena");
            }
            else
            {
                if (currentGame.Turn == Player.BLUE)
                {
                    if (numberOfNewBoxes == 0)
                    {
                        SwitchTurn();

                        if (currentGame.Opponent != null)
                            FinishTurn(currentGame.Opponent.MakeTurn());
                    }

                    UpdateGUI();
                }
                else
                {
                    if (numberOfNewBoxes == 0)
                        SwitchTurn();
                    else if (numberOfNewBoxes > 0 && currentGame.Opponent != null)
                    {
                        UpdateGUI();
                        Thread.Sleep(1000);

                        FinishTurn(currentGame.Opponent.MakeTurn());
                    }

                    UpdateGUI();
                }
                
            }
        }

        private bool TransferFromNonExistingToExisting(LineBetweenCircles line)
        {
            for (int i = 0; i < nonExistingLines.Count; i++)
            {
                if ((nonExistingLines[i].CoordinateFrom == line.CoordinateFrom && nonExistingLines[i].CoordinateTo == line.CoordinateTo) ||
                    (nonExistingLines[i].CoordinateTo == line.CoordinateFrom && nonExistingLines[i].CoordinateFrom == line.CoordinateTo))
                {
                    LineBetweenCircles trueLine = nonExistingLines[i];

                    nonExistingLines.RemoveAt(i);
                    existingCanvasLines.Add(trueLine);

                    return true;
                }
            }

            return false;
        }

        private void CreateNonExistingMovesList()
        {
            nonExistingLines.Clear();

            // horizontal lines
            for (int i = 0; i < currentGame.TableSizeX; i++)            // rows
            {
                for (int j = 0; j < currentGame.TableSizeY - 1; j++)    // columns
                {
                    LineBetweenCircles newLine = new LineBetweenCircles();
                    newLine.CoordinateFrom = new VTuple<int, int>(i, j);
                    newLine.CoordinateTo = new VTuple<int, int>(i, j + 1);

                    newLine.From = (Point)circleCenters[newLine.CoordinateFrom];
                    newLine.To = (Point)circleCenters[newLine.CoordinateTo];

                    nonExistingLines.Add(newLine);
                }
            }

            // vertical lines
            for (int j = 0; j < currentGame.TableSizeY; j++)            // rows
            {
                for (int i = 0; i < currentGame.TableSizeX - 1; i++)    // columns
                {
                    LineBetweenCircles newLine = new LineBetweenCircles();
                    newLine.CoordinateFrom = new VTuple<int, int>(i, j);
                    newLine.CoordinateTo = new VTuple<int, int>(i + 1, j);

                    newLine.From = (Point)circleCenters[newLine.CoordinateFrom];
                    newLine.To = (Point)circleCenters[newLine.CoordinateTo];

                    nonExistingLines.Add(newLine);
                }
            }

            Miscellaneous.ShuffleList(nonExistingLines);
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
            throw new NotImplementedException();

            using (StreamReader file = new StreamReader(location))
            {
                string firstLine = file.ReadLine();
                int sizeX = Int32.Parse(firstLine.Substring(0, firstLine.IndexOf(' ')));
                int sizeY = Int32.Parse(firstLine.Substring(firstLine.IndexOf(' ') + 1));

                CurrentGame currentGame = new CurrentGame(sizeX, sizeY);

                string line;
                while ((line = file.ReadLine()) != null)
                {
                    throw new NotImplementedException();
                }
            }
        }

        #endregion

    }
}