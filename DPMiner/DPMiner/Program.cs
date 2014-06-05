using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Monad;


namespace DPMiner
{
    
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
            Application.Run(new FormMain());

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
                        mat[i, 0] = mat[i, 0];
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
                {
                    b = true;
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
                   
                }
                
                return true;
            }
            public static string Typename(Type t)
            {
                return t.ToString().Split(new char[] { '.' })[1];
            }
           
        }
    }
}
    

