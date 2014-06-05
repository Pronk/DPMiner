using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using DPMiner;

namespace Petri
{
    class PetriView:Panel,IMiningState
    {
        IPetriControl control;
        IVisualSet visuals;
        Panel canvus;
        Control immit;
        bool select = false;
        BackgroundWorker bw;
        public PetriView(IPetriNet model, Alphabeth language, Control parent )
        {
            control = model.GetControls();
            visuals = model.GetView(language);
            Parent = parent;
            canvus = new Panel();
            canvus.Dock = DockStyle.Fill;
            canvus.Size = new Size(10000, 10000);
            canvus.BackColor = Color.White;
            canvus.AutoScroll = true;
            canvus.VerticalScroll.Enabled = true;
            canvus.VerticalScroll.Visible = true;
            canvus.HorizontalScroll.Enabled = true;
            canvus.VerticalScroll.Enabled = true;
            canvus.Paint += Draw;
            canvus.MouseClick += Interact;
            Parent.KeyDown += Drop;
            Controls.Add(canvus);

            Panel cPanel = new Panel();
            cPanel.Dock = DockStyle.Right;
            cPanel.Width = 80;
            Controls.Add(cPanel);

            Button button = new Button();
            button.Text = "Immitate";
            button.Name = "buttonImmitate";
            button.Click += BeginImmitate;
            button.Height = 20;
            button.Dock = DockStyle.Top;
            cPanel.Controls.Add(button);
            immit = button;

            button = new Button();
            button.Text = "Reset";
            button.Name = "buttonReset";
            button.Height = 20;
            button.Click += (o, e) => { control.Reset(); visuals.Update();  canvus.Refresh(); };
            button.Dock = DockStyle.Top;
            cPanel.Controls.Add(button);

            button = new Button();
            button.Text = "Statistics";
            button.Name = "buttonStats";
            button.Height = 20;
            button.Dock = DockStyle.Top;
            cPanel.Controls.Add(button);
        }
        protected void Interact(object sender, MouseEventArgs e)
        {
            if(!select)
            {
                select = visuals.Select(e.Location);
                
            }
            else
            {
                if(e.Button == MouseButtons.Left)
                {
                    visuals.Move(e.Location);
                    select = false;
                    Refresh();
                }
                else
                {
                    Int32 n = visuals.Code();
                    if(n == -1 )
                        return;
                    if(control.TryFire(n))
                    {
                        visuals.Update();
                        Refresh();
                    }
                }
            }
        }
        public Stages Stage()
        {
            return Stages.model;
        }
        public bool HideControls()
        {
            return true;
        }
        public bool IsEnd()
        {
            return true;
        }
        public Control Handle()
        {
            return this;
        }
        public IMiningState Next()
        {
            return this;
        }
        protected void Drop(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                select = false;
        }
        protected void Draw(object sender, PaintEventArgs e)
        {
            visuals.Draw(e.Graphics);
        }
        protected void Immitate(object o, DoWorkEventArgs e)
        {
            BackgroundWorker bw = o as BackgroundWorker;
            IPetriControl controller = e.Argument as IPetriControl;
            while(controller.Act())
            {
                bw.ReportProgress(0);
                Thread.Sleep(2500);
                if (bw.CancellationPending)
                    break;
            }
            e.Cancel = true;

        }
        protected void Update(object o, ProgressChangedEventArgs e)
        {
            visuals.Update();
            Refresh();
        }
        protected void BeginImmitate(object o, EventArgs e )
        {
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += Immitate;
            bw.RunWorkerCompleted += EndImmitate;
            bw.ProgressChanged += Update;

            Button clicked = o as Button;
            clicked.Click -= BeginImmitate;
            clicked.Click += EndImmitate;
            clicked.Text = "Stop";

            canvus.MouseClick -= Interact;

            bw.RunWorkerAsync(control);
            
        }

        protected void EndImmitate(object o, EventArgs e)
        {
            bw.CancelAsync();

           
            immit.Click += BeginImmitate;
            immit.Click -= EndImmitate;
            immit.Text = "Immitate";

            canvus.MouseClick += Interact;
        }
    }
}
