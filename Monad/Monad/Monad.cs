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
	public class Maybe<t>:Monad<t>
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
	public class Finite<t>
	{
		t item
	`	int order;
		Finite<t> child;
		private Finite<t>(int order, Finite<t> child, t item)
		{
			this.order = order;
			this.child = child;
			this.item = item;
		}
		public static Finite<t> Project(uint size,uint start Func<uint,t> f)
		{
			if(start < size)
				throw new Exception("wrong!");
			if(size == start)
				return new Finite(start, null, f(start));
			return new Finite(start,Project(size,++start,f),f(start));
		}
		public t Head()
		{
			return item;
		}
		public Finite<t> Tail()
		{
			return child;
		}
		
		public Maybe<t> this[index uint]
		{
			if(index == order)
				return Maybe<t>.Something(item);
			if(index > order || child is null)
				return new Maybe<t>.None();
			reurn child[index];
		}
		public  Finite<y> Transform<y>(Func<t,y> f)
		{
			if(child is null)
				return Finite(order,null,f(item));
			return Finite(order,child.Transform(f),f(item));
		}
		public y Fold(Func<y,t,y> f, y iter)
		{
			if(child is null)
				return f(iter,item);
			return child.Fold(f, f(iter,item));
		}
		public y Fold(Func<t,t,int,y> f, t iter)
		{
			if(child is null)
				return f(iter,item,order);
			return child.Fold(f, f(iter,item,order));
		}
		public Finite<t> Normalize()
		{
		     
		     Func<Finite<t>, uint,Dinite<t>> f = (Finite<t> xs, uint n) =>
		     {
		     	if(xs.child is null)
		     		return new Finite<t>(n,null,xs.item);
		     	return new Finite<t>(n, f(xs.child,++n),xs.item)};
		
		     return f(this,0);
		}
		public Finite<t> Reverse()
		{
			Func<Finite<t>,Finite<t>,int,Finite<t>> f = ( xs, parent, n) =>
			{
				if(child is null)
					return new Finite<t>(n, parent, xs.item);
				return f(xs.child, new Finite<t>(n,parent,xs.item),n++);
			};
			
			return f(this,null,0);
		}
	
	}
	public class N
	{
		byte[] memory;
		public byte[] Value()
		{
			return memory;
		}
		public N()
		{
			memory = new byte[]{0};
			
		}
		public N(ulong n)
		{
			memory = BitConverter.GetBytes(n);
		}
		public N(byte[] memory)
		{
			this.memory = memory;
		}
		public N Next()
		{
			long i;
			for( i=0; i<memory.Length; i++)
				if(memory[i] != 255)
					break;
			return Next[i];
			
		}
		private N Next(long n)
		{
			if(memory.Length <= n)
			{
				byte[] memory2 = new byte[n+1];
				for(int i=0; i < n; i++)
					memory2[i] = 0;
				memory2[n] = 1;
				return new N(memory2)
			}
			else
			{
				byte[] memory2 = new byte[memory.Length];
				for(int i=0; i < n; i++)
					memory2[i] = 0;f
				memory2[n] = memory[n] + 1;
				for(int i=n+1; i < memory,length; i++)
					memory2[i] = memory[i];
				return new N(memory2)
			}
		}
		public Maybe<N> Preceder()
		{
		 if(memory.Length == 1 && memory[0]==0) 
		 	return Mayby<N>.None();
		 for( i=0; i<memory.Length; i++)
				if(memory[i] != 0)
					break;
			return Maybe<N>.Something(Prec[i]);
		}
			
	
		private N Prec(long n)
		{
			long l = memory.Length;
			if(n == l - 1)
			{
		        	byte[] memory2 = new byte[l - 1];
		        	for(long i = 0;i<l-2; i++)
		        		memory2[i] = 0;
		        	memory2[l-1] = 255;
		        	return new N(memory2);
			}
			else
			{
			    byte[] memory2 = new byte[l];
		        	for(long i = 0;i<n;i++)
		        		memory2[i] = 0
		        	memory2[n] = 255;
		        	for(long i = n+1;i<l;i++)
		        		memory2[i] = memory[i];
		        	return new N(memory2);
			}
		
		}
	}
	public class Order<t>
	{
		Func<N,t> def;
		N order;
		public Order(Func<N,t> f)
		{
		 def = f;
		 order = new N();
		}
		private Order(Func<N,t> f, N order)
		{
		 def = f;
		 this.order = order; 
		}
		public t Head()
		{
			return def(order);
		}
		public Order<t> Tail()
		{
			return new Order<t> (def, order.Next());
		}
		public Order<y> Transform (Func<t,y> f)
		{
			return new Order<t> ((n)=>f(def(n)), order)
		}
		public t  this[N index]
		{
			return def(index);
		}
	}
}
