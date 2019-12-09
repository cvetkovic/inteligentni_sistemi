using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        #endregion

        #region Class Variables

        private delegate void GameStateChanged();
        private event GameStateChanged gameStateChangedEvent;

        private bool blueTurn;
        private int[] score = new int[2];
        private List<GameTurn> gameTurnsList = new List<GameTurn>();

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            blueTurn = true;
            score[(int)PLAYER.BLUE] = score[(int)PLAYER.RED] = 0;
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
        }
        
        #endregion
    }
}
