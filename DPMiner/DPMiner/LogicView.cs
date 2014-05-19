using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Threading;


namespace DPMiner
{
   
    public class LogicView : Panel, IMiningState
    {
        ILogicControl control;
        Label messageBoard;
        BackgroundWorker messanger;
        public LogicView(ProcessLogic logic)
        {
            RichTextBox rTB = new RichTextBox();
            rTB.Name = "logicTextBox";
            rTB.Dock = DockStyle.Fill;
            rTB.Lines = logic.ProcessEvents.Select<EventLogic, string>(e => e.ToString()).ToArray();
            Controls.Add(rTB);

            Panel controlPanel = new Panel();
            controlPanel.Name = "ControlPanel";
            controlPanel.Height = 30;
            controlPanel.Dock = DockStyle.Bottom;

            Button compileButton = new Button();
            compileButton.Size = new Size(80,30);
            compileButton.Location=new Point(1,1);
            compileButton.Name = "CompileButton";
            compileButton.Text = "Compile";

            Button restoreButton = new Button();
            restoreButton.Size = new Size(80,30);
            restoreButton.Location=new Point(80,1);
            restoreButton.Name = "restoreButton";
            restoreButton.Text = "Restore";

            messageBoard = new Label();
            messageBoard.Text = "";
            messageBoard.Size = new Size(200,30);
            messageBoard.Location = new Point(165,1);
            controlPanel.Controls.AddRange(new Control[] {restoreButton,compileButton,messageBoard});
            Controls.Add(controlPanel);

            control = new TextToLogicParser(rTB, logic);
            messanger = new BackgroundWorker();
            messanger.DoWork += ShowAndHide( 2500);
            messanger.WorkerReportsProgress = true;
            messanger.ProgressChanged += Print();
            ListBox processList = new ListBox();

            processList.Name = "ProcessList";
            processList.Text = "Process variables";
            processList.Width = 80;
            processList.Dock = DockStyle.Right;
            processList.DataSource = logic.ProcessValues;
            processList.ValueMember = processList.DisplayMember;
            Controls.Add(processList);

            processList.Click += control.AddSelected();
            compileButton.Click += control.Compile(InvokeMessage("Logic compiled well!"),InvokeNumberedMessage("Compilation failed on line"));
            restoreButton.Click+=control.Return();
            
        }
        public IMiningState Next()
        {
            return this;
        }
        public new Control Handle()
        {
            return this;
        }
        private DoWorkEventHandler ShowAndHide(int time)
        {
            return (sender, e) =>
                {
                    string message = e.Argument as string;
                    messanger.ReportProgress(0, message);
                    Thread.Sleep(time);
                    messanger.ReportProgress(0, "");
                    e.Cancel = true;
                };
        }
        public bool IsEnd()
        { return false; }
        public bool HideControls()
        { return true; }
        public Stages Stage()
        {
            return Stages.logic;
        }
        private ProgressChangedEventHandler Print()
        {
            return (sender, e) =>
                {
                    messageBoard.Text = e.UserState as string;
                };
        }
        private Action InvokeMessage( string message)
        {

            return () => { Clean(); messanger.RunWorkerAsync(message); };
        }
        private Action<int> InvokeNumberedMessage(string message)
        {

            return (n) => { Clean(); messanger.RunWorkerAsync(message + " " + n.ToString()); };
        }
        private void Clean()
        {
            if(messanger.IsBusy)
            {
                messanger.Dispose();
                messanger = new BackgroundWorker();
                messanger.DoWork += ShowAndHide(2500);
                messanger.WorkerReportsProgress = true;
                messanger.ProgressChanged += Print();
            }
        }
    }
}
