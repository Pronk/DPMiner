using System;
using System.Threading.Tasks;

namespace Monad
{
	public abstract class Monad<t>
	{
		t item; 
		public Monad (t item)
		{
			this.item = item;
		}
        public Monad ()
        {

        }
        public virtual t Load ()
		{
			return this.item;
		}
		public abstract Monad<y> Transform<y> (Func<t,y> function);
		public abstract void SideEffect ( Action<t> action);
		public abstract void SideEffect (Action<t> action, Action otherwise);
		public abstract  y  FinalTransform<y> (Func<t,y> function, y otherwise);
        
	}
	public class Maybe<t>:Monad<t>
	{
		
        public Maybe ()
        {
            if (!(this is None<t>))
                throw new InvalidOperationException("This is Nothing!");
        }
        public Maybe (t item):base(item){}
 
		public static Maybe<t> None ()
		{
            
			return new None<t>();
		}
		public override Monad<y> Transform<y> (Func<t, y> function)
		{
		     if( this is None<t> )
				return Maybe<y>.None();
			return new Maybe<y>(function(Load()));
		}
		public override void SideEffect (Action<t> action)
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
		public override void SideEffect (Action<t> action, Action otherwise)
		{
			if(this is None<t>)
				otherwise();
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
	 public sealed class None<t>:Maybe<t>
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
			Maybe<int> m = new Maybe<int>(1);
			Maybe<object> o = new Maybe<object>(null);
            o = Maybe<object>.None();
            Console.WriteLine( m + "\n");
            Console.WriteLine(o + "\n");
            Console.ReadLine();

		}
	}
}
