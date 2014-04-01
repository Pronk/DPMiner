using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace DPMiner
{   
	
	public interface ProcessMiner
	{
	 IPetryNet Mine(int[][] log);

    }
	public class AlphaMiner
	{
		private int size;
		private bool[,] relationship = null;
		private Relationships[,] relations = null;
        private List<Tuple<int, int>> followers = new List<Tuple<int, int>>();
		public AlphaMiner (int size)
		{
			this.size = size;
			relationship = new bool[size,size];
			relations = new Relationship[size, size];
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
		private void DigRelation(Relationships relation, Func<int,int, bool> predicat)
		{
			for(int i=0; i<size; i++)
				for(int j=i; j<size; j++)
					if(predicat(i,j))
						Relationshipsp[i,j] = relation;
		}
	
	}
}
