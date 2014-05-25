using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
       public static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static Random Local;

            public static Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }
        public static class Extensions
        {
             
              public static void Shuffle<T>(this IList<T> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
            public static IList<T> CloneWithout<T>(this IList<T> list,T a)where T : IEquatable<T>
            {
                List<T> clone = new List<T>();
                foreach(T x in list)
                    if(!x.Equals(a))
                        clone.Add(a);
                return clone;
            }
            public static IList<T> Combinations<T>(this IList<T> list)where T : IEquatable<T>
            {
                if(list.Count() < 2)
                    return list;
                else
                {
                    List<T> newList = new List<T>();
                    foreach(T x in list)
                    {
                        newList.Add(x);
                        newList.AddRange(list.CloneWithout(x).Combinations());
                    }
                    return newList;
                }
                
            }
            
        }
}
