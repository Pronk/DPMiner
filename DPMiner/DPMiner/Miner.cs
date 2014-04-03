using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace DPMiner
{   
	
	public interface ProcessMiner
	{
	 IPetriNet Mine(int[][] log);

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
			relations = new Relationships[size, size];
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
		private void DigRelation()
		{
			for(int i=0; i<size; i++)
				for(int j=i; j<size; j++)
				{
					if(relationship[i,j]&&!relationship[j,i])
						relations[i,j] = Relationships.preceder;
					if(!relationship[i,j]&&relationship[j,i])
						relations[i,j] = Relationships.descend;
					if(relationship[i,j]&&relationship[j,i])
						relations[i,j] = Relationships.parallel;
					else
						relations[i,j] = Relationships.choice;
				}
		}
		private List<Tuple<List<int>,List<int>>> GetMoves()
		{
            List<Tuple<List<int>, List<int>>> pairs = new List<Tuple<List<int>, List<int>>>();
            List<List<int>> independs = GetAllSuchThat(size, (set, j) => Enumerable.All<int>(set, i => relations[i, j] == Relationships.choice));
            foreach(List<int> set in independs)
            {
                List<List<int>> descends = GetAllSuchThat(size, GetDescentsChecker(set));
                foreach(List<int> descend in descends)
                    pairs.Add(new Tuple<List<int>,List<int>>(set,descend));
            }
            pairs = RemoveSmallOnes(pairs);
            return pairs;
	         
		}

        private List<Tuple<List<int>, List<int>>> RemoveSmallOnes(List<Tuple<List<int>, List<int>>> pairs)
        {
            for(int i =0; i<pairs.Count; i++)
                for(int j =0; j<pairs.Count; i++)
                    if(i!=j)
                        if(Program.Util.IsContained<int>(pairs[i].Item1,pairs[j].Item1)&&Program.Util.IsContained<int>(pairs[i].Item2,pairs[j].Item2))
                        {
                            pairs.RemoveAt(j);
                            if (i > j)
                                i--;
                            j--;
                        }
            return pairs;
                   
        }
        private List<List<int>> GetAllSuchThat(int limit, Func<List<int>,int,bool> predicat) 
        {    
            List<List<int>> combinations=new List<List<int>>();
            for(int i=0; i<limit;i++)
            {
                List<int> set = new List<int>();
                set.Add(i);
                for (int next = i + 1; next < limit; next++)
                {
                    for (int j = next; j < limit; j++)
                        if (predicat(set, j))
                            set.Add(j);
                    if (!combinations.Contains(set) && set.Count != 0)
                        combinations.Add(set);
                }
            }
            return combinations;
        
        }
        private Func<List<int>,int,bool> GetDescentsChecker(List<int> set)
        {
            return (newSet, j) =>
            {
                if (!Enumerable.All<int>(newSet, i => relations[i, j] == Relationships.choice))
                    return false;
                
                Func<int, bool> f = i =>
                {
                    if (i <= j)
                        return relations[i, j] == Relationships.preceder;
                    else
                        return relations[j, i] == Relationships.descend;
                };
                return Enumerable.All<int>(set, f);

            };
        }
        
	        public IPetriNet Mine( int[][] log)
	        {
	        	int first;
	        	int last;
	        	int places;
	        	List<int> firsts = new List<int>();
	        	List<int> lasts = new List<int>();
	        	foreach(int[] trace in log)
	        	{
	                        first =  trace[0];
	                        last = trace[trace.Length()];
	                        if(!firsts.Contain(first))
	                        	firsts.Add(first)
	                        if(!lasts.Contain(last))
	                        	Add.last()
	        	}
	        	DigRelationships (log);
	        	DigRelation();
                	List<Tuple<List<int>, List<int>>> moves = GetMoves();
                        places = moves.Count() + 2;
                        int[,] transitions = new int[size,places];
                        foreach(int first in firsts)
                        	transitions[first,0] = -1;
                        foreach(int last in lasts)
                        	transitions[last, places - 1 ]=1
                        for(int j=1; j<places-1;j++)
                         {
                           List[int] sources = moves[j-1].Item1();
                           List[int] destanations = moves[j-1].Item2();
                           foreach(int source in sources)
                           	transition[source, j] = -1;
                           foreach(int dest in destanations)
                           	transition[dest, j] = 1;
                         }
                         PetriNet pn = new PetriNet(places, size);
                         pn.SetTransitions(transitions).SideEffect(p => pn=p, => throw new Exception("Incorrect sizr""));
                         return pn;
	   	}
	}
}
