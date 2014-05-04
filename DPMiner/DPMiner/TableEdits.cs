using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPMiner
{
    public abstract class TableEdits:IView
    {
        public void Refresh(){}
        Panel panel;
        IDataTable self=null;
        DataVaultConstructor parent;
        public TableEdits(IDataTable table, DataVaultConstructor parent)
        {
            self = table;
            panel = new Panel();
            this.parent = parent;
        }
        public TableEdits( DataVaultConstructor parent)
        {
            panel = new Panel();
            this.parent = parent;
        }
        public abstract Panel Edits(IDataTable table, DataVaultConstructor parent);
        public abstract Panel Edits( DataVaultConstructor parent);
        public  void Publish()
        {
            
            if (self == null)
                panel = Edits(parent);
            else
                panel = Edits(self, parent);
            try
            {
                parent.Controls["panelEditor"].Controls.Add(panel) ;
                
            }
            catch (KeyNotFoundException e) { }
        }
    }
    public class HubEdits : TableEdits 
    {
        public HubEdits(DataVaultConstructor parent) : base(parent) { }
        public HubEdits(IDataTable table, DataVaultConstructor parent) : base(table, parent) { }
        public override Panel Edits(IDataTable table, DataVaultConstructor parent)
        {
            Hub hub = table as Hub;
            Panel panel = new Panel();
            panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)| AnchorStyles.Left));
            panel.AutoScroll = false;
            panel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            panel.Location = new System.Drawing.Point(1, 30);
            panel.Name = "HubControls";
            panel.Size = new System.Drawing.Size(190, 175);
            panel.TabIndex = 0;
            Label label = new Label() ;
            DataField[] fields = hub.Content();
            label.Text = "Business Key";
            label.Name = "bKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            TextBox textBox = new TextBox();
            textBox.Text = fields[0].ToString();
            textBox.Name = "bKeyBox";
            textBox.Location = new System.Drawing.Point(80, 1);
            textBox.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(textBox);
            panel.Controls.Add(label);
            if(hub.Surrogate)
            {
                label = new Label();
                label.Text = "Surrogate Key";
                label.Name = "sKeyLabel";
                label.Location = new System.Drawing.Point(1, 30);
                textBox = new TextBox();
                textBox.Text = fields[1].ToString();
                textBox.Name = "sKeyBox";
                textBox.Location = new System.Drawing.Point(80, 1);
                textBox.Size = new System.Drawing.Size(100, 20);
                panel.Controls.Add(textBox);
                panel.Controls.Add(label);
            }
            else
            {
                label = new Label();
                label.Text = "Surrogate Key";
                label.Name = "sKeyLabel";
                label.Location = new System.Drawing.Point(1, 30);
                textBox = new TextBox();
                textBox.Text = " ";
                textBox.Name = "sKeyBox";
                textBox.Location = new System.Drawing.Point(80, 30);
                textBox.Size = new System.Drawing.Size(100, 20);
                panel.Controls.Add(textBox);
                panel.Controls.Add(label);
            }
            Button button = new Button();
            button.Name = "addButton";
            button.Text = "Add";
            button.Location = new System.Drawing.Point(1, 60);
            button.Size = new System.Drawing.Size(60, 20);
            panel.Controls.Add(button);
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "delete";
            button.Location = new System.Drawing.Point(80, 60);
            button.Size = new System.Drawing.Size(60, 20);
            panel.Controls.Add(button);
            return panel;

        }
        public override Panel Edits( DataVaultConstructor parent)
        {
            throw new MissingMethodException();
        }
    }
   /* public class LinkEdits : TableEdits
    {

    }
    public class SateliteEdits : TableEdits
    {

    }
    public class CategoryEdits : TableEdits
    {

    } */
}
