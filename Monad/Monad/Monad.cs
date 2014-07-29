using System;
using System.Threading.Tasks;

namespace Monad
{
	//abstract class for representation of Monad object
	public abstract class Monad<t>
	{
		
		t item;// Bound element of the Base type
		
		protected Monad (t item)
		{
			this.item = item;
		}
	        protected Monad ()
	        {
	
	        }
	         
	        //Access the Bound value
	        protected virtual t Load ()
		{
			return this.item;
		}
		// The binding operator
		public abstract Monad<y> Transform<y> (Func<t,y> function);
		
        
	}
	//Represents an Output of function with Domain incomplete towards possible function inputs or unrelyble computational nature
	public class Maybe<t>:Monad<t>
	{
		//empty constructor for inheritance	
	        protected Maybe ()
	        {
	            if (!(this is None<t>))
	                throw new InvalidOperationException("This is Nothing!");
	        }
	        //
	        protected Maybe (t item):base(item){}
	        //the return operator
	        public static Maybe<t> Something(t something)
	        {
	            return new Maybe<t>(something);
	        }
		public static Maybe<t> None ()
		{
            
			return new None<t>();
		}
		//The binding operator
		public override Monad<y> Transform<y> (Func<t, Maybe<y>> function)
		{
		     if( this is None<t> )
				return Maybe<y>.None();
			return function(Load());
		}
		//Fires an action if input was aquared
		public override void Do (Action<t> action)
		{
			if(!(this is None<t>))
				action(Load());
		}
		//retrives bound value by a functioinal transformation
		public override y FinalTransform<y> (Func<t, y> function,y otherwise)
		{
			if( this is None<t> )
				return otherwise;
			return function(Load());
		}
		//Branching operator dependant on existance of the bound interval
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
	 // Represents an empty output (to be used instead of null)
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
	//  Represent image of a functions with two possible output types
	public class Or<l,r>
	{
		private object load; // bound value
		private bool right; // shows the actual return type
		private Or(object load,bool right )
		{
			this.load = load;
			this.right = right;
		}
		// Return operator for a left Type Statement
		public static  Or<l,r> Left(l member)
		{
			return new Or(member, false);	
		}
		//Return operator for a right Type statement
		public static  Or<l,r> Right(r member)
		{
			return new Or(member, true);	
		}
		//Retrives bound variable wuth a functional transformation
		public y Elimination<y> (Func<l,y> f,Func<r,y> g)
		{
			if(right)
				return g(load as r);
			return f(load as l);
		}
		//The binding operator
		public Or<x,y> Transform<x,y> (Func<l,x> f, Func<r,y> g)
		{
			if(right)
				return Or<x,y>.Right(g(load as r));
			return Or<x, y>.Left(f(load as l));
		}
		//Fires one of the two actions dependant on the value type
		public void Do (Action<l> a, Action<r> b)
		{
			if(right)
				b(load as r);
			else
				a(load as l);
		}
		//returns the true type of Bound variable
		public Type Type ()
		{
			if(right)
				return typeof(r);
			return typeof(l);
		}
		
	}
	//represents a finite list of objects with type "t"
	public class Finite<t>:FList
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
		//Projects a subset of natural numbers over space of "t"
		public static Finite<t> Project(uint size,uint start Func<uint,t> f)
		{
			if(start < size)
				throw new Exception("wrong!");
			if(size == start)
				return new Finite(start, null, f(start));
			return new Finite(start,Project(size,++start,f),f(start));
		}
		//Returns the firs value in list
		public t Head()
		{
			return item;
		}
		//Return all the lisr except the first value
		public Finite<t> Tail()
		{
			return child;
		}
		//Returns an element with certain index if it exists, otherwise returns None 
		public Maybe<t> this[uint index ]
		{
			if(index == order)
				return Maybe<t>.Something(item);
			if(index > order || child is null)
				return new Maybe<t>.None();
			reurn child[index];
		}
		//A binding function
		public  Finite<y> Transform<y>(Func<t,y> f)
		{
			if(child is null)
				return Finite(order,null,f(item));
			return Finite(order,child.Transform(f),f(item));
		}
		//Folds over a list 
		public y Fold(Func<y,t,y> f, y iter)
		{
			if(child is null)
				return f(iter,item);
			return child.Fold(f, f(iter,item));
		}
		//indexwise Fold
		public y Fold(Func<t,t,int,y> f, t iter)
		{
			if(child is null)
				return f(iter,item,order);
			return child.Fold(f, f(iter,item,order));
		}
		//Switch all indexes to be destributed in the span [0..n]
		public Finite<t> Normalize()
		{
		     
		     Func<Finite<t>, uint,Dinite<t>> f = (Finite<t> xs, uint n) =>
		     {
		     	if(xs.child is null)
		     		return new Finite<t>(n,null,xs.item);
		     	return new Finite<t>(n, f(xs.child,++n),xs.item)};
		
		     return f(this,0);
		}
		//Reverses the list
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
		public bool isEmpty()
		{
			return child is null;
		}
	
	}
	public interface FList<t>
	{
		public t Head();
		public FList<t> Tail();
		public Flist<y> Transform(Func<t,y>);
		public Maybe<t> this[int index];
		public bool isEmpty();
	}
	//Represents natural numbers
	public class N
	{
		byte[] memory;// memory to store a number
		//todo : Long arithmetics
		public byte[] Value()// returns byte representation of the number
		{
			return memory;
		}
		//Returns 0
		public N()
		{
			memory = new byte[]{0};
			
		}
		//Preserves value of number n
		public N(ulong n)
		{
			memory = BitConverter.GetBytes(n);
		}
		// takes byte representation of a natural number
		public N(byte[] memory)
		{
			this.memory = memory;
		}
		//return next number in the number order
		public N Next()
		{
			long i;
			for( i=0; i<memory.Length; i++)
				if(memory[i] != 255)
					break;
			return Next[i];
			
		}
		protected N Next(long n)
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
		// Returns preceder of the numer if it exists, otherwise return None
		public Maybe<N> Preceder()
		{
		 int i;
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
	//Represents an infinite list of  elements with type "t" 
	public class Order<t>:FList
	{
		Func<N,t> def;//The definition of the sequance
		N order;// number of current element
		//todo : Lazy evaluation
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
		//returns first element of list
		public bool isEmpty()
		{
			return false;
		}
		public t Head()
		{
			return def(order);
		}
		public Order<t> Tail()
		{
			return new Order<t> (def, order.Next());
		}
		public Order<t> Reset()
		{
			return new Order<t> ( def, new N() );
		}
		public Order<y> Transform (Func<t,y> f)
		{
			return new Order<t> ((n)=>f(def(n)), order)
		}
		public t this[N index]
		{
			return def(index);
		}
		public Maybe<t> this[int index]
		{
			return this[new N(index)];
		}
		public y PrRecursion<y>(Func<y,t,y> f, N start, y iter )
		{
			iter2 = f(iter,def(start));
			return start.Preceder().FinalTransform<y>(n => PrRecursion(f,n,iter2), ()=> iter2);
		}
		public y PrRecursion<y>(Func<y,t,y> f, N start, y iter, Func<y,bool> breaker )
		{
			iter2 = f(iter,def(start));
			if(breaker(iter))
				return iter;
			return start.Preceder().FinalTransform<y>(n => PrRecursion(f,n,iter2), ()=> iter2);
		}
		
		
	}
}
