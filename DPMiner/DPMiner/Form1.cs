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
    using DataVaultSetup = Dictionary<FieldProperty, Dictionary<string, List<DataField>>>;
    public partial class Form1 : Form
    {
        IMiningState current ;
        Control[] outerControl;
        public Form1()
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

       

       
        
    }
}
