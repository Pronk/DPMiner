﻿namespace DPMiner
{
    partial class Form1
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
            this.dataVault = new DPMiner.SimpleEditor();
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
            this.hubButton.UseVisualStyleBackColor = true;
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
            this.catButton.Text = "C";
            this.catButton.UseVisualStyleBackColor = true;
            // 
            // dataVault
            // 
            this.dataVault.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataVault.Location = new System.Drawing.Point(11, 61);
            this.dataVault.Name = "dataVault";
            this.dataVault.Size = new System.Drawing.Size(618, 449);
            this.dataVault.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 542);
            this.Controls.Add(this.catButton);
            this.Controls.Add(this.sateliteButton);
            this.Controls.Add(this.linkButton);
            this.Controls.Add(this.hubButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.dataVault);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SimpleEditor dataVault;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button hubButton;
        private System.Windows.Forms.Button linkButton;
        private System.Windows.Forms.Button sateliteButton;
        private System.Windows.Forms.Button catButton;

       

    }
}

