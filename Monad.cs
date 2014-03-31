using System;

namespace Monad
{
	public abstract class Monad<t>
	{
		t item; 
		public Monad (t item)
		{
			this.item = item;
		}
        public virtual t Load ()
		{
			return this.item;
		}
		public abstract Monad<y> Transform (Func<y,t> function);
		public abstract void SideEffect ( Action<t> action);
		public abstract void SideEffect (Action<t> action, Action otherwise);
		public abstract  y  FinalTransform (Func<y,t> function, y otherwise);
		
	}
	public class Maybe<t>:Monad<t>
	{
		public Maybe (t item):base(item)
		{
		  if (item = null)
				this = Maybe<t>.None();
		}
		public static Maybe<t> None ()
		{
			return new None<t>();
		}
		public override Monad<y> Transform (Func<y, t> function)
		{
		     if( this is None<t> )
				return Maybe<y>.None();
			return new Maybe<t>(function(Load ()));
		}
		public override void SideEffect (Action<t> action)
		{
			if(!(this is None<t>))
				action(load);
		}
		public override y FinalTransform (Func<y, t> function, y otherwise)
		{
			if( this is None<t> )
				return otherwise;
			return function(Load ());
		}
		public override void SideEffect (Action<t> action, Action otherwise)
		{
			if(this is None<t>)
				otherwise();
			action(Load ());
		}
	}
	public class None<t>:Maybe<t>
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
	    static void main(string[] args)
		{
			Maybe<int> m = new Maybe<int>(1);
			Maybe<object> o = new Maybe<object>(null);

		}
	}
}
