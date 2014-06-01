namespace DPMiner
{
    partial class MESolutionSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOk = new System.Windows.Forms.Button();
            this.radioDrop = new System.Windows.Forms.RadioButton();
            this.radioFirst = new System.Windows.Forms.RadioButton();
            this.radioPriority = new System.Windows.Forms.RadioButton();
            this.radioShuffle = new System.Windows.Forms.RadioButton();
            this.radioParallel = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(12, 131);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(89, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // radioDrop
            // 
            this.radioDrop.AutoSize = true;
            this.radioDrop.Location = new System.Drawing.Point(12, 12);
            this.radioDrop.Name = "radioDrop";
            this.radioDrop.Size = new System.Drawing.Size(87, 17);
            this.radioDrop.TabIndex = 2;
            this.radioDrop.TabStop = true;
            this.radioDrop.Text = "Drop solution";
            this.radioDrop.UseVisualStyleBackColor = true;
            this.radioDrop.Click += new System.EventHandler(this.radioDrop_Click);
            // 
            // radioFirst
            // 
            this.radioFirst.AutoSize = true;
            this.radioFirst.Location = new System.Drawing.Point(12, 36);
            this.radioFirst.Name = "radioFirst";
            this.radioFirst.Size = new System.Drawing.Size(83, 17);
            this.radioFirst.TabIndex = 3;
            this.radioFirst.TabStop = true;
            this.radioFirst.Text = "First solution";
            this.radioFirst.UseVisualStyleBackColor = true;
            this.radioFirst.Click += new System.EventHandler(this.radioFirst_Click);
            // 
            // radioPriority
            // 
            this.radioPriority.AutoSize = true;
            this.radioPriority.Location = new System.Drawing.Point(12, 60);
            this.radioPriority.Name = "radioPriority";
            this.radioPriority.Size = new System.Drawing.Size(95, 17);
            this.radioPriority.TabIndex = 4;
            this.radioPriority.TabStop = true;
            this.radioPriority.Text = "Priority solution";
            this.radioPriority.UseVisualStyleBackColor = true;
            this.radioPriority.Click += new System.EventHandler(this.radioPriority_Click);
            // 
            // radioShuffle
            // 
            this.radioShuffle.AutoSize = true;
            this.radioShuffle.Location = new System.Drawing.Point(12, 84);
            this.radioShuffle.Name = "radioShuffle";
            this.radioShuffle.Size = new System.Drawing.Size(97, 17);
            this.radioShuffle.TabIndex = 5;
            this.radioShuffle.TabStop = true;
            this.radioShuffle.Text = "Shuffle solution";
            this.radioShuffle.UseVisualStyleBackColor = true;
            this.radioShuffle.Click += new System.EventHandler(this.radioShuffle_Click);
            // 
            // radioParallel
            // 
            this.radioParallel.AutoSize = true;
            this.radioParallel.Location = new System.Drawing.Point(12, 108);
            this.radioParallel.Name = "radioParallel";
            this.radioParallel.Size = new System.Drawing.Size(98, 17);
            this.radioParallel.TabIndex = 6;
            this.radioParallel.TabStop = true;
            this.radioParallel.Text = "Parallel solution";
            this.radioParallel.UseVisualStyleBackColor = true;
            this.radioParallel.Click += new System.EventHandler(this.radioParallel_Click);
            // 
            // MESolutionSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(105, 159);
            this.Controls.Add(this.radioParallel);
            this.Controls.Add(this.radioShuffle);
            this.Controls.Add(this.radioPriority);
            this.Controls.Add(this.radioFirst);
            this.Controls.Add(this.radioDrop);
            this.Controls.Add(this.buttonOk);
            this.Name = "MESolutionSelector";
            this.Text = "Solutiom";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.RadioButton radioDrop;
        private System.Windows.Forms.RadioButton radioFirst;
        private System.Windows.Forms.RadioButton radioPriority;
        private System.Windows.Forms.RadioButton radioShuffle;
        private System.Windows.Forms.RadioButton radioParallel;
    }
}