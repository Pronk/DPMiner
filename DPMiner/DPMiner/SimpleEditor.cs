using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Monad;

namespace DPMiner
{
   
        public abstract class DataVaultConstructor : Panel
        {
            public abstract void PreviewTable(object sender, EventArgs e);
            public abstract void Edit(object sender, EventArgs e);
            public abstract void Delete(object sender, EventArgs e);
            public abstract void UpdateEditor();
            public abstract void NewEditor(TableType type);
            public abstract void AddTable(IDataTable table);
            public abstract void NewHub(object sender, EventArgs e);
            public abstract void NewLink(object sender, EventArgs e);
            public abstract void NewSatelite(object sender, EventArgs e);
            public abstract void NewReference(object sender, EventArgs e);
            public abstract Maybe<IDataTable> GetTable(string Key, TableType type);
            private void InitializeComponent()
            {
                this.SuspendLayout();
                // 
                // DataVaultConstructor
                // 
                this.ClientSize = new System.Drawing.Size(452, 407);
                this.Name = "DataVaultConstructor";
                this.ResumeLayout(false);

            }

        }
        public partial class SimpleEditor : DataVaultConstructor
        {
            IView tables;
            IView editor;
            IDataVaultControl control;
            public SimpleEditor()
            {
                IDataVault model = new DataVault("test");
                tables = model.View(this);
                control = model.Control();
            }

            private void button1_Click(object sender, EventArgs e)
            {
                string s = null;
                control.Add(new Hub("table", "a", "b", s, s));
                tables.Renew();
            }
            public override void Delete(object sender, EventArgs e)
            {
                try
                {
                    Button clicked = sender as Button;
                    TableControls edits = clicked.Parent as TableControls;
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
            public override void PreviewTable(object sender, EventArgs e)
            {
                Label table = sender as Label;
                control.GetTable(table.Text).Do(t => t.Preview(this));
                this.Refresh();
            }
            public override void Edit(object sender, EventArgs e)
            {
                Label table = sender as Label;
                control.GetTable(table.Text).Do(t => editor = t.Editor(this));
                this.Refresh();
            }
            public override void Refresh()
            {
                base.Refresh();
                tables.Renew();
            }
            public override void UpdateEditor()
            {
                editor.Renew();
            }
            public override void NewEditor(TableType type)
            {
                Controls.RemoveByKey("panelEditor");
                editor = new TableEditor(type, this);
            }
            public override void AddTable(IDataTable table)
            {
                control.Add(table);
                Controls.RemoveByKey("panelEditor");
                editor = new TableEditor(table, this);
                this.Refresh();
            }
            public override Maybe<IDataTable> GetTable(string key, TableType type)
            {
                Maybe<IDataTable> result = control.GetTable(key, type);
                result.Do(t => { }, () => MessageBox.Show("Неверно указана связь по ключам!"));
                return result;
            }
            public override void NewHub(object sender, EventArgs e)
            {
                editor = new TableEditor(TableType.Hub, this);
                new HubControls(this).Publish();
            }
            public override void NewLink(object sender, EventArgs e)
            {
                editor = new TableEditor(TableType.Link, this);
                new LinkControls(this).Publish();
            }
            public override void NewSatelite(object sender, EventArgs e)
            {
                editor = new TableEditor(TableType.Satelite, this);
                new SateliteControls(this).Publish();
            }
            public override void NewReference(object sender, EventArgs e)
            {
                editor = new TableEditor(TableType.Reference, this);
                new ReferenceControls(this).Publish();
            }
        }
    }

