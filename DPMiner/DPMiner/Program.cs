using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Monad;


namespace DPMiner
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
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
        public static class Util
        {
            public static Tuple<int, int> MeasureSize(int[,] mat)
            {
                int n = 0, m = 0;
                try
                {
                    for (int i = 0; true; i++)
                    {
                        n = i;
                        mat[0, 0] = mat[i, 0];
                    }
                }
                catch (Exception) { }

                m = mat.Length / n;

                return new Tuple<int, int>(n, m);
            }

            public static bool IsContained<T>(IEnumerable<T> container, IEnumerable<T> contained) where T : IEquatable<T>
            {
                bool b = true;
                foreach (T el1 in contained)
                    foreach (T el2 in container)
                    {
                        if (el1.Equals(el2))
                        {
                            b = false;
                            break;
                        }
                    }
                if (b)
                    return false;
                return true;
            }
            public static string Typename(Type t)
            {
                return t.ToString().Split(new char[] { '.' })[1];
            }
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
        }
    }
}
    

