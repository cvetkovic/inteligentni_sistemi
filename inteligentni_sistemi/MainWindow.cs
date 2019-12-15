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
        public const int LINE_WIDTH = 8;

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
            IntermediateAI intermediateAI = new IntermediateAI(existingCanvasLines, nonExistingLines, boxes, Player.RED, 3);
            currentGame = new CurrentGame((int)tableSizeX.Value, (int)tableSizeY.Value, intermediateAI);
            intermediateAI.SetCurrentGame(currentGame);

            Logic.CalculateCanvasParameters(currentGame.TableSizeX,
                                            currentGame.TableSizeY,
                                            canvas.Width,
                                            canvas.Height,
                                            out horizontalSpacingBetweenCircles, 
                                            out verticalSpacingBetweenCircles, 
                                            out circleDiameter);    // don't remove this
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
            aiDifficulty.Enabled = aiTreeDepth.Enabled = aiMode.Enabled = (currentGame.GameOver);
            aiMinimaxTree.Enabled = (!currentGame.GameOver && currentGame.Opponent != null);

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
            Logic.CalculateCanvasParameters(currentGame.TableSizeX,
                                            currentGame.TableSizeY,
                                            canvas.Width,
                                            canvas.Height,
                                            out horizontalSpacingBetweenCircles,
                                            out verticalSpacingBetweenCircles,
                                            out circleDiameter);
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
                    //Debug.WriteLine("min coordinates (" + coordinatesFrom.Item1 + ", " + coordinatesFrom.Item2 + ")");

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

                        //Debug.WriteLine("Looking for (" + (coordinatesFrom.Item1 + dx) + ", " + (coordinatesFrom.Item2 + dy) + ")");

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
            if (currentGame == null)
            {
                MessageBox.Show("Igra nije započeta!");
                return;
            }

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
                if (aiDifficulty.SelectedIndex == -1)
                {
                    MessageBox.Show("Težina protivničkog igrača nije izabrana.");
                    return;
                }
                else if (aiMode.SelectedIndex == -1)
                {
                    MessageBox.Show("Režim rada protivničkog igrača nije izabran.");
                    return;
                }

                switch (aiDifficulty.SelectedIndex)
                {
                    case 0:
                        opponent = new BeginnerAI(existingCanvasLines, nonExistingLines, boxes);

                        break;
                    case 1:
                        opponent = new IntermediateAI(existingCanvasLines, nonExistingLines, boxes, Player.RED, (int)aiTreeDepth.Value);

                        break;
                    case 2:
                        opponent = new ExpertAI(existingCanvasLines, nonExistingLines, boxes, Player.RED, (int)aiTreeDepth.Value);

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

            Logic.CalculateCanvasParameters(currentGame.TableSizeX,
                                            currentGame.TableSizeY,
                                            canvas.Width,
                                            canvas.Height,
                                            out horizontalSpacingBetweenCircles,
                                            out verticalSpacingBetweenCircles,
                                            out circleDiameter);

            // clearing existing data structures
            mouseHoverLine = null;
            existingCanvasLines.Clear();
            boxes.Clear();
            circleCenters.Clear();
            turnRichTextBox.Clear();

            UpdateGUI();
            canvas.Refresh();

            CreateNonExistingMovesList();
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
        
        private void FinishTurn(LineBetweenCircles line)
        {
            if (line == null)
                throw new Exception("AI returned null for the next move.");

            // will return false if clicked on line that had already been drawn on the canvas
            if (!TransferFromNonExistingToExisting(line))
                return;

            ///////////////////////////////////
            string log = line.ToString();

            int startPosition = turnRichTextBox.Text.Length;
            string textToAppend = log + Environment.NewLine;

            turnRichTextBox.AppendText(textToAppend);
            turnRichTextBox.SelectionStart = startPosition;
            turnRichTextBox.SelectionLength = log.Length;
            turnRichTextBox.SelectionColor = (currentGame.Turn == Player.BLUE ? Color.Blue : Color.Red);
            turnRichTextBox.ScrollToCaret();
            ///////////////////////////////////

            List<Box> newBoxes = AICommon.TryClosingBoxes(existingCanvasLines, currentGame.Turn, line, out int[] notUsed);
            boxes.AddRange(newBoxes);
            currentGame.Score[(int)currentGame.Turn] += newBoxes.Count;

            if (newBoxes.Count > 0)
            {
                canvas.Refresh();
                UpdateGUI();
            }

            if (boxes.Count == (currentGame.TableSizeX - 1) * (currentGame.TableSizeY - 1))
                currentGame.GameOver = true;

            if (currentGame.GameOver)
            {
                UpdateGUI();
                MessageBox.Show("Igra je završena");
            }
            else
            {
                if (currentGame.Turn == Player.BLUE)
                {
                    if (newBoxes.Count == 0)
                    {
                        SwitchTurn();

                        if (currentGame.Opponent != null)
                            FinishTurn(currentGame.Opponent.MakeTurn());
                    }

                    UpdateGUI();
                }
                else
                {
                    if (newBoxes.Count == 0)
                        SwitchTurn();
                    else if (newBoxes.Count > 0 && currentGame.Opponent != null)
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

        private void SaveGameState(string location)
        {
            using (StreamWriter file = new StreamWriter(location))
            {
                // writing table size
                file.WriteLine(String.Format("{0} {1}", currentGame.TableSizeX, currentGame.TableSizeY));

                // writing moves
                foreach (LineBetweenCircles line in existingCanvasLines)
                    file.WriteLine(line.ToString());
            }
        }

        private void LoadGameState(string location)
        {
            using (StreamReader file = new StreamReader(location))
            {
                string firstLine = file.ReadLine();
                int sizeX = Int32.Parse(firstLine.Substring(0, firstLine.IndexOf(' ')));
                int sizeY = Int32.Parse(firstLine.Substring(firstLine.IndexOf(' ') + 1));

                currentGame = new CurrentGame(sizeX, sizeY);
                Logic.CalculateCanvasParameters(currentGame.TableSizeX,
                                            currentGame.TableSizeY,
                                            canvas.Width,
                                            canvas.Height,
                                            out horizontalSpacingBetweenCircles,
                                            out verticalSpacingBetweenCircles,
                                            out circleDiameter);

                existingCanvasLines.Clear();
                nonExistingLines.Clear();
                boxes.Clear();
                canvas.Refresh();        // needed to create circleCenters
                turnRichTextBox.Clear();
                CreateNonExistingMovesList();

                tableSizeX.Value = sizeX;
                tableSizeY.Value = sizeY;
                humanVsHumanRadio.Checked = true;
                aiDifficulty.SelectedIndex = aiMode.SelectedIndex = -1;
                aiTreeDepth.Value = 1;

                UpdateGUI();

                string line;
                while ((line = file.ReadLine()) != null)
                {
                    String first, second;

                    if (Char.IsDigit(line[0]))          // first is digit
                    {
                        first = line[0].ToString();
                        if (Char.IsDigit(line[1]))
                        {
                            first += line[1].ToString();
                            second = line.Substring(2);
                        }
                        else
                            second = line.Substring(1);
                    }
                    else                                // first is letter
                    {
                        first = line[0].ToString();
                        second = line.Substring(1);
                    }

                    MakeArtificialMouseHoverLine(first, second);
                }

                canvas.Refresh();
            }
        }

        private void MakeArtificialMouseHoverLine(string first, string second)
        {
            LineBetweenCircles line = new LineBetweenCircles();

            int tmp1, tmp2;

            if (Int32.TryParse(first, out tmp1))                // horizontal line
            {
                tmp2 = Miscellaneous.TranslateLetterToAxis(second[0]);

                line.CoordinateFrom = new VTuple<int, int>(tmp1, tmp2);
                line.CoordinateTo = new VTuple<int, int>(tmp1, tmp2 + 1);
            }
            else                                                // vertical line
            {
                tmp1 = Miscellaneous.TranslateLetterToAxis(first[0]);
                tmp2 = Int32.Parse(second);

                line.CoordinateFrom = new VTuple<int, int>(tmp1, tmp2);
                line.CoordinateTo = new VTuple<int, int>(tmp1 + 1, tmp2);
            }

            line.From = (Point)circleCenters[line.CoordinateFrom];
            line.To = (Point)circleCenters[line.CoordinateTo];
            line.WhoDrew = currentGame.Turn;

            FinishTurn(line);
        }

        #endregion

        private void AiMinimaxTree_Click(object sender, EventArgs e)
        {
            if (currentGame.Opponent == null)
                throw new Exception("Not in proper mode to show minimax turn table.");
            else if (currentGame.Opponent is BeginnerAI)
                throw new Exception("Beginner opponent does not utilize minimax algorithm.");

            new MinimaxOverview(currentGame.Opponent.GetMinimaxTreeNode(),
                                currentGame.TableSizeX,
                                currentGame.TableSizeY).ShowDialog();
        }
    }
}