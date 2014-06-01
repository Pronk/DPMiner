
using DataVault;
namespace DPMiner
{
    
    partial class FormMain
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.exitButton = new System.Windows.Forms.Button();
            this.hubButton = new System.Windows.Forms.Button();
            this.linkButton = new System.Windows.Forms.Button();
            this.sateliteButton = new System.Windows.Forms.Button();
            this.catButton = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multyEventSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataVault = new DataVault.SimpleEditor();
            this.candidates = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.dataVault.SuspendLayout();
            this.SuspendLayout();
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.Location = new System.Drawing.Point(554, 516);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "Выход";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // hubButton
            // 
            this.hubButton.Location = new System.Drawing.Point(14, 32);
            this.hubButton.Name = "hubButton";
            this.hubButton.Size = new System.Drawing.Size(39, 23);
            this.hubButton.TabIndex = 2;
            this.hubButton.Text = "H";
            // 
            // linkButton
            // 
            this.linkButton.Location = new System.Drawing.Point(59, 32);
            this.linkButton.Name = "linkButton";
            this.linkButton.Size = new System.Drawing.Size(35, 23);
            this.linkButton.TabIndex = 3;
            this.linkButton.Text = "L";
            this.linkButton.UseVisualStyleBackColor = true;
            // 
            // sateliteButton
            // 
            this.sateliteButton.Location = new System.Drawing.Point(100, 32);
            this.sateliteButton.Name = "sateliteButton";
            this.sateliteButton.Size = new System.Drawing.Size(37, 23);
            this.sateliteButton.TabIndex = 4;
            this.sateliteButton.Text = "S";
            this.sateliteButton.UseVisualStyleBackColor = true;
            // 
            // catButton
            // 
            this.catButton.Location = new System.Drawing.Point(143, 32);
            this.catButton.Name = "catButton";
            this.catButton.Size = new System.Drawing.Size(35, 23);
            this.catButton.TabIndex = 5;
            this.catButton.Text = "R";
            this.catButton.UseVisualStyleBackColor = true;
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNext.Location = new System.Drawing.Point(545, 32);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 6;
            this.buttonNext.Text = "=>";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(632, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionToolStripMenuItem,
            this.multyEventSolutionToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(56, 20);
            this.toolStripMenuItem1.Text = "Options";
            // 
            // connectionToolStripMenuItem
            // 
            this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            this.connectionToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.connectionToolStripMenuItem.Text = "Connection";
            this.connectionToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // multyEventSolutionToolStripMenuItem
            // 
            this.multyEventSolutionToolStripMenuItem.Name = "multyEventSolutionToolStripMenuItem";
            this.multyEventSolutionToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.multyEventSolutionToolStripMenuItem.Text = "Multy event solution";
            this.multyEventSolutionToolStripMenuItem.Click += new System.EventHandler(this.multyEventSolutionToolStripMenuItem_Click);
            // 
            // dataVault
            // 
            this.dataVault.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataVault.Controls.Add(this.candidates);
            this.dataVault.Location = new System.Drawing.Point(2, 61);
            this.dataVault.Name = "dataVault";
            this.dataVault.Size = new System.Drawing.Size(618, 449);
            this.dataVault.TabIndex = 0;
            // 
            // candidates
            // 
            this.candidates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.candidates.FormattingEnabled = true;
            this.candidates.Location = new System.Drawing.Point(3, 206);
            this.candidates.Name = "candidates";
            this.candidates.Size = new System.Drawing.Size(89, 238);
            this.candidates.TabIndex = 2;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 542);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.catButton);
            this.Controls.Add(this.sateliteButton);
            this.Controls.Add(this.linkButton);
            this.Controls.Add(this.hubButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.dataVault);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.dataVault.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SimpleEditor dataVault;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button hubButton;
        private System.Windows.Forms.Button linkButton;
        private System.Windows.Forms.Button sateliteButton;
        private System.Windows.Forms.Button catButton;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.ListBox candidates;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multyEventSolutionToolStripMenuItem;

       

    }
}

