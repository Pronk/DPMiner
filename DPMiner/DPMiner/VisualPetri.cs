using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Petri
{
    public interface IVisualSet
    {
         void Draw(Graphics canvus);
         bool Select(Point point);
         void Move(Point point);
         void Update();
         Int32 Code();
    }
    public abstract class Drawable
    {
        
        public abstract void Draw(Graphics canvus);
        public abstract bool IsCaught(Point point);
        protected int x;
        protected int y;
        public virtual Point Location
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
        public override bool IsCaught(Point p)
        {
            return (p.X - x <= 64) && (p.Y - y <= 64);
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
        public override bool IsCaught(Point p)
        {
            return (p.X - x <= 25 && p.X -x > -5) && (p.Y - y <= 65 && p.Y - y > -5);
        }
        public override void Draw(Graphics canvus)
        {
            canvus.DrawRectangle(new Pen(Color.Black, 6), x, y, 20, 60);
            Font myFont = new Font("Helvetica", 15);
            canvus.DrawString(name, myFont, new SolidBrush(Color.Black), new PointF(x - (name.Length - 1) * 5, y - 26));
            
        }
    }
    public class Arrow:Drawable
    {
        int weight;
        Node node;
        Transition trans;
        public Arrow(Node node, Transition trans, int weight)
        {
            this.weight = weight;
            this.node = node;
            this.trans = trans;
        }
        public override bool IsCaught(Point point)
        {
            return false;
        }
        public override void Draw(Graphics canvus)
        {
            PointF begin;
            PointF end;
            PointF scnd;
            PointF third;
            bool toNode = node.Location.X > trans.Location.X;
            if (toNode)
            {
                begin = new PointF(node.Location.X - 2, node.Location.Y + 32);
                end = new PointF(trans.Location.X + 22, trans.Location.Y + 30);
                scnd = new PointF((float)(node.Location.X - Coef() * (node.Location.X - trans.Location.X)), node.Location.Y + 32);
                third = new PointF((float)(trans.Location.X + Coef() * (node.Location.X - trans.Location.X)), trans.Location.Y + 30);
            }
            else
            {
                begin = new Point(node.Location.X + 66, node.Location.Y + 32);
                end = new Point(trans.Location.X - 2, trans.Location.Y + 30);
                scnd = new PointF((float)(node.Location.X - Coef() * (node.Location.X - trans.Location.X)), node.Location.Y + 32);
                third = new PointF((float)(trans.Location.X + Coef() * (node.Location.X - trans.Location.X)), trans.Location.Y + 30);
            }
            canvus.DrawBezier(new Pen(Color.Black, 4), begin, scnd, third, end);
            if (toNode && weight > 0)
            {
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(begin.X - 15, begin.Y + 5), new PointF(begin.X - 2, begin.Y));
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(begin.X - 15, begin.Y - 5), new PointF(begin.X - 2, begin.Y));
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(begin.X - 12, begin.Y - 3), new PointF(begin.X - 12, begin.Y + 3));
                if (weight > 1)
                    canvus.DrawString(weight.ToString(), new Font("Impact", 15, FontStyle.Italic), new SolidBrush(Color.Blue), new PointF(end.X + (float)0.1 * (1 / Coef()) * Location.X, end.Y + (float)0.1 * (1 / Coef()) * Location.Y));
                return;
            }
            if (weight > 0)
            {
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(begin.X + 15, begin.Y + 5), new PointF(begin.X + 2, begin.Y));
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(begin.X + 15, begin.Y - 5), new PointF(begin.X + 2, begin.Y));
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(begin.X + 12, begin.Y - 3), new PointF(begin.X + 12, begin.Y + 3));
                if (weight > 1)
                    canvus.DrawString(weight.ToString(), new Font("Impact", 15, FontStyle.Italic), new SolidBrush(Color.Blue), new PointF(end.X + (float)0.1 * (1 / Coef()) * Location.X, end.Y + (float)0.1 * (1 / Coef()) * Location.Y));
                return;
            }
            if (toNode)
            {
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(end.X + 15, end.Y + 5), new PointF(end.X + 2, end.Y));
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(end.X + 15, end.Y - 5), new PointF(end.X + 2, end.Y));
                canvus.DrawLine(new Pen(Color.Black, 4), new PointF(end.X + 12, end.Y - 3), new PointF(end.X + 12, end.Y + 3));
                if (weight < -1)
                    canvus.DrawString((-weight).ToString(), new Font("Impact", 15, FontStyle.Italic), new SolidBrush(Color.Blue), new PointF(begin.X - (float)0.1 * (1 / Coef()) * Location.X, begin.Y - (float)0.1 * (1 / Coef()) * Location.Y));
                return;
            }
            canvus.DrawLine(new Pen(Color.Black, 4), new PointF(end.X - 15, end.Y + 5), new PointF(end.X - 2, end.Y));
            canvus.DrawLine(new Pen(Color.Black, 4), new PointF(end.X - 15, end.Y - 5), new PointF(end.X - 2, end.Y));
            canvus.DrawLine(new Pen(Color.Black, 4), new PointF(end.X - 12, end.Y - 3), new PointF(end.X - 12, end.Y + 3));
            if (weight < -1)
                canvus.DrawString((-weight).ToString(), new Font("Impact", 15, FontStyle.Italic), new SolidBrush(Color.Blue), new PointF(begin.X - (float)0.1 * (1 / Coef()) * Location.X, begin.Y - (float)0.1 * (1 / Coef()) * Location.Y));
            return;
        }
        protected float Distance(Point a, Point b)
        {
            return (float)(Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2)));
        }

        protected float Coef()
        {
            float x = Math.Abs(node.Location.X - trans.Location.X);
            float y = Math.Abs(node.Location.Y - trans.Location.Y);
            float d = Distance(node.Location, trans.Location);
            if (d == 0.0)
                return 0;
            if (x > y)
                return y / d;
            else
                return x / d;


        }
        public override Point Location
        {
            get { return new Point(node.Location.X - trans.Location.X, node.Location.Y - trans.Location.Y); }
            set { }
        }
        }
    }

