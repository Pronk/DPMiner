using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPMiner
{
    public partial class MESolutionSelector : Form
    {
        public MESolutionSelector()
        {
            InitializeComponent();
            switch (Global.Solution)
            {
                case MultyEventSolution.drop:
                    radioDrop.Checked = true;
                    break;
                case MultyEventSolution.first:
                    radioFirst.Checked = true;
                    break;
                case MultyEventSolution.priority:
                    radioPriority.Checked = true;
                    break;
                case MultyEventSolution.shuffle:
                    radioShuffle.Checked = true;
                    break;
                case MultyEventSolution.parallel:
                    radioParallel.Checked = true;
                    break;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioDrop_Click(object sender, EventArgs e)
        {
            Global.Solution = MultyEventSolution.drop;
        }

        private void radioFirst_Click(object sender, EventArgs e)
        {
            Global.Solution = MultyEventSolution.first;
        }

        private void radioPriority_Click(object sender, EventArgs e)
        {
            Global.Solution = MultyEventSolution.priority;
        }

        private void radioShuffle_Click(object sender, EventArgs e)
        {
            Global.Solution = MultyEventSolution.shuffle;
        }

        private void radioParallel_Click(object sender, EventArgs e)
        {
            Global.Solution = MultyEventSolution.parallel;
        }

        

        
    }
}
