using System;
using etf.dotsandboxes.cl160127d.AI.Minimax;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace etf.dotsandboxes.cl160127d
{
    public partial class MinimaxOverview : Form
    {
        private Minimax minimax;

        public MinimaxOverview()//Minimax minimax)
        {
            InitializeComponent();

            //this.minimax = minimax;

            PopulateTreeStructure();
        }

        private void PopulateTreeStructure()
        {

        }

        private void MinimaxTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }
    }
}