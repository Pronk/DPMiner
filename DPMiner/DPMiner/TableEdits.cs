﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Monad;

namespace DPMiner
{
    public abstract class TableEdits:Panel,IView
    {
        public static Maybe<string> TrueText(TextBox carier)
        {
            if (carier.Text.Trim() == "")
                return Maybe<string>.None();
            return carier.Text;
        }
        public void Renew()
        {
            Controls.Clear();
            Edits(parent);
            Publish();
            parent.Refresh();
        }
        IDataTable self=null;
        protected DataVaultConstructor parent;
        public Maybe<IDataTable> Table
        {
            get { return new Maybe<IDataTable>(self); }
            protected set { if (value is None<IDataTable>) self = null; else self = value.Load(); }
        }
        public TableEdits(IDataTable table, DataVaultConstructor parent):base()
        {
            self = table;
           
            this.parent = parent;
        }
        public TableEdits( DataVaultConstructor parent):base()
        {

            this.parent = parent;
        }
        public abstract void Edits(IDataTable table, DataVaultConstructor parent);
        public abstract void Edits( DataVaultConstructor parent);
        public  void Publish()
        {
            
            if (self == null)
                Edits(parent);
            else
                Edits(self, parent);
            try
            {
                parent.Controls["panelEditor"].Controls.Add(this) ;
                
            }
            catch (KeyNotFoundException e) { }
        }
        protected  void OnDelete(object sender, EventArgs args)
        {
            this.Controls.Clear();
            TableType type = self.Type();
            Table = Maybe<IDataTable>.None();
            parent.NewEditor(type);
            Edits(parent);
            Publish();
        }
    }
    public class HubEdits : TableEdits
    {
        TextBox name;
        TextBox sur;
        TextBox bis;
        public HubEdits(DataVaultConstructor parent) : base(parent) { }
        public HubEdits(IDataTable table, DataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, DataVaultConstructor parent)
        {
            Controls.Clear();
            Hub hub = table as Hub;
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new System.Drawing.Point(1, 30);
            this.Name = "HubControls";
            this.Size = new System.Drawing.Size(190, 175);
            this.TabIndex = 0;
            Label label = new Label();
            DataField[] fields = hub.Content();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            name = new TextBox();
            name.Text = hub.Name;
            name.Name = "bKeyBox";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(name);
            this.Controls.Add(label);
            label = new Label();
            label.Text = "Business Key";
            label.Name = "bKeyLabel";
            label.Location = new System.Drawing.Point(1, 30);
            bis = new TextBox();
            bis.Text = fields[0].ToString();
            bis.Name = "bKeyBox";
            bis.Location = new System.Drawing.Point(80, 30);
            bis.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(bis);
            this.Controls.Add(label);
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 60);
            sur = new TextBox();
            if (hub.Surrogate)
                sur.Text = fields[1].ToString();
            else
                sur.Text = " ";
            sur.Name = "sKeyBox";
            sur.Location = new System.Drawing.Point(80, 60);
            sur.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(sur);
            this.Controls.Add(label);
            Button button = new Button();
            button.Name = "UpdateButton";
            button.Text = "Update";
            button.Location = new System.Drawing.Point(1, 90);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += UpdateHub;
            this.Controls.Add(button);
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "Delete";
            button.Click += parent.Delete;
            button.Click += OnDelete;
            button.Location = new System.Drawing.Point(80, 90);
            button.Size = new System.Drawing.Size(60, 20);
            this.Controls.Add(button);


        }
        public override void Edits(DataVaultConstructor parent)
        {
            Controls.Clear();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new System.Drawing.Point(1, 30);
            this.Name = "HubControls";
            this.Size = new System.Drawing.Size(190, 175);
            this.TabIndex = 0;
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            name = new TextBox();
            name.Text = "";
            name.Name = "bKeyBox";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(name);
            this.Controls.Add(label);
            label = new Label();
            label.Text = "Business Key";
            label.Name = "bKeyLabel";
            label.Location = new System.Drawing.Point(1, 30);
            bis = new TextBox();
            bis.Text = "";
            bis.Name = "bKeyBox";
            bis.Location = new System.Drawing.Point(80, 30);
            bis.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(bis);
            this.Controls.Add(label);
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 60);
            sur = new TextBox();
            sur.Text = " ";
            sur.Name = "sKeyBox";
            sur.Location = new System.Drawing.Point(80, 60);
            sur.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(sur);
            this.Controls.Add(label);
            Button button = new Button();
            button.Name = "AddButton";
            button.Text = "Add";
            button.Location = new System.Drawing.Point(1, 90);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += AddHub;
            this.Controls.Add(button);
        }
        private void UpdateHub(object sender, EventArgs args)
        {
            Maybe<string> newName = TableEdits.TrueText(name);
            Maybe<string> newBKey = TableEdits.TrueText(bis);
            Maybe<string> newSKey = TableEdits.TrueText(sur);
            if (newBKey is None<string> || newName is None<string>)
            {
                MessageBox.Show("Бизнес ключ и имя должны быть введены!");
                return;
            }
            else
            {
                Hub hub = Table.Load() as Hub;
                hub.PhisicalID = new DataField(newBKey);
                hub.Name = newName;
                if (!(newSKey is None<string>))
                    hub.SurogateID = new DataField(newSKey);
                else
                    hub.PhisicalID = null;
                parent.Refresh();
                parent.UpdateEditor();
                this.Edits(hub, parent);
                this.Publish();
            }

        }
        private void AddHub(object sender, EventArgs args)
        {
            Maybe<string> newName = TableEdits.TrueText(name);
            Maybe<string> newBKey = TableEdits.TrueText(bis);
            Maybe<string> newSKey = TableEdits.TrueText(sur);
            if (newBKey is None<string> || newName is None<string>)
            {
                MessageBox.Show("Бизнес ключ и имя должны быть введены!");
                return;
            }
            else
            {
                Hub hub = new Hub(newName, newBKey, "source", newSKey, Maybe<string>.None());
                parent.AddTable(hub);
                Table = hub;
                this.Edits(hub, parent);
                this.Publish();

            }

        }
    }
    public class LinkEdits : TableEdits
    {
        int n=0;
        TextBox sur;
        TextBox name;
        List<TextBox> hubKeys =new List<TextBox>();
        List<Control> movable = new List<Control>();
        public override void Edits(IDataTable table, DataVaultConstructor parent)
        {
           
            Controls.Clear();
            movable.Clear();
            hubKeys.Clear();
            Link link = table as Link;
            DataField[] fields = link.Content();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            this.Size = new Size(190, 175);
            this.TabIndex = 0;
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            name = new TextBox();
            name.Text = link.Name;
            name.Name = "bKeyBox";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(name);
            this.Controls.Add(label);   
            sur = new TextBox();
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 30);
            sur = new TextBox();
            sur.Name = "sKeyBox";
            sur.Text = fields[0].ToString();
            sur.Location = new System.Drawing.Point(80, 30);
            sur.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(sur);
            this.Controls.Add(label);
            foreach (int k in Enumerable.Range(1, fields.Length - 2))
            {
                label = new Label();
                label.Text = "Hub" + k.ToString();
                label.Name = "nameLabel";
                label.Location = new System.Drawing.Point(1, 1);
                TextBox hub = new TextBox();
                hub.Text = fields[k].ToString();
                hub.Name = "bKeyBox";
                hub.Location = new System.Drawing.Point(80, 30*(k+1));
                hub.Size = new System.Drawing.Size(100, 30*(k+1));
                Controls.Add(label);
                Controls.Add(hub);
                n = k;
            }
            Button button = new Button();
            button.Name = "UpdateButton";
            button.Text = "Update";
            button.Location = new System.Drawing.Point(1, 30*(n+2));
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += UpdateLink;
            this.Controls.Add(button);
            movable.Add(button);
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "Delete";
            button.Click += parent.Delete;
            button.Click += OnDelete;
            button.Location = new System.Drawing.Point(80, 30*(n+2));
            button.Size = new System.Drawing.Size(60, 20);
            this.Controls.Add(button);
            movable.Add(button);
            button = new Button();
            button.Name = "newHubButton";
            button.Text = "Add";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(160, 30*(n+2));
            button.Size = new System.Drawing.Size(60, 20);
            movable.Add(button);
            this.Controls.Add(button);
        }
        public override void Edits( DataVaultConstructor parent)
        {
            Controls.Clear();
            movable.Clear();
            hubKeys.Clear();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            this.Size = new Size(190, 175);
            this.TabIndex = 0;
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            name = new TextBox();
            name.Text = "";
            name.Name = "name";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(name);
            this.Controls.Add(label); 
            sur = new TextBox();
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 30);
            sur = new TextBox();
            sur.Name = "sKeyBox";
            sur.Text = "";
            sur.Location = new System.Drawing.Point(80, 30);
            sur.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(sur);
            this.Controls.Add(label);
            label = new Label();
            label.Text = "Hub1";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            TextBox hub = new TextBox();
            hub.Text = "";
            hub.Name = "hKeyBox";
            hub.Location = new System.Drawing.Point(80, 60);
            hub.Size = new System.Drawing.Size(100, 60);
            Controls.Add(label);
            Controls.Add(hub);
            Button button = new Button();
            button.Name = "addButton";
            button.Text = "Add";
            button.Location = new System.Drawing.Point(1, 90);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += AddLink;
            this.Controls.Add(button);
            movable.Add(button);
            button = new Button();
            button.Name = "newHubButton";
            button.Text = "Add";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(80, 90);
            button.Size = new System.Drawing.Size(60, 20);
            movable.Add(button);
            this.Controls.Add(button);
        }
        private void Shift()
        {

            foreach (Control control in movable)
                control.Location = new Point(control.Left, control.Top + 30);

        }
        private void UpdateLink(object sender, EventArgs e)
        {
            string newName = "";
            Link link = Table.Load() as Link;
            string newKey ="";
            bool missed=false;
            bool fail = false;
            List<Hub> joint = new List<Hub>();
            TableEdits.TrueText(name).SideEffect(s=>{newName = s;},()=>{fail = true;});
            TableEdits.TrueText(sur).SideEffect(s => { newKey = s; }, () => { fail = true; });
            foreach(TextBox hubKey in hubKeys)
            {
                string key = "";
                TableEdits.TrueText(sur).SideEffect(s => { key = s; }, () => { missed = true; });
                if(missed)
                {
                    missed = false;
                    continue;
                }
                parent.GetTable(key, TableType.Hub).SideEffect(t => { joint.Add(t as Hub); }, () => { fail = true; });
                if (fail)
                    return;
            }
            if(joint.Count < 2)
            {
                MessageBox.Show("Нужно связать как минмум 2 хаба");
                return;
            }
            link.Name = newName;
            link.Joint = joint;
            link.Key = new DataField(newKey );
            parent.Refresh();
            parent.UpdateEditor();
        }
        private void AddField(object sender, EventArgs e)
        {
            n++;
            Label label = new Label();
            label.Text = "Hub" + n.ToString();
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            TextBox hub = new TextBox();
            hub.Text = "";
            hub.Name = "bKeyBox";
            hub.Location = new System.Drawing.Point(80, 30 * (n + 1));
            hub.Size = new System.Drawing.Size(100, 30 * (n + 1));
            Controls.Add(label);
            Controls.Add(hub);
            Shift();
            Refresh();
        }
        private void AddLink(object sender, EventArgs e)
        {
            string newName = "";
            string newKey = "";
            bool missed = false;
            bool fail = false;
            List<Hub> joint = new List<Hub>();
            TableEdits.TrueText(name).SideEffect(s => { newName = s; }, () => { fail = true; });
            TableEdits.TrueText(sur).SideEffect(s => { newKey = s; }, () => { fail = true; });
            foreach (TextBox hubKey in hubKeys)
            {
                string key = "";
                TableEdits.TrueText(sur).SideEffect(s => { key = s; }, () => { missed = true; });
                if (missed)
                {
                    missed = false;
                    continue;
                }
                parent.GetTable(key, TableType.Hub).SideEffect(t => { joint.Add(t as Hub); }, () => { fail = true; });
                if (fail)
                    return;
            }
            if (joint.Count < 2)
            {
                MessageBox.Show("Нужно связать как минмум 2 хаба");
                return;
            }
            Link link = new Link(newName, joint, "source", newKey, Maybe<string>.None());
            parent.AddTable(link);
            Table = link;
            Publish();
        }
    }
    public class SateliteEdits : TableEdits
    {

    }
    public class CategoryEdits : TableEdits
    {

    } 
}
