using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using DPMiner;


namespace Test
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Diagonal()
        {
            //arrange
            AlphaMiner miner = new AlphaMiner(4);
            int[][] log = new int[][] { new int[] { 0, 0, 0, 0 }, new int[] { 1, 1, 1, 1, 1 }, new int[] { 2, 2 }, new int[] { 3 },new int[]{}, };
            bool b =true;
            //act
            bool[,] m = miner.GetTester().TestMatrix1(log);
            for(int i =0;i<3;i++)
                b =b && m[i,i];
            //assert
            Assert.AreEqual(true, b);
        }
        
        [TestMethod]
        public void Exception()
        {
            // arrange
            int[][] log = new int[][] {new int[] {2,3}};
            AlphaMiner.TesterOfMiner tester = (new AlphaMiner(2)).GetTester();
            bool b = false;
            //act
            try
            {
                tester.TestMatrix1(log);
            }
            catch (ArgumentException) { b = true; }
            Assert.IsTrue(b);
        }
    }
}
