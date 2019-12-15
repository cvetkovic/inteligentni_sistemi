namespace etf.dotsandboxes.cl160127d
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.redTurnIndicator = new System.Windows.Forms.Label();
            this.blueTurnIndicator = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.scoreLabel = new System.Windows.Forms.Label();
            this.canvas = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.aiMinimaxTree = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.aiMode = new System.Windows.Forms.ComboBox();
            this.aiTreeDepth = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.newGame = new System.Windows.Forms.Button();
            this.loadGameState = new System.Windows.Forms.Button();
            this.saveGameState = new System.Windows.Forms.Button();
            this.aiDifficulty = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.pcVsPcRadio = new System.Windows.Forms.RadioButton();
            this.humanVsPcRadio = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.humanVsHumanRadio = new System.Windows.Forms.RadioButton();
            this.tableSizeY = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.tableSizeX = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.turnRichTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aiTreeDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableSizeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableSizeX)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.redTurnIndicator);
            this.groupBox1.Controls.Add(this.blueTurnIndicator);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.scoreLabel);
            this.groupBox1.Location = new System.Drawing.Point(618, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 81);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rezultat";
            // 
            // redTurnIndicator
            // 
            this.redTurnIndicator.AutoSize = true;
            this.redTurnIndicator.Location = new System.Drawing.Point(269, 57);
            this.redTurnIndicator.Name = "redTurnIndicator";
            this.redTurnIndicator.Size = new System.Drawing.Size(36, 17);
            this.redTurnIndicator.TabIndex = 4;
            this.redTurnIndicator.Text = "---->";
            this.redTurnIndicator.Visible = false;
            // 
            // blueTurnIndicator
            // 
            this.blueTurnIndicator.AutoSize = true;
            this.blueTurnIndicator.Location = new System.Drawing.Point(34, 57);
            this.blueTurnIndicator.Name = "blueTurnIndicator";
            this.blueTurnIndicator.Size = new System.Drawing.Size(41, 17);
            this.blueTurnIndicator.TabIndex = 3;
            this.blueTurnIndicator.Text = "<-----";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(268, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Igrač B";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(6, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Igrač A";
            // 
            // scoreLabel
            // 
            this.scoreLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scoreLabel.Location = new System.Drawing.Point(81, 18);
            this.scoreLabel.Name = "scoreLabel";
            this.scoreLabel.Size = new System.Drawing.Size(181, 38);
            this.scoreLabel.TabIndex = 1;
            this.scoreLabel.Text = "0 : 0";
            this.scoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.canvas.Location = new System.Drawing.Point(12, 12);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(600, 600);
            this.canvas.TabIndex = 1;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.Canvas_Paint);
            this.canvas.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseClick);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseMove);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.aiMinimaxTree);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.aiMode);
            this.groupBox2.Controls.Add(this.aiTreeDepth);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.newGame);
            this.groupBox2.Controls.Add(this.loadGameState);
            this.groupBox2.Controls.Add(this.saveGameState);
            this.groupBox2.Controls.Add(this.aiDifficulty);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.pcVsPcRadio);
            this.groupBox2.Controls.Add(this.humanVsPcRadio);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.humanVsHumanRadio);
            this.groupBox2.Controls.Add(this.tableSizeY);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tableSizeX);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(618, 99);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 338);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Podešavanja igre";
            // 
            // aiMinimaxTree
            // 
            this.aiMinimaxTree.Enabled = false;
            this.aiMinimaxTree.Location = new System.Drawing.Point(183, 292);
            this.aiMinimaxTree.Name = "aiMinimaxTree";
            this.aiMinimaxTree.Size = new System.Drawing.Size(149, 31);
            this.aiMinimaxTree.TabIndex = 20;
            this.aiMinimaxTree.Text = "Stablo igre";
            this.aiMinimaxTree.UseVisualStyleBackColor = true;
            this.aiMinimaxTree.Click += new System.EventHandler(this.AiMinimaxTree_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 202);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 17);
            this.label11.TabIndex = 19;
            this.label11.Text = "AI režim:";
            // 
            // aiMode
            // 
            this.aiMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.aiMode.FormattingEnabled = true;
            this.aiMode.Items.AddRange(new object[] {
            "Korak po korak",
            "Krajnje stanje"});
            this.aiMode.Location = new System.Drawing.Point(116, 199);
            this.aiMode.Name = "aiMode";
            this.aiMode.Size = new System.Drawing.Size(216, 24);
            this.aiMode.TabIndex = 18;
            // 
            // aiTreeDepth
            // 
            this.aiTreeDepth.Location = new System.Drawing.Point(116, 171);
            this.aiTreeDepth.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.aiTreeDepth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.aiTreeDepth.Name = "aiTreeDepth";
            this.aiTreeDepth.Size = new System.Drawing.Size(216, 22);
            this.aiTreeDepth.TabIndex = 17;
            this.aiTreeDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.aiTreeDepth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 173);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 17);
            this.label10.TabIndex = 16;
            this.label10.Text = "Dubina stabla:";
            // 
            // newGame
            // 
            this.newGame.Location = new System.Drawing.Point(183, 255);
            this.newGame.Name = "newGame";
            this.newGame.Size = new System.Drawing.Size(149, 31);
            this.newGame.TabIndex = 15;
            this.newGame.Text = "Nova igra";
            this.newGame.UseVisualStyleBackColor = true;
            this.newGame.Click += new System.EventHandler(this.NewGame_Click);
            // 
            // loadGameState
            // 
            this.loadGameState.Location = new System.Drawing.Point(10, 292);
            this.loadGameState.Name = "loadGameState";
            this.loadGameState.Size = new System.Drawing.Size(149, 31);
            this.loadGameState.TabIndex = 14;
            this.loadGameState.Text = "Učitaj stanje igre...";
            this.loadGameState.UseVisualStyleBackColor = true;
            this.loadGameState.Click += new System.EventHandler(this.LoadGameState_Click);
            // 
            // saveGameState
            // 
            this.saveGameState.Location = new System.Drawing.Point(10, 255);
            this.saveGameState.Name = "saveGameState";
            this.saveGameState.Size = new System.Drawing.Size(149, 31);
            this.saveGameState.TabIndex = 13;
            this.saveGameState.Text = "Sačuvaj stanje igre...";
            this.saveGameState.UseVisualStyleBackColor = true;
            this.saveGameState.Click += new System.EventHandler(this.SaveGameState_Click);
            // 
            // aiDifficulty
            // 
            this.aiDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.aiDifficulty.FormattingEnabled = true;
            this.aiDifficulty.Items.AddRange(new object[] {
            "Početni",
            "Napredni",
            "Takmičarski"});
            this.aiDifficulty.Location = new System.Drawing.Point(116, 141);
            this.aiDifficulty.Name = "aiDifficulty";
            this.aiDifficulty.Size = new System.Drawing.Size(216, 24);
            this.aiDifficulty.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 144);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 17);
            this.label9.TabIndex = 11;
            this.label9.Text = "Nivo igrača:";
            // 
            // pcVsPcRadio
            // 
            this.pcVsPcRadio.AutoSize = true;
            this.pcVsPcRadio.Enabled = false;
            this.pcVsPcRadio.Location = new System.Drawing.Point(116, 114);
            this.pcVsPcRadio.Name = "pcVsPcRadio";
            this.pcVsPcRadio.Size = new System.Drawing.Size(145, 21);
            this.pcVsPcRadio.TabIndex = 10;
            this.pcVsPcRadio.Text = "Računar - računar";
            this.pcVsPcRadio.UseVisualStyleBackColor = true;
            this.pcVsPcRadio.CheckedChanged += new System.EventHandler(this.GUI_GameSettingChanged);
            // 
            // humanVsPcRadio
            // 
            this.humanVsPcRadio.AutoSize = true;
            this.humanVsPcRadio.Enabled = false;
            this.humanVsPcRadio.Location = new System.Drawing.Point(116, 87);
            this.humanVsPcRadio.Name = "humanVsPcRadio";
            this.humanVsPcRadio.Size = new System.Drawing.Size(130, 21);
            this.humanVsPcRadio.TabIndex = 9;
            this.humanVsPcRadio.Text = "Čovek - računar";
            this.humanVsPcRadio.UseVisualStyleBackColor = true;
            this.humanVsPcRadio.CheckedChanged += new System.EventHandler(this.GUI_GameSettingChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 17);
            this.label8.TabIndex = 8;
            this.label8.Text = "Tip igre:";
            // 
            // humanVsHumanRadio
            // 
            this.humanVsHumanRadio.AutoSize = true;
            this.humanVsHumanRadio.Checked = true;
            this.humanVsHumanRadio.Enabled = false;
            this.humanVsHumanRadio.Location = new System.Drawing.Point(116, 60);
            this.humanVsHumanRadio.Name = "humanVsHumanRadio";
            this.humanVsHumanRadio.Size = new System.Drawing.Size(118, 21);
            this.humanVsHumanRadio.TabIndex = 7;
            this.humanVsHumanRadio.TabStop = true;
            this.humanVsHumanRadio.Text = "Čovek - čovek";
            this.humanVsHumanRadio.UseVisualStyleBackColor = true;
            this.humanVsHumanRadio.CheckedChanged += new System.EventHandler(this.GUI_GameSettingChanged);
            // 
            // tableSizeY
            // 
            this.tableSizeY.Enabled = false;
            this.tableSizeY.Location = new System.Drawing.Point(237, 32);
            this.tableSizeY.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.tableSizeY.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.tableSizeY.Name = "tableSizeY";
            this.tableSizeY.Size = new System.Drawing.Size(95, 22);
            this.tableSizeY.TabIndex = 6;
            this.tableSizeY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tableSizeY.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(217, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "x";
            // 
            // tableSizeX
            // 
            this.tableSizeX.Enabled = false;
            this.tableSizeX.Location = new System.Drawing.Point(116, 32);
            this.tableSizeX.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.tableSizeX.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.tableSizeX.Name = "tableSizeX";
            this.tableSizeX.Size = new System.Drawing.Size(95, 22);
            this.tableSizeX.TabIndex = 4;
            this.tableSizeX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tableSizeX.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Veličina table:";
            // 
            // turnRichTextBox
            // 
            this.turnRichTextBox.Location = new System.Drawing.Point(6, 21);
            this.turnRichTextBox.Name = "turnRichTextBox";
            this.turnRichTextBox.Size = new System.Drawing.Size(332, 142);
            this.turnRichTextBox.TabIndex = 3;
            this.turnRichTextBox.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.turnRichTextBox);
            this.groupBox3.Location = new System.Drawing.Point(618, 443);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 169);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Odigrani potezi";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 618);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dots and Boxes";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aiTreeDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableSizeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableSizeX)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label scoreLabel;
        private System.Windows.Forms.Panel canvas;
        private System.Windows.Forms.Label redTurnIndicator;
        private System.Windows.Forms.Label blueTurnIndicator;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown tableSizeY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown tableSizeX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox turnRichTextBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox aiDifficulty;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton pcVsPcRadio;
        private System.Windows.Forms.RadioButton humanVsPcRadio;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton humanVsHumanRadio;
        private System.Windows.Forms.NumericUpDown aiTreeDepth;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button newGame;
        private System.Windows.Forms.Button loadGameState;
        private System.Windows.Forms.Button saveGameState;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox aiMode;
        private System.Windows.Forms.Button aiMinimaxTree;
    }
}

