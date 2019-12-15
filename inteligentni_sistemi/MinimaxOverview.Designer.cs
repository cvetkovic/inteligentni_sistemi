namespace etf.dotsandboxes.cl160127d
{
    partial class MinimaxOverview
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
            this.minimaxTree = new System.Windows.Forms.TreeView();
            this.canvas = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // minimaxTree
            // 
            this.minimaxTree.Location = new System.Drawing.Point(12, 12);
            this.minimaxTree.Name = "minimaxTree";
            this.minimaxTree.Size = new System.Drawing.Size(207, 600);
            this.minimaxTree.TabIndex = 0;
            this.minimaxTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MinimaxTree_AfterSelect);
            // 
            // canvas
            // 
            this.canvas.Location = new System.Drawing.Point(225, 12);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(600, 600);
            this.canvas.TabIndex = 1;
            // 
            // MinimaxOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 627);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.minimaxTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MinimaxOverview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Prikaz minimax algoritma protivnika";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView minimaxTree;
        private System.Windows.Forms.Panel canvas;
    }
}