using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace DataVault
{
    class RoleSelector
    {
        HashSet<FieldProperty> selected = new HashSet<FieldProperty>();
        public HashSet<FieldProperty> Selected { get { return selected; } }
        public RoleSelector(HashSet<FieldProperty> locked, HashSet<FieldProperty> blocked, Control parent)
        {
            int offset = 140;
            Button button;
            foreach(FieldProperty fp in Enum.GetValues(typeof(FieldProperty)) )
            {
                offset += 40;
                button = new Button();
                parent.Controls.Add(button);
                button.Location = new Point(offset, 1);
                button.Size = new Size(40, 20);
                button.Text = fp.ToString();
                if (locked.Contains(fp))
                {
                    button.BackColor = Color.Orange;
                    continue;
                    
                }
                if(blocked.Contains(fp))
                {
                    button.BackColor = Color.Gray;
                    continue;
                }
                button.BackColor = Color.Beige;
                button.Click += new EventHandler(Use(fp));
                    
            }
            selected = locked;
        }
        public RoleSelector(HashSet<FieldProperty> locked, HashSet<FieldProperty> blocked, HashSet<FieldProperty> selected, Control parent)
        {
            int offset = 140;
            Button button;
            foreach (FieldProperty fp in Enum.GetValues(typeof(FieldProperty)))
            {
                offset += 40;
                button = new Button();
                parent.Controls.Add(button);
                button.Location = new Point(offset, 1);
                button.Size = new Size(40, 20);
                button.Text = fp.ToString();
                if (locked.Contains(fp))
                {
                    button.BackColor = Color.Orange;
                    continue;
                }
                if (blocked.Contains(fp))
                {
                    button.BackColor = Color.Gray;
                    continue;
                }
                if (selected.Contains(fp))
                {
                    button.BackColor = Color.Azure;
                    button.Click += new EventHandler(Use(fp));
                    continue;
                }
                button.BackColor = Color.Beige;
                
                button.Click += new EventHandler(Use(fp));

            }
            this.selected = new HashSet<FieldProperty>( locked.Union(selected));
        }
        private Action<object,EventArgs> Use(FieldProperty fp)
        {
            Action<object, EventArgs> action = (o, e) =>
            {
                Control sender = o as Control;
                if (selected.Contains(fp))
                {
                    sender.BackColor = Color.Beige;
                    selected.Remove(fp);
                }
                else
                {
                    sender.BackColor = Color.Azure;
                    selected.Add(fp);
                }
            };
            return action;
        }
    }
}
