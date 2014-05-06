using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Monad;

namespace DPMiner
{
    public abstract class TableControls:Panel
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
        public TableControls(IDataTable table, DataVaultConstructor parent):base()
        {
            self = table;
           
            this.parent = parent;
        }
        public TableControls( DataVaultConstructor parent):base()
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
    public class HubControls : TableControls
    {
        TextBox name;
        TextBox sur;
        TextBox bis;
        public HubControls(DataVaultConstructor parent) : base(parent) { }
        public HubControls(IDataTable table, DataVaultConstructor parent) : base(table, parent) { }
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
            Maybe<string> newName = TableControls.TrueText(name);
            Maybe<string> newBKey = TableControls.TrueText(bis);
            Maybe<string> newSKey = TableControls.TrueText(sur);
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
            Maybe<string> newName = TableControls.TrueText(name);
            Maybe<string> newBKey = TableControls.TrueText(bis);
            Maybe<string> newSKey = TableControls.TrueText(sur);
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
    public class LinkControls : TableControls
    {
        int n=1;
        TextBox sur;
        TextBox name;
        List<TextBox> hubKeys =new List<TextBox>();
        List<Control> movable = new List<Control>();
        public LinkControls(DataVaultConstructor parent) : base(parent) { }
        public LinkControls(IDataTable table, DataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, DataVaultConstructor parent)
        {
            n = 1;
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
                label.Location = new System.Drawing.Point(1, 30*(k + 1));
                TextBox hub = new TextBox();
                hub.Text = fields[k].ToString();
                hub.Name = "bKeyBox";
                hub.Location = new System.Drawing.Point(80, 30*(k+1));
                hub.Size = new System.Drawing.Size(100, 30);
                Controls.Add(label);
                Controls.Add(hub);
                hubKeys.Add(hub);
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
            n = 1;
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
            TableControls.TrueText(name).SideEffect(s=>{newName = s;},()=>{fail = true;});
            TableControls.TrueText(sur).SideEffect(s => { newKey = s; }, () => { fail = true; });
            foreach(TextBox hubKey in hubKeys)
            {
                string key = "";
                TableControls.TrueText(sur).SideEffect(s => { key = s; }, () => { missed = true; });
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
            TableControls.TrueText(name).SideEffect(s => { newName = s; }, () => { fail = true; });
            TableControls.TrueText(sur).SideEffect(s => { newKey = s; }, () => { fail = true; });
            foreach (TextBox hubKey in hubKeys)
            {
                string key = "";
                TableControls.TrueText(sur).SideEffect(s => { key = s; }, () => { missed = true; });
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
            Refresh();
        }
    }
    public class SateliteControls : TableControls
    {
        int n = 2;
        int m = 3;
        TextBox sur;
        TextBox name;
        TextBox link;
        List<TextBox> measures = new List<TextBox>();
        List<TextBox> refKeys = new List<TextBox>();
        List<Control> movable = new List<Control>();
        List<Control> movable2 = new List<Control>();
        public SateliteControls(DataVaultConstructor parent) : base(parent) { }
        public SateliteControls(IDataTable table, DataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, DataVaultConstructor parent)
        {
            n = 2;
            m = 3;
            Controls.Clear();
            movable.Clear();
            movable2.Clear();
            refKeys.Clear();
            measures.Clear();
            Satelite link = table as Satelite;
            DataField[] fields = link.Content();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "satControls";
            this.Size = new Size(300, 175);
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
            sur = new TextBox();
            label = new Label();
            label.Text = "Link Key";
            label.Name = "kinkLabel";
            label.Location = new System.Drawing.Point(1, 60);
            sur = new TextBox();
            sur.Name = "fKeyBox";
            sur.Text = fields[1].ToString();
            sur.Location = new System.Drawing.Point(80, 30);
            sur.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(sur);
            this.Controls.Add(label);
            foreach (int k in Enumerable.Range(2, link.Count() + 2))
            {
                label = new Label();
                label.Text = "measure" + (k - 1).ToString();
                label.Name = "=meLabel";
                label.Location = new System.Drawing.Point(1, 30*(k+1));
                TextBox mes = new TextBox();
                mes.Text = fields[k].ToString();
                mes.Name = "measureBox";
                mes.Location = new System.Drawing.Point(80, 30 * (k + 1));
                mes.Size = new System.Drawing.Size(100, 30 );
                Controls.Add(label);
                Controls.Add(mes);
                measures.Add(mes);
                n = k;
            }
            m = n;
            foreach (int k in Enumerable.Range(n, fields.Count() - 1))
            {
                label = new Label();
                label.Text = "reference" + (k - n + 1).ToString();
                label.Name = "=meLabel";
                label.Location = new System.Drawing.Point(1, 30 * (k + 1));
                TextBox referance = new TextBox();
                referance.Text = fields[k].ToString();
                referance.Name = "refBox";
                referance.Location = new System.Drawing.Point(80, 30 * (k + 1));
                referance.Size = new System.Drawing.Size(100, 30);
                Controls.Add(label);
                Controls.Add(referance);
                refKeys.Add(referance);
                movable2.Add(referance);
                movable2.Add(label);
                m = k;
            }
            Button button = new Button();
            button.Name = "UpdateButton";
            button.Text = "Update";
            button.Location = new System.Drawing.Point(1, 30 * (m + 2));
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += UpdateSatelite;
            this.Controls.Add(button);
            movable.Add(button);
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "Delete";
            button.Click += parent.Delete;
            button.Click += OnDelete;
            button.Location = new System.Drawing.Point(80, 30 * (m + 2));
            button.Size = new System.Drawing.Size(60, 20);
            this.Controls.Add(button);
            movable.Add(button);
            button = new Button();
            button.Name = "newRefButton";
            button.Text = "Add ref";
            button.Click += AddRef;
            button.Location = new System.Drawing.Point(160, 30 * (m + 2));
            button.Size = new System.Drawing.Size(60, 20);
            movable.Add(button);
            this.Controls.Add(button);
            button.Name = "newRefButton";
            button.Text = "Add field";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(240, 30 * (m + 2));
            button.Size = new System.Drawing.Size(60, 20);
            movable.Add(button);
            this.Controls.Add(button);
        }
        public override void Edits(DataVaultConstructor parent)
        {
            n = 2;
            m = 3;
            Controls.Clear();
            movable.Clear();
            movable2.Clear();
            refKeys.Clear();
            measures.Clear();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "satControls";
            this.Size = new Size(300, 175);
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
            sur.Text = "";
            sur.Location = new System.Drawing.Point(80, 30);
            sur.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(sur);
            this.Controls.Add(label);
            sur = new TextBox();
            label = new Label();
            label.Text = "Link Key";
            label.Name = "kinkLabel";
            label.Location = new System.Drawing.Point(1, 60);
            sur = new TextBox();
            sur.Name = "fKeyBox";
            sur.Text = "";
            sur.Location = new System.Drawing.Point(80, 30);
            sur.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(sur);
            this.Controls.Add(label);
            label = new Label();
            label.Text = "measure1";
            label.Name = "=meLabel";
            label.Location = new System.Drawing.Point(1, 90);
            TextBox mes = new TextBox();
            mes.Text ="";
            mes.Name = "measureBox";
            mes.Location = new System.Drawing.Point(80, 90);
            mes.Size = new System.Drawing.Size(100, 30);
            Controls.Add(label);
            Controls.Add(mes);
            measures.Add(mes);
            label = new Label();
            label.Text = "reference" + (1).ToString();
            label.Name = "=meLabel";
            label.Location = new System.Drawing.Point(1, 120);
            TextBox referance = new TextBox();
            referance.Text = "";
            referance.Name = "measureBox";
            referance.Location = new System.Drawing.Point(80, 120);
            referance.Size = new System.Drawing.Size(100, 30);
            Controls.Add(label);
            Controls.Add(referance);
            refKeys.Add(referance);
            movable2.Add(referance);
            movable2.Add(label);
            Button button = new Button();
            button.Name = "DDButton";
            button.Text = "Add";
            button.Location = new System.Drawing.Point(1, 150);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += AddSatelite;
            this.Controls.Add(button);
            movable.Add(button);
            button = new Button();
            button.Name = "newRefButton";
            button.Text = "Add ref";
            button.Click += AddRef;
            button.Location = new System.Drawing.Point(160, 150);
            button.Size = new System.Drawing.Size(60, 20);
            movable.Add(button);
            this.Controls.Add(button);
            button.Name = "newRefButton";
            button.Text = "Add field";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(240, 150);
            button.Size = new System.Drawing.Size(60, 20);
            movable.Add(button);
            this.Controls.Add(button);
        }
        protected void Shift(List<Control> movable )
        {
            foreach (Control e in movable)
                e.Top += 30;
        }
        protected void UpdateSatelite(Object sender, EventArgs args)
        {

            string newName = null;
            string newKey = null;
            string fKey = null;
            Link newLink = null;
            List<DataField> newFields = new List<DataField>();
            List<Reference> newRefs = new List<Reference>();
            try
            {
                TableControls.TrueText(name).SideEffect(s => newName = s, () => { throw new System.ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).SideEffect(s => newKey = s, () => { throw new System.ArgumentException("Table must have a key!"); });
                foreach (TextBox mes in measures)
                    TableControls.TrueText(mes).SideEffect(s => newFields.Add(new DataField(s)), () => {});
                foreach (TextBox refKey in refKeys)
                {
                    try
                    {
                        TableControls.TrueText(refKey).SideEffect(s => fKey = s, () => { throw new System.ArgumentException(); });
                    }
                    catch (ArgumentException) { continue; }
                    parent.GetTable(fKey, TableType.Reference).SideEffect(t => newRefs.Add(t as Reference), () => { throw new ArgumentException("Referance table with key " + fKey + " not found!"); });
                }
                TableControls.TrueText(link).SideEffect(s => fKey = s, () => { throw new System.ArgumentException(); });
                parent.GetTable(fKey, TableType.Link).SideEffect(t => newLink = t as Link, () => { throw new ArgumentException("Link with key " + fKey + " not found!"); });
                Satelite satelite = Table.Load() as Satelite;
                satelite.Name = newName;
                satelite.Key = new DataField(newKey);
                satelite.Link = newLink;
                satelite.Measures = newFields;
                parent.UpdateEditor();
                parent.Refresh();
            }
            catch (ArgumentException e) { MessageBox.Show(e.Message); }
        }
        protected void AddRef(Object sender, EventArgs args)
        {
            m++;
            Label label = new Label();
            label.Text = "reference" + (m - n + 1).ToString();
            label.Name = "=meLabel";
            label.Location = new System.Drawing.Point(1, 30 * (m + 1));
            TextBox referance = new TextBox();
            referance.Text = "";
            referance.Name = "refBox";
            referance.Location = new System.Drawing.Point(80, 30 * (m + 1));
            referance.Size = new System.Drawing.Size(100, 30);
            Controls.Add(label);
            Controls.Add(referance);
            refKeys.Add(referance);
            movable2.Add(referance);
            movable2.Add(label);
            Shift(movable);
            Refresh();
        }
        protected void AddField(Object sender, EventArgs args)
        {
            n++;
            Label label = new Label();
            label.Text = "reference" + (n  + 1).ToString();
            label.Name = "=meLabel";
            label.Location = new System.Drawing.Point(1, 30 * (n + 1));
            TextBox field = new TextBox();
            field.Text = "";
            field.Name = "fieldBox";
            field.Location = new System.Drawing.Point(80, 30 * (n + 1));
            field.Size = new System.Drawing.Size(100, 30);
            Controls.Add(label);
            Controls.Add(field);
            measures.Add(field);
            Shift(movable2);
            Refresh();
        }
        protected void AddSatelite(Object sender, EventArgs args)
        {

            string newName = null;
            string newKey = null;
            string fKey = null;
            Link newLink = null;
            List<string> newFields = new List<string>();
            List<Reference> newRefs = new List<Reference>();
            try
            {
                TableControls.TrueText(name).SideEffect(s => newName = s, () => { throw new System.ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).SideEffect(s => newKey = s, () => { throw new System.ArgumentException("Table must have a key!"); });
                foreach (TextBox mes in measures)
                    TableControls.TrueText(mes).SideEffect(s => newFields.Add(s), () => { });
                foreach (TextBox refKey in refKeys)
                {
                    try
                    {
                        TableControls.TrueText(refKey).SideEffect(s => fKey = s, () => { throw new System.ArgumentException(); });
                    }
                    catch (ArgumentException) { continue; }
                    parent.GetTable(fKey, TableType.Reference).SideEffect(t => newRefs.Add(t as Reference), () => { throw new ArgumentException("Referance table with key " + fKey + " not found!"); });
                }
                TableControls.TrueText(link).SideEffect(s => fKey = s, () => { throw new System.ArgumentException(); });
                parent.GetTable(fKey, TableType.Link).SideEffect(t => newLink = t as Link, () => { throw new ArgumentException("Link with key " + fKey + " not found!"); });
                Satelite satelite = new Satelite(newName, newLink, newKey, "source", newFields, newRefs, Maybe<string>.None());
                parent.AddTable(satelite);
                Table = satelite;
                Publish();
                Refresh();
                
            }
            catch (ArgumentException e) { MessageBox.Show(e.Message); }
        }
    }
    public class ReferenceControls : TableControls
    {
        int n = 1;
        TextBox name = new TextBox();
        TextBox sur = new TextBox();
        List<TextBox> measures = new List<TextBox>();
        List<Control> movable = new List<Control>();
        public ReferenceControls(DataVaultConstructor parent) : base(parent) { }
        public ReferenceControls(IDataTable table, DataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, DataVaultConstructor parent)
        {
            n = 1;
            Controls.Clear();
            movable.Clear();
            measures.Clear();
            Reference reference = table as Reference;
            DataField[] fields = reference.Content();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            this.Size = new Size(300, 250);
            this.TabIndex = 0;
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            name = new TextBox();
            name.Text = reference.Name;
            name.Name = "bKeyBox";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(name);
            this.Controls.Add(label);
            sur = new TextBox();
            label = new Label();
            label.Text = "Key";
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
                label.Location = new System.Drawing.Point(1, 30 * (k + 1));
                TextBox mes = new TextBox();
                mes.Text = fields[k].ToString();
                mes.Name = "bKeyBox";
                mes.Location = new System.Drawing.Point(80, 30 * (k + 1));
                mes.Size = new System.Drawing.Size(100, 30);
                Controls.Add(label);
                Controls.Add(mes);
                measures.Add(mes);
                n = k;
            }
            Button button = new Button();
            button.Name = "UpdateButton";
            button.Text = "Update";
            button.Location = new System.Drawing.Point(1, 30 * (n + 2));
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += UpdateRef;
            this.Controls.Add(button);
            movable.Add(button);
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "Delete";
            button.Click += parent.Delete;
            button.Click += OnDelete;
            button.Location = new System.Drawing.Point(80, 30 * (n + 2));
            button.Size = new System.Drawing.Size(60, 20);
            this.Controls.Add(button);
            movable.Add(button);
            button = new Button();
            button.Name = "newHubButton";
            button.Text = "Add";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(160, 30 * (n + 2));
            button.Size = new System.Drawing.Size(60, 20);
            movable.Add(button);
            this.Controls.Add(button);
        }
        public override void Edits(DataVaultConstructor parent)
        {
            n = 1;
            Controls.Clear();
            movable.Clear();
            measures.Clear();
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
            button.Click += AddRef;
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
        protected void UpdateRef(object srnder, EventArgs args)
        {
            string newName = null;
            string newKey = null;
            List<DataField> newFields = new List<DataField>();
            try
            {
                TableControls.TrueText(name).SideEffect(s => newName = s, () => { throw new ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).SideEffect(s => newKey = s, () => { throw new ArgumentException("Table must have a Key!"); });
                foreach (TextBox field in measures)
                    TableControls.TrueText(field).SideEffect(s => newFields.Add(new DataField(s)), () => { });
                if (newFields.Count == 0)
                    throw new ArgumentException("Table must have at least one meaningful field!");
                Reference reference = Table.Load() as Reference;
                reference.Name = newName;
                reference.Key = new DataField(newKey);
                reference.Fields = newFields;
                parent.UpdateEditor();
                parent.Refresh();
            }
            catch (ArgumentException e) { MessageBox.Show(e.Message); }
        }
        protected void AddRef(object sender, EventArgs args)
        {
            string newName = null;
            string newKey = null;
            List<string> newFields = new List<string>();
            try
            {
                TableControls.TrueText(name).SideEffect(s => newName = s, () => { throw new ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).SideEffect(s => newKey = s, () => { throw new ArgumentException("Table must have a Key!"); });
                foreach (TextBox field in measures)
                    TableControls.TrueText(field).SideEffect(s => newFields.Add(s), () => { });
                if (newFields.Count == 0)
                    throw new ArgumentException("Table must have at least one meaningful field!");
                Reference reference = new Reference(newName, newFields, newKey);
                Table = reference;
                Publish();
                parent.AddTable(reference);
            }
            catch (ArgumentException e) { MessageBox.Show(e.Message); }
        }
        protected void Shift()
        {
            foreach (Control control in movable)
                control.Top += 30;
        }
        protected void AddField(object sender,EventArgs args)
        {
            n++;
            Label label = new Label();
            label.Text = "Hub" + n.ToString();
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            TextBox field = new TextBox();
            field.Text = "";
            field.Name = "bKeyBox";
            field.Location = new System.Drawing.Point(80, 30 * (n + 1));
            field.Size = new System.Drawing.Size(100, 30 * (n + 1));
            Controls.Add(label);
            Controls.Add(field);
            Shift();
            Refresh();
        }
    } 
}
