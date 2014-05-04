using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPMiner
{
    public interface IView
    {
        void Renew();
    }
    public class DataVaultFormView:IView
    {
        List<IDataTable> tables;
        Panel panel;
        DataVaultConstructor parent;
        public DataVaultFormView(List<IDataTable> tables, DataVaultConstructor parent)
        {
            this.parent = parent;
            panel = new Panel();
            this.tables = tables;
            panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            panel.AutoScroll = true;
            panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel.Location = new System.Drawing.Point(parent.Width - 173, -1);
            panel.Name = "panelVault";
            panel.Size = new System.Drawing.Size(158, 331);
            panel.TabIndex = 0;
            this.parent.Controls.Add(panel);
             
        }
        public Control Display()
        {
            return panel;
        }
        public void Renew()
        {
            
            int top = 20;
            panel.Controls.Clear();
            foreach(IDataTable table in tables)
            {
                
                Label tableLabel = new Label();
                tableLabel.Text = table.ToString();
                tableLabel.Name = table.ToString();
                tableLabel.Left = 20;
                tableLabel.Top = top;
                tableLabel.MouseHover += parent.PreviewTable;
                tableLabel.Click += parent.Edit;
                panel.Controls.Add(tableLabel);
                top += 30;
                
            }
            panel.Refresh();
        }
       
    }
    public class DataTableView : IView
    {
        
        IDataTable self;
        DataVaultConstructor parent;
        Panel panel;
        public DataTableView(IDataTable self, DataVaultConstructor parent)
        {
            this.self = self;
            this.parent = parent;
            this.parent.Controls.RemoveByKey("panelTable");
            panel = new Panel();
            panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            panel.AutoScroll = false;
            panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel.Location = new System.Drawing.Point(-1, 200);
            panel.Name = "panelTable";
            panel.Size = new System.Drawing.Size(parent.Width - 173, parent.Height - 200);
            panel.TabIndex = 0;
            this.parent.Controls.Add(panel);
            Label tableLabel = new Label();
            tableLabel.Text = self.ToString();
            tableLabel.Name = self.ToString();
            tableLabel.Left = 30;
            tableLabel.Top = 10;
            panel.Controls.Add(tableLabel);
            Renew();

        }
        public void Renew()
        {

            int top = 30;
            foreach ( DataField field in self.Content())
            {

                Label tableLabel = new Label();
                tableLabel.Text = field.ToString();
                tableLabel.Name = field.ToString();
                tableLabel.Left = 20;
                tableLabel.Top = top;
                panel.Controls.Add(tableLabel);
                top += 30;

            }
            panel.Refresh();
        }
        
    }
    public class TableEditor:IView
    {
        IDataTable self;
        DataVaultConstructor parent;
        Panel panel;
        public TableEditor(IDataTable self, DataVaultConstructor parent )
        {
            this.self = self;
            this.parent = parent;
            panel = new Panel();
            Build();
            

        }

        private void Build()
        {
            Type trueType = self.GetType();
            panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left) | AnchorStyles.Right));
            panel.AutoScroll = false;
            panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel.Location = new System.Drawing.Point(-1, -1);
            panel.Name = "panelEditor";
            panel.Size = new System.Drawing.Size(190, 200);
            panel.TabIndex = 0;
            Label label = new Label();
            label.Text = Program.Util.Typename(trueType) + " " + self.ToString();
            label.Name = ToString();
            label.Left = 10;
            label.Top = 10;
            label.Size = new System.Drawing.Size(160, 15);
            panel.Controls.Add(label);
            parent.Controls.RemoveByKey("panelEditor");
            this.parent.Controls.Add(panel);
        }
        public TableEditor(Type type, DataVaultConstructor parent)
        {
            this.parent = parent;
            panel = new Panel();
            panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left) | AnchorStyles.Right));
            panel.AutoScroll = false;
            panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            panel.Location = new System.Drawing.Point(-1, -1);
            panel.Name = "panelEditor";
            panel.Size = new System.Drawing.Size(parent.Width - 173, 200);
            panel.TabIndex = 0;
            Label label = new Label();
            label.Text = Program.Util.Typename(type) + ". Enter the name.";
            label.Name = ToString();
            label.Left = 10;
            label.Top = 10;
            label.Size = new System.Drawing.Size(160, 15);
            panel.Controls.Add(label);
            parent.Controls.RemoveByKey("panelEditor");
            this.parent.Controls.Add(panel);
        }

        public void Renew()
        {
            if(self!=null)
            {
                panel.Controls.Clear();
                Build();
            }
        }
    }
}
