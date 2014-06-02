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
    public partial class FormForConnection : Form
    {
        public FormForConnection()
        {
            InitializeComponent();
            if (Connection.Offline)
                checkOffline.Checked = true;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (Connection.Offline = checkOffline.Checked)
                this.Close();
            Connection.DataSource = textBoxServer.Text;
            Connection.DataBase = textBoxDB.Text;
            Connection.Username = textBoxUN.Text;
            Connection.Password = textBoxPass.Text;
            this.Close();
        }

        private void checkOffline_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPass.Enabled = !checkOffline.Checked;
            textBoxUN.Enabled = !checkOffline.Checked;
            textBoxDB.Enabled = !checkOffline.Checked;
            textBoxServer.Enabled = !checkOffline.Checked;
        }
    }
}
