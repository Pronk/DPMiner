using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad;
using DPMiner;
using System.Drawing;

namespace Petri
{
    public interface IPetriNet
    {
        IPetriControl GetControls();
        IVisualSet GetView(Alphabeth alpha);
    }
    public interface  IPetriControl
    {
        bool TryFire(int id);
        bool Act();
      int[] Firable();

    }
    class PetriNet : IPetriNet, IEquatable<PetriNet>
    {

        int places;
        int[] markup;
        int tc;
        int[,] transitions;

        // naive compare
        public bool Equals(PetriNet that)
        {
            if (this.tc != that.tc)
                return false;
            if (this.places != that.places)
                return false;
            for (int i = 0; i < tc; i++)
                for (int j = 0; j < places; j++)
                    if (this.transitions[i, j] != that.transitions[i, j])
                        return false;
            return true;
        }
        public PetriNet(int places, int tc)
        {
            if (places < 0 || tc < 0)
                throw new ArgumentException("numbers must be positive!");
            this.places = places;
            this.tc = tc;
            markup = new int[places];
            transitions = new int[tc, places];
        }

        public Maybe<PetriNet> setMarkup(int[] markup)
        {
            if (markup == null)
                throw new ArgumentNullException();
            if (markup.Length != places)
                return Maybe<PetriNet>.None();
            this.markup = markup;
            return this;
        }
        public Maybe<PetriNet> setTransitions(int[,] transitions)
        {
            if (transitions == null)
                throw new ArgumentNullException();
            Tuple<int, int> size = DPMiner.Program.Util.MeasureSize(transitions);
            if (size.Item2 != places || size.Item1 != tc)
                return Maybe<PetriNet>.None();
            this.transitions = transitions;
            return this;
        }

        private void Fire(int transID)
        {
            for (int i = 0; i < places; i++)
                markup[i] += transitions[transID, i];

        }
        private bool IsFirable(int transID)
        {
            for (int i = 0; i < places; i++)
                if (transitions[transID, i] < 0 && markup[i] < -transitions[transID, i])
                    return false;
            return true;
        }
        public IPetriControl GetControls()
        {
            return new PetriControl(this);
        }
        public IVisualSet GetView(Alphabeth alpha)
        {
            return new VisualPetri(this, alpha);
        }
        public class PetriControl : IPetriControl
        {
            PetriNet model;
            public PetriControl(PetriNet net)
            {
                model = net;
            }
            public int[] Firable()
            {
                List<int> firable = new List<int>();
                for (int i = 0; i < model.places; i++)
                    if (model.IsFirable(i))
                        firable.Add(i);
                return firable.ToArray();
            }
            public bool TryFire(int id)
            {
                if (model.IsFirable(id))
                {
                    model.Fire(id);
                    return true;
                }
                return false;

            }
            public bool Act()
            {
                Random gen = new Random();
                int[] actions = Firable();
                if (actions.Length == 0)
                    return false;
                model.Fire(gen.Next(0, actions.Length));
                return true;

            }

        }
        public partial class VisualPetri:IVisualSet
        {
            PetriNet model;
            Dictionary<string, Drawable> picture;
            Drawable selected;
            Alphabeth alpha;
            public VisualPetri(PetriNet model, Alphabeth alpha)
            {
                this.model = model;
                if (!(model.places == 0 || model.tc == 0))
                    PlaceNode(0, 10, 10);
            }
            public void Draw(Graphics g)
            {

                foreach (Drawable element in picture.Values)
                    element.Draw(g);
                
            }
            public void Select(Point point)
            {
                foreach (Drawable element in picture.Values)
                    if (element.IsCaught(point))
                    {
                        selected = element;
                        break;
                    }
            }
            public void Move(Point point)
            {
                if (selected != null)
                    selected.Location = point;
            }
            public void Update()
            {
                foreach (KeyValuePair<string, Drawable> pair in picture)
                    if (pair.Key[0] == 'p')
                        (pair.Value as Node).Markup = model.markup[Int32.Parse(pair.Key.Substring(1))];
            }
            protected Node PlaceNode(int n, int x, int y )
            {
                int scew = 0;
                Random gen = new Random();
                Node node = new Node(x, y, model.markup[n]);
                picture.Add("p" + n.ToString(),node);
                foreach(int t in Enumerable.Range(0,model.tc))
                {
                    int weight = model.transitions[n,t];
                    if(weight != 0)
                        if(!picture.Keys.Contains("t" + t.ToString()))
                        {
                            Transition tr = PlaceTr(t, x + Math.Sign(weight)*gen.Next(20,71),y + scew + gen.Next(0,71));
                            picture.Add("a." + "p" + n.ToString() + "t" + t.ToString(), new Arrow(node, tr, weight));
                            scew+=70;
                        }
                }
                return node;
            }
            protected Transition PlaceTr(int t, int x, int y)
            {
                int scew = 0;
                Random gen = new Random();
                Transition tr = new Transition(x, y, alpha.Decode(t));
                picture.Add("t" + t.ToString(), tr);
                foreach (int n in Enumerable.Range(0, model.places))
                {
                    int weight = model.transitions[n, t];
                    if (weight != 0)
                        if (!picture.Keys.Contains("p" + n.ToString()))
                        {
                           Node node = PlaceNode(t, x - Math.Sign(weight) * gen.Next(20, 101), y + scew + gen.Next(0, 101));
                            picture.Add("a."+"p" + n.ToString() + "t" + t.ToString(), new Arrow(node, tr, weight));
                            scew += 100;
                        }
                }
                return tr;
            }
        }
    }
    }

