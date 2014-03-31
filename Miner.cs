using System;
using System.Text;
using System.Collection;
using System.Collection.Array;

namespace DPMiner
{   
	public interface PetryNet
	{

	}
	public interface ProcessMiner
	{
	 PetryNet Mine(int[][] log);

    }
	public class AlphaMiner
	{
		private int size;
		private bool[,] relationship = null;
		private List<Tuple<int,int>> preceders = new List<Tuple<int,int>>();
		private List<Tuple<int,int>> parallels = new List<Tuple<int,int>>();
		private List<Tuple<int,int>> choices = new List<Tuple<int,int>>();
		public AlphaMiner (int size)
		{
			this.size = size;
			relationship = new bool[size,size];
			for(int i=0; i<size; i++)
				for(int j=0; j<size; j++)
					relationship[i,j]=false;
		}
		private void DigRelationships (int[][] log)
		{

			foreach (int[] trace in log ) 
				for (int n=0; n < trace.Length - 1; n++) 
						relationship[trace[n],trace[n+1]]=true;
				   
		}
		private void RelationThink(List<Tuple<int,int>> rList, Func<int,int, bool> predicat)
		{
			for(int i=0; i<size; i++)
				for(int j=i+1; j<size; j++)
					if(predicat(i,j))
						rList.Add(new Tuple<int,int>(i,j));
		}
	
	}
}
