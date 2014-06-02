using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Monad;

namespace DataVault
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
        protected Panel controlsPanel = null;
        int n = 0;
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
            n = 0;
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
        public Panel AddField(string text, string name, out TextBox field)
        {
            Label label = new Label();
 
            label.Text = name;
            label.Name = "fieldLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(80,20);
            field = new TextBox();
            field.Text = text;
            field.Name = "bKeyBox";
            field.Location = new System.Drawing.Point(80, 1);
            field.Size = new System.Drawing.Size(100, 20);
            Panel panel = FieldPanel();
            panel.Controls.Add(field);
            panel.Controls.Add(label);
            return panel;
        }
        protected Panel AddField(string text, string name, out TextBox field, HashSet<FieldProperty> locked, HashSet<FieldProperty> blocked, out RoleSelector selector)
        {
            Panel panel = AddField(text, name, out field);
            selector = new RoleSelector(locked, blocked, panel);
            return panel;
        }
        protected Panel AddField(string text, string name, out TextBox field, HashSet<FieldProperty> locked, HashSet<FieldProperty> blocked,HashSet<FieldProperty> choosen, out RoleSelector selector)
        {
            Panel panel = AddField(text, name, out field);
            selector = new RoleSelector(locked, blocked, choosen, panel);
            return panel;
        }
        protected void AddButton(string name, EventHandler action)
        {
            if (controlsPanel == null)
                controlsPanel = ControlPanel();
            Button button = new Button();
            button.Name = "Button";
            button.Text = name;
            button.Click += action;
            button.Location = new System.Drawing.Point(n*90, 1);
            button.Size = new System.Drawing.Size(90, 20);
            controlsPanel.Controls.Add(button);
            n++;
        }
        protected void AddButton(string name, IEnumerable<EventHandler> actions)
        {
            if (controlsPanel == null)
                controlsPanel = ControlPanel();
            Button button = new Button();
            button.Name = "Button";
            button.Text = name;
            foreach(EventHandler action in actions)
                button.Click += action;
            button.Location = new System.Drawing.Point(n * 90, 1);
            button.Size = new System.Drawing.Size(90, 20);
            controlsPanel.Controls.Add(button);
            n++;
        }
        protected void AddFkey(string text, string name, TableType type , out FKEditor fk, HashSet<FieldProperty> locked, HashSet<FieldProperty> blocked, out RoleSelector selector)
        {
            Panel panel = FieldPanel(); 
            Label label = new Label();
            label.Text = name;
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(80, 20);
            fk = new FKEditor();
            fk.Text = text;
            fk.Name = "hKeyBox";
            fk.Location = new System.Drawing.Point(80, 1);
            fk.Enter += parent.InvokeSelector(type, fk);
            fk.Size = new System.Drawing.Size(100, 20);
            selector =new RoleSelector(locked,blocked,panel);
            panel.Controls.Add(label);
            panel.Controls.Add(fk);
        }
        protected void AddFkey(string text, string name, IDataTable table, TableType type, out FKEditor fk, HashSet<FieldProperty> locked, HashSet<FieldProperty> blocked, HashSet<FieldProperty> choosen,  out RoleSelector selector)
        {
            Panel panel = FieldPanel();
            Label label = new Label();
            label.Text = name;
            label.Name = "nameLabel";
            label.Location = new System.Drawing.Point(1, 1);
            label.Size = new Size(80, 20);
            fk = new FKEditor();
            fk.Text = text;
            fk.Name = "hKeyBox";
            fk.Location = new System.Drawing.Point(80, 1);
            fk.Enter += parent.InvokeSelector(type, fk);
            fk.Size = new System.Drawing.Size(100, 20);
            fk.setTable(table);
            selector = new RoleSelector(locked, blocked, choosen, panel);
            panel.Controls.Add(label);
            panel.Controls.Add(fk);
        }
    }
    public class HubControls : TableControls
    {
        TextBox name;
        TextBox bis;
        TextBox time;
        RoleSelector keyRoles;
        RoleSelector timeRoles;
        public HubControls(IDataVaultConstructor parent) : base(parent) { }
        public HubControls(IDataTable table, IDataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, IDataVaultConstructor parent)
        {
            Controls.Clear();
            Clear();
            controlsPanel = null;
            Hub hub = table as Hub;
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = false;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new System.Drawing.Point(1, 30);
            this.Name = "HubControls";
            this.Size = new System.Drawing.Size(420, 175);
            this.TabIndex = 0;
            AddField(hub.Name, "Name:", out name);
            AddField(hub.ID.ToString(), "Key:", out bis, new HashSet<FieldProperty> { FieldProperty.key }, new HashSet<FieldProperty> { FieldProperty.fkey }, hub.ID.Roles, out keyRoles);
            AddField(hub.TimeStamp.ToString(), "TimeStamp:", out time, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key }, hub.TimeStamp.Roles, out timeRoles);
            AddButton("Update", UpdateHub);
            AddButton("Delete", new EventHandler[]{parent.Delete,OnDelete});


        }
        public override void Edits(IDataVaultConstructor parent)
        {
            controlsPanel = null;
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
            AddField("", "Name:", out name);
            AddField("", "Key:", out bis, new HashSet<FieldProperty> { FieldProperty.key }, new HashSet<FieldProperty> { FieldProperty.fkey }, out keyRoles);
            AddField("", "TimeStamp:", out time, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key }, out timeRoles);
            AddButton("Create", AddHub);
        }
        private void UpdateHub(object sender, EventArgs args)
        {
            try
            {
                string newName = "";
                string newBKey = "";
                string newTime = "";
                TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Hub must have a name!"); });
                TrueText(bis).Do(s => newBKey = s, () => { throw new ArgumentException("Hub must have a business key!"); });
                TrueText(time).Do(s => newTime = s, () => { throw new ArgumentException("Hub must have a Time stamp!"); });
                Hub hub = Table.Load() as Hub;
                hub.ID = new DataField(newBKey) <= keyRoles.Selected;
                hub.TimeStamp = new DataField(newTime) <= timeRoles.Selected;
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
                string newTime = "";
                TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Hub must have a name!"); });
                TrueText(bis).Do(s => newBKey = s, () => { throw new ArgumentException("Hub must have a business key!"); });
                TrueText(time).Do(s => newTime = s, () => { throw new ArgumentException("Hub must have a Time stamp!"); });
                Hub hub = new Hub(newName, newBKey);
                hub.TimeStamp = new DataField(newTime) <= timeRoles.Selected;
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
        TextBox time;
        RoleSelector keyRoles;
        RoleSelector timeRoles;
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
            controlsPanel = null;
            Link link = table as Link;
            DataField[] fields = link.Content();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            this.Size = new Size(420, 175);
            this.TabIndex = 0;
            AddField(link.Name, "Name:", out name);
            AddField(link.Key.ToString(), "Key:", out sur, new HashSet<FieldProperty> { FieldProperty.key }, new HashSet<FieldProperty> { FieldProperty.fkey }, link.Key.Roles, out keyRoles);
            AddField(link.TimeStamp.ToString(), "TimeStamp:", out time, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key }, link.TimeStamp.Roles, out timeRoles);
            foreach (Fkey fkey in link.Joint)
            {
                FKEditor fk;
                RoleSelector fkRole;
                AddFkey(fkey.Item1.ToString(), "Hub " + n.ToString() + ":", fkey.Item2, TableType.Hub, out fk,
                    new HashSet<FieldProperty> { FieldProperty.fkey }, new HashSet<FieldProperty> { FieldProperty.key }, fkey.Item1.Roles, out fkRole);
                hubKeys.Add(fk);
                fKeyRoles.Add(fkRole);
                n++;
            }
           
            AddButton("Update", UpdateLink);
            AddButton("Delete", new EventHandler[] { parent.Delete, OnDelete });
            AddButton("Add Hub", AddHub);
        }
        public override void Edits(IDataVaultConstructor parent)
        {
            n = 1;
            Clear();
            Controls.Clear();
            hubKeys.Clear();
            controlsPanel = null;
            List<Point> movables = new List<Point>();
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            this.Size = new Size(430, 175);
            this.TabIndex = 0;
            AddField("", "Name:", out name);
            AddField("", "Key:", out sur, new HashSet<FieldProperty> { FieldProperty.key }, new HashSet<FieldProperty> { FieldProperty.fkey }, out keyRoles);
            AddField("", "TimeStamp:", out time, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key }, out timeRoles);
            FKEditor fk;
            RoleSelector fkRole;
            AddFkey("", "Hub " + n.ToString() + ":",  TableType.Hub, out fk,
                new HashSet<FieldProperty> { FieldProperty.fkey }, new HashSet<FieldProperty> { FieldProperty.key },  out fkRole);
            hubKeys.Add(fk);
            fKeyRoles.Add(fkRole);
            n++;
            AddButton("Create", AddLink);
            AddButton("Add Hub", AddHub);
        }
        private void UpdateLink(object sender, EventArgs e)
        {
            string newName = "";
            Link link = Table.Load() as Link;
            string newKey = "";
            string newTime = "";
            List<Fkey> newJoint = null;
            try{
                TableControls.TrueText(name).Do(s => { newName = s; }, () => { throw new Exception("Link must have a name!");});
                TableControls.TrueText(sur).Do(s => { newKey = s; }, () => { throw new Exception("Link must have a key!"); });
                TableControls.TrueText(time).Do(s => { newTime = s; }, () => { throw new Exception("Link must have a time stamp!"); });
                FkeyList list = FKEditor.UnloadList(hubKeys);
                newJoint = Enumerable.Zip<string,IDataTable, Fkey>(list.Item1,list.Item2,(str, table) => new Fkey(new DataField(str), table)).ToList();
               
            }catch(Exception x){MessageBox.Show(x.Message); return;}
            link.Name = newName;
            link.Joint = newJoint;
            foreach (int k in Enumerable.Range(0, link.Joint.Count))
                link.Joint[k] = new Fkey( link.Joint[k].Item1 <= fKeyRoles[k].Selected, link.Joint[k].Item2);
            link.Key = new DataField(newKey);
            link.Key = link.Key <= keyRoles.Selected;
            link.TimeStamp = new DataField(newTime) <= timeRoles.Selected;
            parent.Refresh();
            parent.UpdateEditor();
            Publish();
        }
        private void AddHub(object sender, EventArgs e)
        {
            FKEditor fk;
            RoleSelector fkRole;
            AddFkey("", "Hub " + n.ToString() + ":", TableType.Hub, out fk,
                new HashSet<FieldProperty> { FieldProperty.fkey }, new HashSet<FieldProperty> { FieldProperty.key }, out fkRole);
            hubKeys.Add(fk);
            fKeyRoles.Add(fkRole);
            n++;
            
        }
        private void AddLink(object sender, EventArgs e)
        {
            string newName = "";
            string newKey = "";
            string newTime = "";
            List<string> newNames = null;
            List<Hub> newHubs =new List<Hub>();
            try
            {
                TableControls.TrueText(name).Do(s => { newName = s; }, () => { throw new Exception("Link must have a name!"); });
                TableControls.TrueText(sur).Do(s => { newKey = s; }, () => { throw new Exception("Link must have a key!"); });
                TableControls.TrueText(time).Do(s => { newTime = s; }, () => { throw new Exception("Link must have a time stamp!"); });
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
            link.TimeStamp = new DataField(newTime) <= timeRoles.Selected;
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
        TextBox time;
        FKEditor link;
        RoleSelector keyRoles;
        RoleSelector fKeyRoles;
        RoleSelector timeRoles;
        List<TextBox> measures = new List<TextBox>();
        List<FKEditor> refKeys = new List<FKEditor>();
        List<RoleSelector> mesRoles = new List<RoleSelector>();
        List<RoleSelector> refRoles = new List<RoleSelector>();
        public SateliteControls(IDataVaultConstructor parent) : base(parent) { }
        public SateliteControls(IDataTable table, IDataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, IDataVaultConstructor parent)
        {
            n = 1;
            m = 1;
            controlsPanel = null;
            Controls.Clear();
            refKeys.Clear();
            measures.Clear();
            mesRoles.Clear();
            refRoles.Clear();
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
            AddField(sat.Name, "Name:", out name);
            AddField(sat.Key.ToString(), "Key:", out sur, new HashSet<FieldProperty> { FieldProperty.key }, new HashSet<FieldProperty> { FieldProperty.fkey }, sat.Key.Roles, out keyRoles);
            AddField(sat.TimeStamp.ToString(), "TimeStamp:", out time, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key }, sat.TimeStamp.Roles, out timeRoles);
            AddFkey(sat.Link.Item1.ToString(), "Link:", sat.Link.Item2, TableType.Link, out link, new HashSet<FieldProperty> { FieldProperty.fkey }, new HashSet<FieldProperty> { FieldProperty.key }, sat.Link.Item1.Roles, out fKeyRoles);
            foreach (DataField measure in sat.Measures)
            {
               
                TextBox mes;
                RoleSelector mesRole;
                AddField(measure.ToString(), "Measure " + n.ToString() + ":", out mes, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey }, measure.Roles, out mesRole);
                measures.Add(mes);
                mesRoles.Add(mesRole);
                n++;

            }
            foreach (Fkey fk in sat.References)
            {
                
                FKEditor editor;
                RoleSelector refRole;
                AddFkey(fk.Item1.ToString(), "Reference " + m.ToString() + ":", fk.Item2, TableType.Reference, out editor, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey }, fk.Item1.Roles, out refRole);
                refKeys.Add(editor);
                refRoles.Add(refRole);
                m++;
            }
            AddButton("Update", UpdateSatelite);
            AddButton("Delete", new EventHandler[] { parent.Delete, OnDelete });
            AddButton("Add reference", AddRef);
            AddButton("Add measure", AddMeasure);
        }
        public override void Edits(IDataVaultConstructor parent)
        {
            n = 1;
            m = 1;
            controlsPanel = null;
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
            AddField("", "Name:", out name);
            AddField("", "Key:", out sur, new HashSet<FieldProperty> { FieldProperty.key }, new HashSet<FieldProperty> { FieldProperty.fkey }, out keyRoles);
            AddField("", "TimeStamp:", out time, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key }, out timeRoles);
            AddFkey("", "Link:", TableType.Link, out link, new HashSet<FieldProperty> { FieldProperty.fkey }, new HashSet<FieldProperty> { FieldProperty.key }, out fKeyRoles);
            TextBox mes;
            RoleSelector mesRole;
            AddField("", "Measure " + n.ToString() + ":", out mes, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey },  out mesRole);
            measures.Add(mes);
            mesRoles.Add(mesRole);
            n++;
            FKEditor editor;
            RoleSelector refRole;
            AddFkey("", "Reference " + m.ToString() + ":", TableType.Reference, out editor, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey }, out refRole);
            refKeys.Add(editor);
            refRoles.Add(refRole);
            m++;
            AddButton("Create", AddSatelite);
            AddButton("Add measure", AddMeasure);
            AddButton("Add reference", AddRef);

        }

        protected void UpdateSatelite(Object sender, EventArgs args)
        {

            string newName = null;
            string newKey = null;
            string newTime = null;
            Fkey newLink = null;
            List<DataField> newFields = new List<DataField>();
            List<Fkey> newRefs = new List<Fkey>();
            try
            {
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new System.ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new System.ArgumentException("Table must have a key!"); });
                TableControls.TrueText(time).Do(s => newTime = s, () => { throw new System.ArgumentException("Table must have a time stamp!"); });
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
                satelite.TimeStamp = new DataField(newTime) <= timeRoles.Selected;
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
           
            FKEditor editor;
            RoleSelector refRole;
            AddFkey("", "Reference " + m.ToString() + ":", TableType.Reference, out editor, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey }, out refRole);
            refKeys.Add(editor);
            refRoles.Add(refRole);
            m++;
        }
        protected void AddMeasure(Object sender, EventArgs args)
        {
            TextBox mes;
            RoleSelector mesRole;
            AddField("", "Measure " + n.ToString() + ":", out mes, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey }, out mesRole);
            measures.Add(mes);
            mesRoles.Add(mesRole);
            n++;

        }
        protected void AddSatelite(Object sender, EventArgs args)
        {

            string newName = null;
            string newKey = null;
            string newTime = null;
            Fkey newLink = null;
            List<string> newFields = new List<string>();
            List<Reference> newRefs = new List<Reference>();
            List<string> refNames = new List<string>();
            try
            {
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new System.ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new System.ArgumentException("Table must have a key!"); });
                TableControls.TrueText(time).Do(s => newTime = s, () => { throw new System.ArgumentException("Table must have a time stamp!"); });
                FKEditor.Unload(link).Do(s => newLink = s, () =>{throw new System.ArgumentException("Satelite must have a foreign key of a Link!");} );
                foreach (TextBox mes in measures)
                    TableControls.TrueText(mes).Do(s => newFields.Add(s));
                FkeyList list = FKEditor.UnloadList(refKeys);
                refNames =  list.Item1;
                newRefs = Enumerable.Select<IDataTable, Reference>(list.Item2, t => t as Reference).ToList();
                Link l = newLink.Item2 as Link;
                Satelite satelite = new Satelite(newName, l , newLink.Item1.ToString(), newKey, newFields, newRefs, refNames);
                satelite.Key = satelite.Key <= keyRoles.Selected;
                satelite.TimeStamp = new DataField(newTime) <= timeRoles.Selected;
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
        TextBox name, sur, time;
        RoleSelector keyRoles, timeRoles;
        List<TextBox> measures = new List<TextBox>();
        List<RoleSelector> mesRoles = new List<RoleSelector>();
        public ReferenceControls(IDataVaultConstructor parent) : base(parent) { }
        public ReferenceControls(IDataTable table, IDataVaultConstructor parent) : base(table, parent) { }
        public override void Edits(IDataTable table, IDataVaultConstructor parent)
        {
            n = 1;
            Clear();
            controlsPanel = null;
            Controls.Clear();
            measures.Clear();
            mesRoles.Clear();
            //this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //| System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            Dock = DockStyle.Fill;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
            //this.Size = new Size(420, parent.Height() - 50);
            this.TabIndex = 0;
            Reference reference = table as Reference;
            AddField(reference.Name, "Name:", out name);
            AddField(reference.Key.ToString(), "Key:", out sur, new HashSet<FieldProperty> { FieldProperty.key }, new HashSet<FieldProperty> { FieldProperty.fkey }, reference.Key.Roles, out keyRoles);
            AddField(reference.TimeStamp.ToString(), "TimeStamp:", out time, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key }, reference.TimeStamp.Roles, out timeRoles);
            foreach (DataField measure in reference.Fields)
            {

                TextBox mes;
                RoleSelector mesRole;
                AddField(measure.ToString(), "Measure " + n.ToString() + ":", out mes, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey }, measure.Roles, out mesRole);
                measures.Add(mes);
                mesRoles.Add(mesRole);
                n++;
            }
            AddButton("Update", UpdateRef);
            AddButton("Delete", new EventHandler[] { parent.Delete, OnDelete });
            AddButton("Add Measure", AddMeasure);

        }
        public override void Edits(IDataVaultConstructor parent)
        {
            n = 1;
            Clear();
            Controls.Clear();
            measures.Clear();
            controlsPanel = null;
            mesRoles.Clear();
            //this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //| System.Windows.Forms.AnchorStyles.Right) | AnchorStyles.Left));
            Dock = DockStyle.Fill;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Location = new Point(1, 30);
            this.Name = "HubControls";
           // this.Size = new Size(420, parent.Height() - 50);
            this.TabIndex = 0;
            AddField("", "Name:", out name);
            AddField("", "Key:", out sur, new HashSet<FieldProperty> { FieldProperty.key }, new HashSet<FieldProperty> { FieldProperty.fkey },  out keyRoles);
            AddField("", "TimeStamp:", out time, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.fkey, FieldProperty.key },  out timeRoles);
            TextBox mes;
            RoleSelector mesRole;
            AddField("", "Measure " + n.ToString() + ":", out mes, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey },  out mesRole);
            measures.Add(mes);
            mesRoles.Add(mesRole);
            n++;
            AddButton("Create", AddRef);
            AddButton("Add measure", AddMeasure);
        }
        protected void UpdateRef(object srnder, EventArgs args)
        {
            string newName = null;
            string newKey = null;
            string newTime = null;
            List<DataField> newFields = new List<DataField>();
            try
            {
                TableControls.TrueText(name).Do(s => newName = s, () => { throw new ArgumentException("Table must have a name!"); });
                TableControls.TrueText(sur).Do(s => newKey = s, () => { throw new ArgumentException("Table must have a Key!"); });
                TableControls.TrueText(time).Do(s => newTime = s, () => { throw new ArgumentException("Table must have a time stamp!"); });
                foreach (int k in Enumerable.Range(0,measures.Count))
                    TableControls.TrueText(measures[k]).Do(s => newFields.Add(new DataField(s) <= mesRoles[k].Selected));
                if (newFields.Count == 0)
                    throw new ArgumentException("Table must have at least one meaningful field!");
                Reference reference = Table.Load() as Reference;
                reference.Name = newName;
                reference.Key = new DataField(newKey) <= keyRoles.Selected;
                reference.TimeStamp = new DataField(newKey) <= timeRoles.Selected;
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
            string newTime = null;
            TableControls.TrueText(time).Do(s => newTime = s, () => { throw new ArgumentException("Table must have a time stamp!"); });
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
                reference.TimeStamp = new DataField(newKey) <= timeRoles.Selected;
                foreach (int k in Enumerable.Range(0, reference.Fields.Count))
                    reference.Fields[k] = reference.Fields[k] <= mesRoles[k].Selected;
                Table = reference;
                parent.AddTable(reference);
                Publish();
                Refresh();
            }
            catch (ArgumentException e) { MessageBox.Show(e.Message); }
        }
        protected void AddMeasure(object sender, EventArgs args)
        {
            TextBox mes;
            RoleSelector mesRole;
            AddField("", "Measure " + n.ToString() + ":", out mes, new HashSet<FieldProperty> { }, new HashSet<FieldProperty> { FieldProperty.key, FieldProperty.fkey }, out mesRole);
            measures.Add(mes);
            mesRoles.Add(mesRole);
            n++;
        }
    }
    }  

