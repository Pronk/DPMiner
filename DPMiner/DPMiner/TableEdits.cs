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
    using Fkey = Tuple<DataField, IDataTable>;
    using FkeyList = Tuple<List<string>, List<IDataTable>>;
    public class FKEditor:TextBox
    {
          IDataTable table = null;
        public Maybe<IDataTable> getTable()
        {
            if (table == null)
                return Maybe<IDataTable>.None();
            return Maybe<IDataTable>.Something(table);
        }
        public void setTable(IDataTable table)
        {
            this.table = table;
        }
        static public  Maybe<Fkey> Unload (FKEditor editor)
        {
            string text=null;
            IDataTable table=null;
            try{
                 TableControls.TrueText(editor).Do(s=>text=s,()=>{throw new Exception();});
                 editor.getTable().Do(t=>table=t,()=>{throw new Exception();});
                }catch(Exception){return Maybe<Fkey>.None(); }
            return  Maybe<Fkey>.Something(new Fkey(new DataField(text),table));
        }
        static public FkeyList UnloadList (IEnumerable<FKEditor> editors)
        {
            FkeyList list = new FkeyList(new List<string>(), new List<IDataTable>());
            IDataTable table = null;
            string name = null;
            foreach(FKEditor editor in editors)
            {
             try{
                 TableControls.TrueText(editor).Do(s=>name=s,()=>{throw new Exception();});
                 editor.getTable().Do(t=>table=t,()=>{throw new Exception();});
                 list.Item1.Add(name);
                 list.Item2.Add(table);
                }catch(Exception){continue;}
            }
            return  list;
        }
    }
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
        protected IDataVaultConstructor parent;
        public Maybe<IDataTable> Table
        {
            get { return Maybe<IDataTable>.Something(self); }
            protected set { value.Do(t => self = t, () => self = null); }
        }
        public TableControls(IDataTable table, IDataVaultConstructor parent)
            : base()
        {
            self = table;

            this.parent = parent;
        }
        public TableControls(IDataVaultConstructor parent)
            : base()
        {

            this.parent = parent;
        }
        public abstract void Edits(IDataTable table, IDataVaultConstructor parent);
        public abstract void Edits(IDataVaultConstructor parent);
        public void Publish()
        {
            fp =false;
            if (self == null)
                Edits(parent);
            else
                Edits(self, parent);
            try
            {

                parent.AddToEditor(this);
               
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
        TextBox bis;
        RoleSelector keyRoles;
        public HubControls(IDataVaultConstructor parent) : base(parent) { }
        public HubControls(IDataTable table, IDataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, IDataVaultConstructor parent)
        {
            Controls.Clear();
            Clear();
            Hub hub = table as Hub;
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new System.Drawing.Point(1, 30);
            this.Name = "HubControls";
            this.Size = new System.Drawing.Size(420, 175);
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
            Panel panel = FieldPanel();
            panel.Controls.Add(name);
            panel.Controls.Add(label);
            label = new Label();
            label.Text = "Business Key";
            label.Name = "bKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            bis = new TextBox();
            bis.Text = fields[0].ToString();
            bis.Name = "bKeyBox";
            bis.Location = new System.Drawing.Point(80, 1);
            bis.Size = new System.Drawing.Size(100, 20);
            panel = FieldPanel();
            keyRoles = new RoleSelector(new HashSet<FieldProperty>(new FieldProperty[] { FieldProperty.key }), new HashSet<FieldProperty>(new FieldProperty[] { FieldProperty.fkey }), hub.ID.Roles, panel);
            panel.Controls.Add(bis);
            panel.Controls.Add(label);
            panel = ControlPanel();
            Button button = new Button();
            button.Name = "UpdateButton";
            button.Text = "Update";
            button.Location = new System.Drawing.Point(1, 1);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += UpdateHub;
            panel.Controls.Add(button);
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "Delete";
            button.Click += parent.Delete;
            button.Click += OnDelete;
            button.Location = new System.Drawing.Point(80, 1);
            button.Size = new System.Drawing.Size(60, 20);
            panel.Controls.Add(button);


        }
        public override void Edits(IDataVaultConstructor parent)
        {
            Controls.Clear();
            Clear();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new System.Drawing.Point(1, 30);
            this.Name = "HubControls";
            this.Size = new System.Drawing.Size(420, 175);
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
            Panel panel= FieldPanel();
            panel.Controls.Add(name);
            panel.Controls.Add(label);
            label = new Label();
            label.Text = "Business Key";
            label.Name = "bKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            bis = new TextBox();
            bis.Text = "";
            bis.Name = "bKeyBox";
            bis.Location = new System.Drawing.Point(80, 1);
            bis.Size = new System.Drawing.Size(100, 20);
            panel = FieldPanel();
            keyRoles = new RoleSelector(new HashSet<FieldProperty>(new FieldProperty[] { FieldProperty.key }), new HashSet<FieldProperty>(new FieldProperty[] { FieldProperty.fkey }), panel);
            panel.Controls.Add(bis);
            panel.Controls.Add(label);
            panel = ControlPanel();
            Button button = new Button();
            button.Name = "AddButton";
            button.Text = "Add";
            button.Location = new System.Drawing.Point(1, 1);
            button.Size = new System.Drawing.Size(60, 20);
            button.Click += AddHub;
            panel.Controls.Add(button);
        }
        private void UpdateHub(object sender, EventArgs args)
        {
            try
            {
                string newName = "";
                string newBKey = "";
                TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Hub must have a name!"); });
                TrueText(bis).Do(s => newBKey = s, () => { throw new ArgumentException("Hub must have a business keu!"); });
                Hub hub = Table.Load() as Hub;
                hub.ID = new DataField(newBKey);
                hub.ID = hub.ID <= keyRoles.Selected;
                hub.Name = newName;
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
                TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Hub must have a name!"); });
                TrueText(bis).Do(s => newBKey = s, () => { throw new ArgumentException("Hub must have a business key!"); });
                Hub hub = new Hub(newName, newBKey);
                hub.ID = hub.ID <= keyRoles.Selected;
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
        RoleSelector keyRoles;
        List<FKEditor> hubKeys = new List<FKEditor>();
        List<RoleSelector> fKeyRoles = new List<RoleSelector>();
        public LinkControls(IDataVaultConstructor parent) : base(parent) { }
        public LinkControls(IDataTable table, IDataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, IDataVaultConstructor parent)
        {
            n = 1;
            Clear();
            Controls.Clear();
            hubKeys.Clear();
            fKeyRoles.Clear();
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
            keyRoles = new RoleSelector(
                new HashSet<FieldProperty> { FieldProperty.key }, 
                new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.time, FieldProperty.pVal }, 
                fields[0].Roles, panel);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            foreach (int k in Enumerable.Range(0, link.Joint.Count))
            {
                panel = FieldPanel();
                label = new Label();
                label.Text = "Hub" + (k+1).ToString();
                label.Name = "nameLabel";
                label.Location = new System.Drawing.Point(1, 1);
                label.Size = new System.Drawing.Size(60, 20);
                FKEditor hub = new FKEditor();
                hub.setTable(link.Joint[k].Item2);
                hub.Text = link.Joint[k].Item1.ToString();
                hub.Name = "bKeyBox";
                hub.Enter+=parent.InvokeSelector(TableType.Hub,hub);
                hub.Location = new System.Drawing.Point(80, 1);
                hub.Size = new System.Drawing.Size(100, 30);
                panel.Controls.Add(label);
                panel.Controls.Add(hub);
                hubKeys.Add(hub);
                fKeyRoles.Add(
                    new RoleSelector(
                        new HashSet<FieldProperty> { FieldProperty.fkey },
                        new HashSet<FieldProperty> { FieldProperty.key }, 
                        link.Joint[k].Item1.Roles, panel)
                        );
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
        public override void Edits(IDataVaultConstructor parent)
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
            keyRoles = new RoleSelector(
               new HashSet<FieldProperty> { FieldProperty.key },
               new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.time, FieldProperty.pVal },
               panel);
            panel.Controls.Add(label);
            panel = FieldPanel();
            label = new Label();
            label.Text = "Hub1";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(60, 20);
            FKEditor hub = new FKEditor();
            hub.Text = "";
            hub.Name = "hKeyBox";
            hub.Location = new System.Drawing.Point(80, 1);
            hub.Enter+=parent.InvokeSelector(TableType.Hub,hub);
            hub.Size = sur.Size;
            fKeyRoles.Add(
                    new RoleSelector(
                        new HashSet<FieldProperty> { FieldProperty.fkey },
                        new HashSet<FieldProperty> { FieldProperty.key }, 
                         panel)
                        );
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
            List<Fkey> newJoint = null;
            try{
                TableControls.TrueText(name).Do(s => { newName = s; }, () => { throw new Exception("Link must have a name!");});
                TableControls.TrueText(sur).Do(s => { newKey = s; }, () => { throw new Exception("Link must have a key!"); });
                FkeyList list = FKEditor.UnloadList(hubKeys);
                newJoint = Enumerable.Zip<string,IDataTable, Fkey>(list.Item1,list.Item2,(str, table) => new Fkey(new DataField(str), table)).ToList();
               
            }catch(Exception x){MessageBox.Show(x.Message); return;}
            link.Name = newName;
            link.Joint = newJoint;
            foreach (int k in Enumerable.Range(0, link.Joint.Count))
                link.Joint[k] = new Fkey( link.Joint[k].Item1 <= fKeyRoles[k].Selected, link.Joint[k].Item2);
            link.Key = new DataField(newKey);
            link.Key = link.Key <= keyRoles.Selected;
            parent.Refresh();
            parent.UpdateEditor();
            Publish();
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
            FKEditor hub = new FKEditor();
            hub.Text = "";
            hub.Name = "bKeyBox";
            hub.Location = new System.Drawing.Point(80, 1);
            hub.Size = new System.Drawing.Size(100, 20);
            hub.Enter+=parent.InvokeSelector(TableType.Hub,hub);
            fKeyRoles.Add(
                    new RoleSelector(
                        new HashSet<FieldProperty> { FieldProperty.fkey },
                        new HashSet<FieldProperty> { FieldProperty.key },
                         panel)
                        );
            panel.Controls.Add(label);
            panel.Controls.Add(hub);
            hubKeys.Add(hub);
            
        }
        private void AddLink(object sender, EventArgs e)
        {
            string newName = "";
            string newKey = "";
            List<string> newNames = null;
            List<Hub> newHubs =new List<Hub>();
            try
            {
                TableControls.TrueText(name).Do(s => { newName = s; }, () => { throw new Exception("Link must have a name!"); });
                TableControls.TrueText(sur).Do(s => { newKey = s; }, () => { throw new Exception("Link must have a key!"); });
                FkeyList list = FKEditor.UnloadList(hubKeys);
                newNames = list.Item1;
                foreach (IDataTable table in list.Item2)
                    newHubs.Add(table as Hub);
            }
            catch (Exception x) { MessageBox.Show(x.Message); return; }
            Link link = new Link(newName, newHubs, newNames, newKey);
            foreach (int k in Enumerable.Range(0, link.Joint.Count))
                link.Joint[k] = new Fkey(link.Joint[k].Item1 <= fKeyRoles[k].Selected, link.Joint[k].Item2);
            link.Key = link.Key <= keyRoles.Selected;
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
        FKEditor link;
        RoleSelector keyRoles;
        RoleSelector fKeyRoles;
        List<TextBox> measures = new List<TextBox>();
        List<FKEditor> refKeys = new List<FKEditor>();
        List<RoleSelector> mesRoles = new List<RoleSelector>();
        List<RoleSelector> refRoles = new List<RoleSelector>();
        public SateliteControls(IDataVaultConstructor parent) : base(parent) { }
        public SateliteControls(IDataTable table, IDataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, IDataVaultConstructor parent)
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
            keyRoles = new RoleSelector(
                new HashSet<FieldProperty> { FieldProperty.key },
                new HashSet<FieldProperty> { FieldProperty.fkey },
                fields[0].Roles, panel);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            panel = FieldPanel();
            link = new FKEditor();
            label = new Label();
            label.Text = "Link Key";
            label.Name = "kinkLabel";
            label.Location = new System.Drawing.Point(1, 1);
            link.Name = "fKeyBox";
            link.Text = fields[1].ToString();
            link.setTable(sat.Link.Item2);
            link.Location = new System.Drawing.Point(80, 1);
            link.Size = new System.Drawing.Size(100, 20);
            link.Enter += parent.InvokeSelector(TableType.Link, link);
            fKeyRoles = new RoleSelector(
                new HashSet<FieldProperty> { FieldProperty.fkey },
                new HashSet<FieldProperty> { FieldProperty.key },
                fields[1].Roles, panel);
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
                mesRoles.Add(new RoleSelector (
                    new HashSet<FieldProperty>{},
                    new HashSet<FieldProperty>{FieldProperty.key, FieldProperty.fkey},
                    fields[k].Roles, panel));
                measures.Add(mes);
                n = k;
            }
            m = n;
            foreach (int k in Enumerable.Range(0, sat.References.Count))
            {
                panel = FieldPanel();
                label = new Label();
                label.Text = "reference" + (k - n + 1).ToString();
                label.Name = "=meLabel";
                label.Location = new System.Drawing.Point(1, 1);
                label.Size = new Size(60, 20);
                FKEditor reference = new FKEditor();
                reference.Text = sat.References[k].Item1.ToString();
                reference.setTable(sat.References[k].Item2);
                reference.Name = "refBox";
                reference.Location = new System.Drawing.Point(80, 1);
                reference.Size = new System.Drawing.Size(100, 30);
                reference.Enter+=parent.InvokeSelector(TableType.Reference, reference);
                panel.Controls.Add(label);
                panel.Controls.Add(reference);
                refRoles.Add(new RoleSelector(
                   new HashSet<FieldProperty> { FieldProperty.fkey},
                   new HashSet<FieldProperty> { FieldProperty.key },
                   sat.References[k].Item1.Roles, panel));
                refKeys.Add(reference);
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
        public override void Edits(IDataVaultConstructor parent)
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
            keyRoles = new RoleSelector(
                new HashSet<FieldProperty> { FieldProperty.key },
                new HashSet<FieldProperty> { FieldProperty.fkey },
                panel);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            panel = FieldPanel();
            link = new FKEditor();
            label = new Label();
            label.Text = "Link Key";
            label.Name = "kinkLabel";
            label.Location = new System.Drawing.Point(1, 1);
            link.Name = "fKeyBox";
            link.Text = "";
            link.Location = new System.Drawing.Point(80, 1);
            link.Size = new System.Drawing.Size(100, 20);
            link.Enter+=parent.InvokeSelector(TableType.Link,link);
            fKeyRoles = new RoleSelector(
                new HashSet<FieldProperty> { FieldProperty.fkey },
                new HashSet<FieldProperty> { FieldProperty.key },
                panel);
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
            mesRoles.Add(new RoleSelector(
                new HashSet<FieldProperty> { },
                new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key},
                panel));
            panel.Controls.Add(label);
            panel.Controls.Add(mes);
            measures.Add(mes);
            panel = FieldPanel();
            label = new Label();
            label.Text = "reference" + (1).ToString();
            label.Name = "=meLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(60, 20);
            refRoles.Add(new RoleSelector(
                new HashSet<FieldProperty> {  FieldProperty.fkey },
                new HashSet<FieldProperty> {  FieldProperty.key },
                panel));
            FKEditor reference = new FKEditor();
            reference.Text = "";
            reference.Name = "measureBox";
            reference.Location = new System.Drawing.Point(80, 1);
            reference.Size = new System.Drawing.Size(100, 30);
            reference.Enter+=parent.InvokeSelector(TableType.Reference,reference);
            panel.Controls.Add(label);
            panel.Controls.Add(reference);
            refKeys.Add(reference);
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
            Fkey newLink = null;
            List<DataField> newFields = new List<DataField>();
            List<Fkey> newRefs = new List<Fkey>();
            try
            {
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new System.ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new System.ArgumentException("Table must have a key!"); });
                FKEditor.Unload(link).Do(t => newLink = t , () =>{throw new System.ArgumentException("Satelite must have a foreign key of a Link!");});
                foreach (int k in Enumerable.Range(0,measures.Count))
                    TableControls.TrueText(measures[k]).Do(s => newFields.Add(new DataField(s) <= mesRoles[k].Selected));
                FkeyList list = FKEditor.UnloadList(refKeys);
                foreach (int k in Enumerable.Range(0, refKeys.Count ))
                {
                    newRefs.Add(new Fkey(new DataField(list.Item1[k]) <= refRoles[k].Selected,list.Item2[k]));
                }
                Satelite satelite = Table.Load() as Satelite;
                satelite.Name = newName;
                satelite.Key = new DataField(newKey) <= keyRoles.Selected;
                satelite.Link = new Fkey(newLink.Item1 <= fKeyRoles.Selected, newLink.Item2);
                satelite.References = newRefs;
                satelite.Measures = newFields;
                parent.UpdateEditor();
                parent.Refresh();
                Publish();
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
            label.Size = new Size(80, 20);
            FKEditor reference = new FKEditor();
            reference.Text = "";
            reference.Name = "refBox";
            reference.Location = new System.Drawing.Point(80, 1);
            reference.Size = new System.Drawing.Size(100, 30);
            reference.Enter+=parent.InvokeSelector(TableType.Reference,reference);
            panel.Controls.Add(label);
            panel.Controls.Add(reference);
            refKeys.Add(reference);
            refRoles.Add(new RoleSelector(
                new HashSet<FieldProperty> { FieldProperty.fkey },
                new HashSet<FieldProperty> { FieldProperty.key },
                panel));
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
            mesRoles.Add(new RoleSelector(
                new HashSet<FieldProperty> { },
                new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key },
                panel));
            Refresh();
        }
        protected void AddSatelite(Object sender, EventArgs args)
        {

            string newName = null;
            string newKey = null;
            Fkey newLink = null;
            List<string> newFields = new List<string>();
            List<Reference> newRefs = new List<Reference>();
            List<string> refNames = new List<string>();
            try
            {
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new System.ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new System.ArgumentException("Table must have a key!"); });
                FKEditor.Unload(link).Do(s => newLink = s, () =>{throw new System.ArgumentException("Satelite must have a foreign key of a Link!");} );
                foreach (TextBox mes in measures)
                    TableControls.TrueText(mes).Do(s => newFields.Add(s));
                FkeyList list = FKEditor.UnloadList(refKeys);
                refNames =  list.Item1;
                newRefs = Enumerable.Select<IDataTable, Reference>(list.Item2, t => t as Reference).ToList();
                Link l = newLink.Item2 as Link;
                Satelite satelite = new Satelite(newName, l , newLink.Item1.ToString(), newKey, newFields, newRefs, refNames);
                satelite.Key = satelite.Key <= keyRoles.Selected;
                satelite.Link = new Fkey( satelite.Link.Item1 <= fKeyRoles.Selected, satelite.Link.Item2);
                foreach(int k in Enumerable.Range(0,satelite.Measures.Count))
                    satelite.Measures[k]= satelite.Measures[k] <= mesRoles[k].Selected;
                foreach (int k in Enumerable.Range(0, satelite.References.Count))
                    satelite.References[k] = new Fkey(satelite.References[k].Item1 <= refRoles[k].Selected,satelite.References[k].Item2);
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
        RoleSelector keyRoles;
        List<TextBox> measures = new List<TextBox>();
        List<RoleSelector> mesRoles = new List<RoleSelector>();
        public ReferenceControls(IDataVaultConstructor parent) : base(parent) { }
        public ReferenceControls(IDataTable table, IDataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, IDataVaultConstructor parent)
        {
            n = 1;
            Clear();
            Controls.Clear();
            measures.Clear();
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            this.Size = new Size(420, 175);
            this.TabIndex = 0;
            Reference reference = table as Reference;
            DataField[] fields = reference.Content();
            Label label = new Label();
            label.Text = "Name";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            Panel panel = FieldPanel();
            name = new TextBox();
            name.Text = reference.Name;
            name.Name = "bKeyBox";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(name);
            panel.Controls.Add(label);
            panel = FieldPanel();
            sur = new TextBox();
            label = new Label();
            label.Text = "Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            sur = new TextBox();
            sur.Name = "sKeyBox";
            sur.Text = fields[0].ToString();
            sur.Location = new System.Drawing.Point(80, 1);
            sur.Size = new System.Drawing.Size(100, 20);
            keyRoles = new RoleSelector(
                new HashSet<FieldProperty> { FieldProperty.key },
                new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.pID },
                fields[0].Roles, panel);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            foreach (int k in Enumerable.Range(0,  reference.Fields.Count))
            {
                panel = FieldPanel();
                label = new Label();
                label.Text = "Field" + k.ToString();
                label.Name = "nameLabel";
                label.Location = new System.Drawing.Point(1, 1);
                label.Size = new Size(80, 20);
                TextBox field = new TextBox();
                field.Text = reference.Fields[k].ToString();
                field.Name = "bKeyBox";
                field.Location = new System.Drawing.Point(80, 1);
                field.Size = new System.Drawing.Size(100, 1);
                mesRoles.Add(new RoleSelector(
                  new HashSet<FieldProperty> { },
                  new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key },
                  reference.Fields[k].Roles, panel));
                panel.Controls.Add(label);
                panel.Controls.Add(field);
                measures.Add(field);
                n = k;
            }
            panel = ControlPanel();
            Button button = new Button();
            button.Name = "UpdateButton";
            button.Text = "Update";
            button.Location = new System.Drawing.Point(1, 1);
            button.Size = new System.Drawing.Size(80, 20);
            button.Click += UpdateRef;
            panel.Controls.Add(button);
            button = new Button();
            button.Name = "deleteButton";
            button.Text = "Delete";
            button.Click += parent.Delete;
            button.Click += OnDelete;
            button.Location = new System.Drawing.Point(80, 1);
            button.Size = new System.Drawing.Size(80, 20);
            panel.Controls.Add(button);
            button = new Button();
            button.Name = "newFieldButton";
            button.Text = "Add Field";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(160, 1);
            button.Size = new System.Drawing.Size(80, 20);
            panel.Controls.Add(button);
        }
        public override void Edits(IDataVaultConstructor parent)
        {
            n = 1;
            Clear();
            Controls.Clear();
            measures.Clear();
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
            name = new TextBox();
            name.Text = "";
            name.Name = "name";
            name.Location = new System.Drawing.Point(80, 1);
            name.Size = new System.Drawing.Size(100, 20);
            panel.Controls.Add(name);
            panel.Controls.Add(label);
            sur = new TextBox();
            label = new Label();
            label.Text = "Surrogate Key";
            label.Name = "sKeyLabel";
            label.Location = new System.Drawing.Point(1, 1);
            panel = FieldPanel();
            sur = new TextBox();
            sur.Name = "sKeyBox";
            sur.Text = "";
            sur.Location = new System.Drawing.Point(80, 1);
            sur.Size = new System.Drawing.Size(100, 20);
            keyRoles = new RoleSelector(
                new HashSet<FieldProperty> { FieldProperty.key },
                new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.pID },
                panel);
            panel.Controls.Add(sur);
            panel.Controls.Add(label);
            panel = FieldPanel();
            label = new Label();
            label.Text = "Field1";
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(80, 20);
            TextBox mes = new TextBox();
            mes.Text = "";
            mes.Name = "hKeyBox";
            mes.Location = new System.Drawing.Point(80, 1);
            mes.Size = new System.Drawing.Size(100, 60);
            mesRoles.Add(new RoleSelector(
               new HashSet<FieldProperty> { },
               new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key },
               panel));
            panel.Controls.Add(label);
            panel.Controls.Add(mes);
            measures.Add(mes);
            panel = ControlPanel();
            Button button = new Button();
            button.Name = "addButton";
            button.Text = "Create";
            button.Location = new System.Drawing.Point(1, 1);
            button.Size = new System.Drawing.Size(80, 20);
            button.Click += AddRef;
            panel.Controls.Add(button);
            
            button = new Button();
            button.Name = "newHubButton";
            button.Text = "Add Field";
            button.Click += AddField;
            button.Location = new System.Drawing.Point(80, 1);
            button.Size = new System.Drawing.Size(80, 20);
            panel.Controls.Add(button);
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
                foreach (int k in Enumerable.Range(0,measures.Count))
                    TableControls.TrueText(measures[k]).Do(s => newFields.Add(new DataField(s) <= mesRoles[k].Selected));
                if (newFields.Count == 0)
                    throw new ArgumentException("Table must have at least one meaningful field!");
                Reference reference = Table.Load() as Reference;
                reference.Name = newName;
                reference.Key = new DataField(newKey) <= keyRoles.Selected;
                reference.Fields = newFields;
                parent.UpdateEditor();
                parent.Refresh();
                Publish();
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
                reference.Key = reference.Key <= keyRoles.Selected;
                foreach (int k in Enumerable.Range(0, reference.Fields.Count))
                    reference.Fields[k] = reference.Fields[k] <= mesRoles[k].Selected;
                Table = reference;
                parent.AddTable(reference);
                Publish();
                Refresh();
            }
            catch (ArgumentException e) { MessageBox.Show(e.Message); }
        }
        protected void AddField(object sender, EventArgs args)
        {
            n++;
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = "Field" + n.ToString();
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(80, 20);
            TextBox field = new TextBox();
            field.Text = "";
            field.Name = "bKeyBox";
            field.Location = new System.Drawing.Point(80, 1);
            field.Size = new System.Drawing.Size(100, 1);
            mesRoles.Add(new RoleSelector(
              new HashSet<FieldProperty> { },
              new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key },
              panel));
            panel.Controls.Add(label);
            panel.Controls.Add(field);
            measures.Add(field);
            Refresh();
        }
    }
    }  

