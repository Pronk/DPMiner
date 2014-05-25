using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace DPMiner
{
    class VisualPetri
    {
       
    }
    public abstract class Drawable
    {
        public abstract void Draw(Graphics canvus);
        protected int x;
        protected int y;
         public Point Location
        {
            get { return new Point(x, y); }
            set { x = value.X; y = value.Y;}
        }
    }

    public class Node:Drawable
    {
        
        int markup;
        public int Markup
        {
            get { return markup; }
            set { markup = value; }
        }
       
        public Node (int x, int y, int markup )
        {
            this.x = x;
            this.y = y;
            this.markup = markup;
        }
        public override void Draw(Graphics canvus)
        {
            Brush brush = new SolidBrush(Color.Coral);
            canvus.FillEllipse(brush, x, y, 64, 64);
            if (markup < 5)
            {

                if (markup == 0)
                    return;
                brush = new SolidBrush(Color.Black);
                if (markup == 1)
                {
                    canvus.FillEllipse(brush, new Rectangle(x + 16, y + 16, 32, 32));
                    return;
                }
                if (markup == 2)
                {
                    canvus.FillEllipse(brush, new Rectangle(x + 20, y + 36, 24, 24));
                    canvus.FillEllipse(brush, new Rectangle(x + 20, y + 4, 24, 24));
                    return;
                }
                if (markup == 3)
                {
                    canvus.FillEllipse(brush, new Rectangle(x + 8, y + 24, 20, 20));
                    canvus.FillEllipse(brush, new Rectangle(x + 28, y + 8, 20, 20));
                    canvus.FillEllipse(brush, new Rectangle(x + 28, y + 40, 20, 20));
                    return;
                }
                if (markup == 4)
                {
                    canvus.FillEllipse(brush, new Rectangle(x + 34, y + 36, 16, 16));
                    canvus.FillEllipse(brush, new Rectangle(x + 34, y + 16, 16, 16));
                    canvus.FillEllipse(brush, new Rectangle(x + 16, y + 36, 16, 16));
                    canvus.FillEllipse(brush, new Rectangle(x + 16, y + 16, 16, 16));
                    return;
                }
            }
            else
            {
                Font myFont = new Font("Helvetica", 20, FontStyle.Italic);
                brush = new SolidBrush(Color.Black);
                canvus.DrawString(markup.ToString(), myFont, brush, new PointF(x + 18, y + 18));
            }
        }
    }
    public class Transition : Drawable
    {
        string name;
        public Transition (int x, int y, string name)
        {
            this.x = x;
            this.y = y;
            this.name = name;
        }
        public override void Draw(Graphics canvus)
        {
            canvus.DrawRectangle(new Pen(Color.Black), x, y, 40, 80);
            Font myFont = new Font("Helvetica", 20);
            canvus.DrawString(name, myFont, new SolidBrush(Color.Black), new PointF(x - (name.Length - 1)*5, y - 22));
        }
    }
}
