using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using Petri;

namespace DPMiner
{
    class WaitScreen:Panel,IMiningState
    {
        BackgroundWorker bw= new BackgroundWorker();
        LogBuilder lb;
        Label messageBoard;
        IPetriNet result;
        public Control Handle()
        {
            return this;
        }
        public bool IsEnd()
        {
            return true;
        }
        public bool HideControls()
        {
            return true;
        }
        public Stages Stage()
        {
            return Stages.mining;
        }
        public IPetriNet Result
        {
            get { return result; }
        }
        public WaitScreen(LogBuilder lb)
        {
            this.lb = lb;

            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.CenterImage;
            Controls.Add(pb);

            messageBoard = new Label();
            messageBoard.Text = "";
            messageBoard.Dock = DockStyle.Bottom;
            Controls.Add(messageBoard);

            bw.WorkerReportsProgress = true;
            bw.DoWork += DoWork;
            bw.ProgressChanged+=Report;
            bw.RunWorkerCompleted += Complete;
            
        }
        protected void DoWork(object sender, DoWorkEventArgs args )
        {
            LogBuilder lb = args.Argument as LogBuilder;
            BackgroundWorker bw = sender as BackgroundWorker;
            int n = 1;
            while(lb.DoWork())
            {
                bw.ReportProgress(n);
                Thread.Sleep(1);
                n++;
            }
            args.Result = lb.Log;
            args.Cancel = true;

        }
        protected void Report(object sender, ProgressChangedEventArgs e )
        {
            messageBoard.Text = e.ProgressPercentage.ToString() + " tuples checked";
        }
        protected void Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            int[][] log = e.Result as int[][];
            ProcessMiner miner = new AlphaMiner(lb.Alphabeth.Size);
            messageBoard.Text = "Process Mining";
            result = miner.Mine(log);
            Form1 parent = Parent as Form1;
            parent.Next();
        }
        public IMiningState Next()
        {
            if (result == null)
                throw new Exception();
            PetriView next = new PetriView(result, lb.Alphabeth);
            next.Location = Location;
            next.Size = Size;
            return next;

        }
    }
}
