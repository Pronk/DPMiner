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
    public abstract class DataVaultConstructor : Form 
    {
        public abstract void PreviewTable(object sender, EventArgs e);
        public abstract void Edit(object sender, EventArgs e);
    }
    public partial class Form1 : DataVaultConstructor 
    {
        DataVaultFormView view;
        IDataVaultControl control;
        public Form1()
        {
            IDataVault model = new DataVault("test");
            view = model.View(this);
            control = model.Control();
            InitializeComponent();
       }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = null;
            control.Add(new Hub("table", "a", "b", s, s));
            view.Refresh();
        }

        public override void PreviewTable(object sender, EventArgs e)
        {
            Label table = sender as Label;
            control.GetTable(table.Text).SideEffect(t => t.Preview(this));
            this.Refresh();
        }
        public override void Edit(object sender, EventArgs e)
        {
            Label table = sender as Label;
            control.GetTable(table.Text).SideEffect(t => t.Editor(this));
            this.Refresh();
        }

    }
}
