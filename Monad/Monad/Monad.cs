using System;
using System.Threading.Tasks;

namespace Monad
{
	public abstract class Monad<t>
	{
		t item; 
		protected Monad (t item)
		{
			this.item = item;
		}
        protected Monad ()
        {

        }
        public virtual t Load ()
		{
			return this.item;
		}
		public abstract Monad<y> Transform<y> (Func<t,Maybe<y>> function);
		public abstract void Do ( Action<t> action);
		public abstract void Do (Action<t> action, Action otherwise);
		public abstract  y  FinalTransform<y> (Func<t,y> function, y otherwise);
        
	}
	public class Maybe<t>:Monad<t>программирование
	{
		
        protected Maybe ()
        {
            if (!(this is None<t>))
                throw new InvalidOperationException("This is Nothing!");
        }
        protected Maybe (t item):base(item){}
        public static Maybe<t> Something(t something)
        {
            return new Maybe<t>(something);
        }
		public static Maybe<t> None ()
		{
            
			return new None<t>();
		}
		public override Monad<y> Transform<y> (Func<t, Maybe<y>> function)
		{
		     if( this is None<t> )
				return Maybe<y>.None();
			return function(Load());
		}
		public override void Do (Action<t> action)
		{
			if(!(this is None<t>))
				action(Load());
		}
		public override y FinalTransform<y> (Func<t, y> function,y otherwise)
		{
			if( this is None<t> )
				return otherwise;
			return function(Load());
		}
		public override void Do (Action<t> action, Action otherwise)
		{
			if(this is None<t>)
				otherwise();
            else
			action(Load ());
		}
        public static implicit operator Maybe<t>(t val)
        {
            if (val == null)
                return new None<t>();
            return new Maybe<t>(val);
        }
        public static implicit operator t(Maybe<t> result)
        {
            if (result is None<t>)
                throw new Exception("Casting nothing to variable!");
            return result.Load();
        }
        public override string ToString()
        {
            if (this is None<t>)
                return "Nothing";
            return Load().ToString();
        } 
	}
	 internal sealed class None<t>:Maybe<t>
	{
		public None ()
		{
		}

		public override t Load ()
		{
			throw  new InvalidOperationException("This is nothing and it can't be used!");
		}
	}
	public static class Program
	{
	    static void Main(string[] args)
		{
			Maybe<int> m = Maybe<int>.Something(1);
            Maybe<int> k = Maybe<int>.None();
            m.Do(n => Console.WriteLine(n.ToString()), () => Console.WriteLine("none"));
            k.Do(n => Console.WriteLine(n.ToString()), () => Console.WriteLine("none"));
            Console.ReadLine();

		}
	}
	public class Or<l,r>
	{
		private object load;
		private bool right;
		private Or(object load,bool right )
		{
			this.load = load;
			this.right = right;
		}
		public static  Or<l,r> Left(l member)
		{
			return new Or(member, false);	
		}
		public static  Or<l,r> Right(r member)
		{
			return new Or(member, true);	
		}
		public y Elimination<y> (Func<l,y> f,Func<r,y> g)
		{
			if(right)
				return g(load as r);
			return f(load as l);
		}
		public Or<x,y> Transform<x,y> (Func<l,x> f, Func<r,y> g)
		{
			if(right)
				return Or<x,y>.Right(g(load as r));
			return Or<x, y>.Left(f(load as l));
		}
		public void Do (Action<l> a, Action<r> b)
		{
			if(right)
				b(load as r);
			else
				a(load as l);
		}
		public Type Type ()
		{
			if(right)
				return typeof(r);
			return typeof(l);
		}
		
	}
	
}
