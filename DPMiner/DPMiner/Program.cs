using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Application.Run(new Form1());
        }
        public static class Util
        {
            public static  Tuple<int,int> MeasureSize(int[,] mat)
            {
                int n=0, m=0;
                try
                {
                    for(int i=0; true ; i++)
                    {
                        n = i;
                        mat[0, 0] = mat[i, 0];
                    }
                }catch(Exception){}

                m = mat.Length / n;

                return new Tuple<int, int>(n, m);
            }
        }
    }
    
}
