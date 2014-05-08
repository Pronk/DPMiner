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
    public abstract class TableControls : Panel
    {
        Panel fieldsPanel = new Panel();
        public void Clear()
        {
            fieldsPanel.Controls.Clear();
        }
        bool fp = false;
        public static Maybe<string> TrueText(TextBox carier)
        {
            if (carier.Text.Trim() == "")
                return Maybe<string>.None();
            return carier.Text;
        }
        protected Panel FieldPanel()
        {
            if(!fp)
            {
               fieldsPanel.Dock = DockStyle.Fill;
               fieldsPanel.AutoScroll = true;
               fieldsPanel.Height = Height - 35;
               fp = true;
               Controls.Add(fieldsPanel);
            }
            Panel panel = new Panel();
            panel.Dock = DockStyle.Top;
            panel.AutoScroll = false;
            panel.BorderStyle = BorderStyle.None;
            panel.Height = 30;
            Control[] zIndexDestroyer = new Control[fieldsPanel.Controls.Count + 1];
            fieldsPanel.Controls.CopyTo(zIndexDestroyer, 0);
            fieldsPanel.Controls.Add(panel);
            foreach (Control c in zIndexDestroyer)
            {
                fieldsPanel.Controls.Remove(c);
                fieldsPanel.Controls.Add(c);
            }
            return panel;
        }
        protected Panel ControlPanel()
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Bottom;
            panel.AutoScroll = false;
            panel.BorderStyle = BorderStyle.None;
            panel.Height = 30;
            Controls.Add(panel);
            return panel;
        }
        public void Renew()
        {
            Controls.Clear();
            Edits(parent);
            Publish();
            parent.Refresh();
        }
        IDataTable self = null;
        protected DataVaultConstructor parent;
        public Maybe<IDataTable> Table
        {
            get { return Maybe<IDataTable>.Something(self); }
            protected set { value.Do(t => self = t, () => self = null); }
        }
        public TableControls(IDataTable table, DataVaultConstructor parent)
            : base()
        {
            self = table;

            this.parent = parent;
        }
        public TableControls(DataVaultConstructor parent)
            : base()
        {

            this.parent = parent;
        }
        public abstract void Edits(IDataTable table, DataVaultConstructor parent);
        public abstract void Edits(DataVaultConstructor parent);
        public void Publish()
        {
            fp =false;
            if (self == null)
                Edits(parent);
            else
                Edits(self, parent);
            try
            {

                parent.Controls["panelEditor"].Controls.Add(this);
               
            }
            catch (KeyNotFoundException e) { }
        }
        protected void OnDelete(object sender, EventArgs args)
        {
            this.Controls.Clear();
            TableType type = self.Type();
            Table = Maybe<IDataTable>.None();
            parent.NewEditor(type);
            Edits(parent);
            Publish();
        }
        public override void Refresh()
        {
           
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
            this.Size = new System.Drawing.Size(300, 175);
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
            try
            {
                string newName = "";
                string newBKey = "";
                string newSKey = "";
                TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Hub must have a name!"); });
                TrueText(bis).Do(s => newName = s, () => { throw new ArgumentException("Hub must have a business keu!"); });
                TrueText(sur).Do(s => newName = s);
                Hub hub = Table.Load() as Hub;
                hub.PhisicalID = new DataField(newBKey);
                hub.Name = newName;
                if (newSKey != "")
                    hub.SurogateID = new DataField(newSKey);
                else
                    hub.PhisicalID = null;
                parent.Refresh();
                parent.UpdateEditor();
                this.Edits(hub, parent);
                this.Publish();
            }
            catch (ArgumentException e) { MessageBox.Show(e.Message); }
        }

        private void AddHub(object sender, EventArgs args)
        {
            try
            {
                string newName = "";
                string newBKey = "";
                Maybe<string> newSKey = TrueText(sur);
                TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Hub must have a name!"); });
                TrueText(bis).Do(s => newBKey = s, () => { throw new ArgumentException("Hub must have a business key!"); });
                Hub hub = new Hub(newName, newBKey, "source", newSKey, Maybe<string>.None());
                parent.AddTable(hub);
                Table = hub;
                this.Publish();
            }
            catch (ArgumentException e) { MessageBox.Show(e.Message); }
        }
    }
    public class LinkControls : TableControls
    {
        int n = 1;
        TextBox sur;
        TextBox name;
        List<TextBox> hubKeys = new List<TextBox>();
        public LinkControls(DataVaultConstructor parent) : base(parent) { }
        public LinkControls(IDataTable table, DataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, DataVaultConstructor parent)
        {
            n = 1;
            Clear();
            Controls.Clear();
            hubKeys.Clear();
            Link link = table as Link;
            DataField[] fields = link.Content();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            this.Size = new Size(420, 175);
            this.TabIndex = 0;
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(60, 20);
            name = new TextBox();
            name.Text = link.Name;
            name.Name = "bKeyBox";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(name);
            panel.Controls.Add(label);
            panel = FieldPanel();
            sur = new TextBox();
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            sur = new TextBox();
            sur.Name = "sKeyBox";
            sur.Text = fields[0].ToString();
            sur.Location = new System.Drawing.Point(80, 1);
            sur.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            foreach (int k in Enumerable.Range(1, fields.Length - 2))
            {
                panel = FieldPanel();
                label = new Label();
                label.Text = "Hub" + k.ToString();
                label.Name = "nameLabel";
                label.Location = new System.Drawing.Point(1, 1);
                label.Size = new System.Drawing.Size(60, 20);
                TextBox hub = new TextBox();
                hub.Text = fields[k].ToString();
                hub.Name = "bKeyBox";
                hub.Location = new System.Drawing.Point(80, 1);
                hub.Size = new System.Drawing.Size(100, 30);
                panel.Controls.Add(label);
                panel.Controls.Add(hub);
                hubKeys.Add(hub);
                n = k;
            }
            panel = ControlPanel();
            Button button = new Button();
            button.Name = "UpdateButton";
            button.Text = "Update";
            button.Location = new System.Drawing.Point(1, 1);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += UpdateLink;
            panel.Controls.Add(button);
          
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "Delete";
            button.Click += parent.Delete;
            button.Click += OnDelete;
            button.Location = new System.Drawing.Point(80, 1);
            button.Size = new System.Drawing.Size(60, 20);
            panel.Controls.Add(button);
          
            button = new Button();
            button.Name = "newHubButton";
            button.Text = "Add";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(160, 1);
            button.Size = new System.Drawing.Size(60, 20);
            panel.Controls.Add(button);
        }
        public override void Edits(DataVaultConstructor parent)
        {
            n = 1;
            Clear();
            Controls.Clear();
            hubKeys.Clear();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            this.Size = new Size(430, 175);
            this.TabIndex = 0;
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            name = new TextBox();
            name.Text = "";
            name.Name = "name";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(name);
            panel.Controls.Add(label);
            panel = FieldPanel();
            sur = new TextBox();
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            sur = new TextBox();
            sur.Name = "sKeyBox";
            sur.Text = "";
            sur.Location = new System.Drawing.Point(80, 1);
            sur.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            panel = FieldPanel();
            label = new Label();
            label.Text = "Hub1";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(60, 20);
            TextBox hub = new TextBox();
            hub.Text = "";
            hub.Name = "hKeyBox";
            hub.Location = new System.Drawing.Point(80, 1);
            hub.Size = sur.Size;
            panel.Controls.Add(label);
            panel.Controls.Add(hub);
            panel = ControlPanel();
            hubKeys.Add(hub);
            Button button = new Button();
            button.Name = "addButton";
            button.Text = "Add";
            button.Location = new System.Drawing.Point(1, 1);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += AddLink;
            panel.Controls.Add(button);
          
            button = new Button();
            button.Name = "newHubButton";
            button.Text = "Add Hub";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(80,1);
            button.Size = new System.Drawing.Size(60, 20);
           panel.Controls.Add(button);
        }
        private void UpdateLink(object sender, EventArgs e)
        {
            string newName = "";
            Link link = Table.Load() as Link;
            string newKey = "";
            bool missed = false;
            bool fail = false;
            List<Hub> joint = new List<Hub>();
            TableControls.TrueText(name).Do(s => { newName = s; }, () => { fail = true; });
            TableControls.TrueText(sur).Do(s => { newKey = s; }, () => { fail = true; });
            foreach (TextBox hubKey in hubKeys)
            {
                string key = "";
                TableControls.TrueText(hubKey).Do(s => { key = s; }, () => { missed = true; });
                if (missed)
                {
                    missed = false;
                    continue;
                }
                parent.GetTable(key, TableType.Hub).Do(t => { joint.Add(t as Hub); }, () => { fail = true; });
                if (fail)
                    return;
            }
            if (joint.Count < 1)
            {
                MessageBox.Show("Нужно связать как минмум 2 хаба");
                return;
            }
            link.Name = newName;
            link.Joint = joint;
            link.Key = new DataField(newKey);
            parent.Refresh();
            parent.UpdateEditor();
        }
        private void AddField(object sender, EventArgs e)
        {
            n++;
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = "Hub" + n.ToString();
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(60, 20);
            TextBox hub = new TextBox();
            hub.Text = "";
            hub.Name = "bKeyBox";
            hub.Location = new System.Drawing.Point(80, 1);
            hub.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(label);
            panel.Controls.Add(hub);
            Refresh();
        }
        private void AddLink(object sender, EventArgs e)
        {
            string newName = "";
            string newKey = "";
            bool missed = false;
            bool fail = false;
            List<Hub> joint = new List<Hub>();
            TableControls.TrueText(name).Do(s => { newName = s; }, () => { fail = true; });
            TableControls.TrueText(sur).Do(s => { newKey = s; }, () => { fail = true; });
            foreach (TextBox hubKey in hubKeys)
            {
                string key = "";
                TableControls.TrueText(hubKey).Do(s => { key = s; }, () => { missed = true; });
                if (missed)
                {
                    missed = false;
                    continue;
                }
                parent.GetTable(key, TableType.Hub).Do(t => { joint.Add(t as Hub); }, () => { fail = true; });
                if (fail)
                    return;
            }
            if (joint.Count < 1)
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
        public SateliteControls(DataVaultConstructor parent) : base(parent) { }
        public SateliteControls(IDataTable table, DataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, DataVaultConstructor parent)
        {
            n = 2;
            m = 3;
            Controls.Clear();
            refKeys.Clear();
            measures.Clear();
            Clear();
            Satelite sat = table as Satelite;
            DataField[] fields = sat.Content();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "satControls";
            this.Size = new Size(420, 175);
            this.TabIndex = 0;
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            name = new TextBox();
            name.Text = sat.Name;
            name.Name = "bKeyBox";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(name);
            panel.Controls.Add(label);
            panel = FieldPanel();
            sur = new TextBox();
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            sur = new TextBox();
            sur.Name = "sKeyBox";
            sur.Text = fields[0].ToString();
            sur.Location = new System.Drawing.Point(80, 1);
            sur.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            panel = FieldPanel();
            link = new TextBox();
            label = new Label();
            label.Text = "Link Key";
            label.Name = "kinkLabel";
            label.Location = new System.Drawing.Point(1, 1);
            link.Name = "fKeyBox";
            link.Text = fields[1].ToString();
            link.Location = new System.Drawing.Point(80, 1);
            link.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(link);
            panel.Controls.Add(label);
            foreach (int k in Enumerable.Range(2, sat.Count() ))
            {
                panel = FieldPanel();
                label = new Label();
                label.Text = "measure" + (k - 1).ToString();
                label.Name = "=meLabel";
                label.Location = new System.Drawing.Point(1, 1);
                label.Size = new Size(60, 20);
                TextBox mes = new TextBox();
                try
                {
                    mes.Text = fields[k].ToString();
                }
                catch (IndexOutOfRangeException) { }
                mes.Name = "measureBox";
                mes.Location = new System.Drawing.Point(80,1);
                mes.Size = new System.Drawing.Size(100, 30);
                panel.Controls.Add(label);
                panel.Controls.Add(mes);
                measures.Add(mes);
                n = k;
            }
            m = n;
            foreach (int k in Enumerable.Range(n, fields.Count() - sat.Count()-3))
            {
                panel = FieldPanel();
                label = new Label();
                label.Text = "reference" + (k - n + 1).ToString();
                label.Name = "=meLabel";
                label.Location = new System.Drawing.Point(1, 1);
                label.Size = new Size(60, 20);
                TextBox referance = new TextBox();
                try{
                referance.Text = fields[k].ToString();
                }
                catch (IndexOutOfRangeException) { }
                referance.Name = "refBox";
                referance.Location = new System.Drawing.Point(80, 1);
                referance.Size = new System.Drawing.Size(100, 30);
                panel.Controls.Add(label);
                panel.Controls.Add(referance);
                refKeys.Add(referance);
                m = k;
            }
            panel = ControlPanel();
            Button button = new Button();
            button.Name = "UpdateButton";
            button.Text = "Update";
            button.Location = new System.Drawing.Point(1, 1);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += UpdateSatelite;
            panel.Controls.Add(button);
       
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "Delete";
            button.Click += parent.Delete;
            button.Click += OnDelete;
            button.Location = new System.Drawing.Point(80, 1);
            button.Size = new System.Drawing.Size(60, 20);
            panel.Controls.Add(button);
        
            button = new Button();
            button.Name = "newRefButton";
            button.Text = "Add ref";
            button.Click += AddRef;
            button.Location = new System.Drawing.Point(160, 1);
            button.Size = new System.Drawing.Size(60, 20);
            panel.Controls.Add(button);
            button = new Button();
            button.Name = "newRefButton";
            button.Text = "Add field";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(240, 1);
            button.Size = new System.Drawing.Size(60, 20);
            panel.Controls.Add(button);
        }
        public override void Edits(DataVaultConstructor parent)
        {
            n = 2;
            m = 3;
            Controls.Clear();
            measures.Clear();
            Clear();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "satControls";
            this.Size = new Size(420, 175);
            this.TabIndex = 0;
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            name = new TextBox();
            name.Text = "";
            name.Name = "bKeyBox";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(name);
            panel.Controls.Add(label);
            panel = FieldPanel();
            sur = new TextBox();
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            sur = new TextBox();
            sur.Name = "sKeyBox";
            sur.Text = "";
            sur.Location = new System.Drawing.Point(80, 1);
            sur.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            panel = FieldPanel();
            link = new TextBox();
            label = new Label();
            label.Text = "Link Key";
            label.Name = "kinkLabel";
            label.Location = new System.Drawing.Point(1, 1);
            link = new TextBox();
            link.Name = "fKeyBox";
            link.Text = "";
            link.Location = new System.Drawing.Point(80, 1);
            link.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(link);
            panel.Controls.Add(label);
            panel = FieldPanel();
            label = new Label();
            label.Text = "measure1";
            label.Name = "meLabel";
            label.Size = new Size(60, 20);
            label.Location = new System.Drawing.Point(1, 1);
            TextBox mes = new TextBox();
            mes.Text = "";
            mes.Name = "measureBox";
            mes.Location = new System.Drawing.Point(80, 1);
            mes.Size = new System.Drawing.Size(100, 30);
            panel.Controls.Add(label);
            panel.Controls.Add(mes);
            measures.Add(mes);
            panel = FieldPanel();
            label = new Label();
            label.Text = "reference" + (1).ToString();
            label.Name = "=meLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(60, 20);
            TextBox referance = new TextBox();
            referance.Text = "";
            referance.Name = "measureBox";
            referance.Location = new System.Drawing.Point(80, 1);
            referance.Size = new System.Drawing.Size(100, 30);
            panel.Controls.Add(label);
            panel.Controls.Add(referance);
            refKeys.Add(referance);
            panel = ControlPanel();
            Button button = new Button();
            button.Name = "DDButton";
            button.Text = "Add";
            button.Location = new System.Drawing.Point(1, 1);
            button.Size = new System.Drawing.Size(80, 20);
            button.Click += AddSatelite;
            panel.Controls.Add(button);
            button = new Button();
            button.Name = "newRefButton";
            button.Text = "Add ref";
            button.Click += AddRef;
            button.Location = new System.Drawing.Point(80, 1);
            button.Size = new System.Drawing.Size(80, 20);
            panel.Controls.Add(button);
            button = new Button();
            button.Name = "newRefButton";
            button.Text = "Add field";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(160, 1);
            button.Size = new System.Drawing.Size(80, 20);
            panel.Controls.Add(button);
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
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new System.ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new System.ArgumentException("Table must have a key!"); });
                foreach (TextBox mes in measures)
                    TableControls.TrueText(mes).Do(s => newFields.Add(new DataField(s)), () => { });
                foreach (TextBox refKey in refKeys)
                {
                    try
                    {
                        TableControls.TrueText(refKey).Do(s => fKey = s, () => { throw new System.ArgumentException(); });
                    }
                    catch (ArgumentException) { continue; }
                    parent.GetTable(fKey, TableType.Reference).Do(t => newRefs.Add(t as Reference), () => { throw new ArgumentException("Referance table with key " + fKey + " not found!"); });
                }
                TableControls.TrueText(link).Do(s => fKey = s, () => { throw new System.ArgumentException(); });
                parent.GetTable(fKey, TableType.Link).Do(t => newLink = t as Link, () => { throw new ArgumentException("Link with key " + fKey + " not found!"); });
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
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = "reference" + (m - 2).ToString();
            label.Name = "=meLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(60, 20);
            TextBox referance = new TextBox();
            referance.Text = "";
            referance.Name = "refBox";
            referance.Location = new System.Drawing.Point(80, 1);
            referance.Size = new System.Drawing.Size(100, 30);
            panel.Controls.Add(label);
            panel.Controls.Add(referance);
            refKeys.Add(referance);
            Refresh();
        }
        protected void AddField(Object sender, EventArgs args)
        {
            n++;
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = "measure" + (n - 1).ToString();
            label.Name = "=meLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(60, 20);
            TextBox field = new TextBox();
            field.Text = "";
            field.Name = "fieldBox";
            field.Location = new System.Drawing.Point(80, 1);
            field.Size = new System.Drawing.Size(100, 30);
            panel.Controls.Add(label);
            panel.Controls.Add(field);
            measures.Add(field);
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
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new System.ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new System.ArgumentException("Table must have a key!"); });
                foreach (TextBox mes in measures)
                    TableControls.TrueText(mes).Do(s => newFields.Add(s), () => { });
                foreach (TextBox refKey in refKeys)
                {
                    try
                    {
                        TableControls.TrueText(refKey).Do(s => fKey = s, () => { throw new System.ArgumentException(); });
                    }
                    catch (ArgumentException) { continue; }
                    parent.GetTable(fKey, TableType.Reference).Do(t => newRefs.Add(t as Reference), () => { throw new ArgumentException("Referance table with key " + fKey + " not found!"); });
                }
                TableControls.TrueText(link).Do(s => fKey = s, () => { throw new System.ArgumentException(); });
                parent.GetTable(fKey, TableType.Link).Do(t => newLink = t as Link, () => { throw new ArgumentException("Link with key " + fKey + " not found!"); });
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
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new ArgumentException("Table must have a Key!"); });
                foreach (TextBox field in measures)
                    TableControls.TrueText(field).Do(s => newFields.Add(new DataField(s)), () => { });
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
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new ArgumentException("Table must have a Key!"); });
                foreach (TextBox field in measures)
                    TableControls.TrueText(field).Do(s => newFields.Add(s), () => { });
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
        protected void AddField(object sender, EventArgs args)
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
