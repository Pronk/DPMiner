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
  
    public partial class FormMain : Form
    {
        IMiningState current ;
        Control[] outerControl;
        public FormMain()
       {
           InitializeComponent();
            hubButton.Click += dataVault.NewHub;
            linkButton.Click += dataVault.NewLink;
            sateliteButton.Click += dataVault.NewSatelite;
            catButton.Click += dataVault.NewReference;
            dataVault.setCandidates(candidates);
            current = dataVault;
            outerControl = new Control[]{hubButton,
            linkButton,
            sateliteButton,
            catButton};
            dataVault.NewHub(dataVault,new EventArgs());
       }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            try
            {
                IMiningState next = current.Next();
                current.Dispose();
                current = next;
                Controls.Add(current.Handle());
                buttonNext.Visible = !current.IsEnd();
                foreach (Control outer in outerControl)
                    outer.Visible = !current.HideControls();
                Refresh();
            }
            catch (ArgumentException x) { MessageBox.Show(x.Message); }
        }
        public void Next()
        {
            buttonNext_Click(this, new EventArgs());
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form ConSetup = new FormForConnection();
            ConSetup.Show();
        }

        private void multyEventSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form selector = new MESolutionSelector();
            selector.Show();
        }
       

       
        
    }
}
