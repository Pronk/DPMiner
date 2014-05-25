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
        using DataVaultSetup = Dictionary<FieldProperty, Dictionary<string, List<DataField>>>;
        public interface IDataVaultConstructor 
        {
             void PreviewTable(object sender, EventArgs e);
             void Edit(object sender, EventArgs e);
             void Delete(object sender, EventArgs e);
             void UpdateEditor();
             void NewEditor(TableType type);
             void AddTable(IDataTable table);
             void NewHub(object sender, EventArgs e);
             void NewLink(object sender, EventArgs e);
             void NewSatelite(object sender, EventArgs e);
             void NewReference(object sender, EventArgs e);
             Maybe<IDataTable> GetTable(string Key, TableType type);
             EventHandler InvokeSelector(TableType t, FKEditor editor);
             void OnLeave(Object sender, EventArgs e);
             void Add(Control control);
             void AddToEditor(Control control);
             void RemoveByKey(string key);
             int Height();
             int Width();
             void Refresh();
            //private void InitializeComponent()
            //{
            //    this.SuspendLayout();
            //    // 
            //    // DataVaultConstructor
            //    // 
            //    this.ClientSize = new System.Drawing.Size(452, 407);
            //    this.Name = "DataVaultConstructor";
            //    this.ResumeLayout(false);

                
            //}

        }
        public  class SimpleEditor : Panel,IDataVaultConstructor,IMiningState
        {
            IView tables;
            IView editor;
            EventHandler onSelect=null;
            IDataVaultControl control;
            ListBox candidates;
            IDataVault model;
            public void setCandidates(ListBox lb)
            {
                candidates = lb;
            }
            public SimpleEditor()
            {
                model = new DataVault("test");
                tables = model.View(this);
                control = model.Control();

            }
            public void Add(Control control)
            {
                Controls.Add(control);
            }
            public void AddToEditor(Control control)
            {
                Controls["panelEditor"].Controls.Add(control);
            }
            public void RemoveByKey(string key)
            {
                Controls.RemoveByKey(key);
            }
            public int Height()
            {
                return base.Height;
            }
            public int Width()
            {
                return base.Width;
            }
            private  DVSetup GetSetup()
            {
                return model.Logic();
            }
            
            public  void OnLeave(Object sender, EventArgs e)
            {
                candidates.Items.Clear();
                candidates.Refresh();
                
            }
            public  EventHandler InvokeSelector(TableType t, FKEditor editor)
            {
                return (o,e) =>{
                    if (onSelect != null)
                        candidates.SelectedIndexChanged -= onSelect;
                    candidates.Items.Clear();
                    candidates.Items.AddRange(control.GetTables(t).ToArray());
                    editor.getTable().Do(table => candidates.SetSelected(candidates.Items.IndexOf(table),true));
                    onSelect = (o2, e2) =>
                    {
                        editor.setTable(candidates.SelectedItem as IDataTable);
                    };
                    candidates.SelectedIndexChanged += onSelect;
                    //candidates.Refresh();
                };
            }
            public  void Delete(object sender, EventArgs e)
            {
                try
                {
                    Button clicked = sender as Button;
                    TableControls edits = clicked.Parent.Parent as TableControls;
                    Action<IDataTable> delete = (t) =>
                    {
                        if (control.TryRemove(t.ToString()))
                        {
                            tables.Renew();
                            edits.Refresh();
                        }
                    };
                    edits.Table.Do(delete);
                }
                catch (InvalidCastException x) { };

            }
            public  void PreviewTable(object sender, EventArgs e)
            {
                Label table = sender as Label;
                control.GetTable(table.Text).Do(t => t.Preview(this));
                this.Refresh();
            }
            public  void Edit(object sender, EventArgs e)
            {
                Label table = sender as Label;
                control.GetTable(table.Text).Do(t => editor = t.Editor(this));
                this.Refresh();
            }
            public  void Refresh()
            {
                base.Refresh();
                tables.Renew();
            }
            public  void UpdateEditor()
            {
                editor.Renew();
            }
            public  void NewEditor(TableType type)
            {
                Controls.RemoveByKey("panelEditor");
                editor = new TableEditor(type, this);
            }
            public  void AddTable(IDataTable table)
            {
                control.Add(table);
                Controls.RemoveByKey("panelEditor");
                editor = new TableEditor(table, this);
                this.Refresh();
            }
            public  Maybe<IDataTable> GetTable(string key, TableType type)
            {
                Maybe<IDataTable> result = control.GetTable(key, type);
                result.Do(t => { }, () => MessageBox.Show("Неверно указана связь по ключам!"));
                return result;
            }
            public  void NewHub(object sender, EventArgs e)
            {
                editor = new TableEditor(TableType.Hub, this);
                new HubControls(this).Publish();
            }
            public  void NewLink(object sender, EventArgs e)
            {
                editor = new TableEditor(TableType.Link, this);
                new LinkControls(this).Publish();
            }
            public  void NewSatelite(object sender, EventArgs e)
            {
                editor = new TableEditor(TableType.Satelite, this);
                new SateliteControls(this).Publish();
            }
            public  void NewReference(object sender, EventArgs e)
            {
                editor = new TableEditor(TableType.Reference, this);
                new ReferenceControls(this).Publish();
            }
            public IMiningState Next()
            {
                if (!control.IsConnected())
                    throw new ArgumentException("DataBase is not Linked properly by keys!");
                ProcessLogic logic = new ProcessLogic(GetSetup());
                LogicView logicView = new LogicView(logic);
                logicView.Size = Size;
                logicView.Location = Location;
                return logicView;
            }
            public new Control Handle()
            {
                return this;
            }
            public bool IsEnd()
            { return false; }
            public bool HideControls()
            { return false; }
            public Stages Stage()
            {
                return Stages.dataVault;
            }
        }
    }

