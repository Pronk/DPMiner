using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Petri;
using Extensions;
namespace DPMiner
{   
	
	public interface ProcessMiner
	{
	 IPetriNet Mine(int[][] log);

    }
	public class AlphaMiner:ProcessMiner
	{
	private int size;
        private List<Tuple<int, int>> followers = new List<Tuple<int, int>>();
		public AlphaMiner (int size)
		{
			this.size = size;
			
		}
		private bool[,] DigRelationships (int[][] log)
		{
            try
            {
                bool[,] relationship = new bool[size, size];
                foreach (int[] trace in log)
                    for (int n = 0; n < trace.Length - 1; n++)
                        relationship[trace[n], trace[n + 1]] = true;
                return relationship;
            }
            catch (IndexOutOfRangeException) { throw new ArgumentException("Bad data formatting in log array!"); }
				   
		}
	private Relationships[,] DigRelation(bool[,] relationship)
	{
		Relationships[,] relations = new Relationships[size,size];
		for(int i=0; i<size; i++)
			for(int j=i; j<size; j++)
				{
                    if (relationship[i, j] && !relationship[j, i])
                    {
                        relations[i, j] = Relationships.preceder;
                        continue;
                    }
                    if (!relationship[i, j] && relationship[j, i])
                    {
                        relations[i, j] = Relationships.descend;
                        continue;
                    }
					if(relationship[i,j]&&relationship[j,i])
						relations[i,j] = Relationships.parallel;
					else
						relations[i,j] = Relationships.choice;
				}
			return relations;
	}
    private List<int> Range(int n)
    {
        List<int> range = new List<int>();
        foreach (int a in Enumerable.Range(0, n))
            range.Add(a);
        return range;
    }
	private List<Tuple<List<int>,List<int>>> GetMoves(Relationships[,] matrix )
	{
            	List<Tuple<List<int>, List<int>>> pairs = new List<Tuple<List<int>, List<int>>>();
        	List<HashSet<int>> independs = GetAllSuchThat((set, j) => Enumerable.All<int>(set, i => matrix[i, j] == Relationships.choice), new List<HashSet<int>>(), Range(size));
            	foreach(HashSet<int> set in independs)
            	{
                    List<HashSet<int>> descends = GetAllSuchThat(GetDescentsChecker(set.ToList(), matrix), new List<HashSet<int>>(), Range(size));
        		foreach(HashSet<int> descend in descends)
                    		pairs.Add(new Tuple<List<int>,List<int>>(set.ToList(),descend.ToList()));
            	}
            	 pairs = RemoveSmallOnes(pairs);
            	 return pairs;
	         
	}

        private List<Tuple<List<int>, List<int>>> RemoveSmallOnes(List<Tuple<List<int>, List<int>>> pairs)
        {
            for(int i =0; i<pairs.Count; i++)
                for(int j =0; j<pairs.Count; j++)
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
        private List<HashSet<int>> GetAllSuchThat( Func<List<int>,int,bool> predicat,  List<HashSet<int>> combinations, IEnumerable<int> source ) 
        {
           List<HashSet<int>> newComb;
           if(combinations.Count == 0)
           {
               foreach (int n in source)
               {
                   if (predicat(new List<int> { }, n))
                   {
                       combinations.Add(new HashSet<int>() { n });
                       newComb = (GetAllSuchThat(predicat, new List<HashSet<int>> { new HashSet<int>() { n } }, source.ToList().CloneWithout(n)));
                       foreach (HashSet<int> set in newComb)
                           if (!combinations.Contains(set))
                               combinations.Add(set);
                   }
                   
               }

               return combinations;
           }
           newComb = new List<HashSet<int>>();
           foreach(HashSet<int> x in combinations)
               foreach(int n in source)
               {
                   if(predicat(x.ToList(), n))
                   {
                       HashSet<int> newSet = new HashSet<int>();
                        foreach(int a in x)
                            newSet.Add(a);
                        newSet.Add(n);
                        newComb.Add(newSet);
                        List<HashSet<int>> iter = GetAllSuchThat(predicat,new List<HashSet<int>>(){newSet},source.ToList().CloneWithout(n).ToList());
                        newComb.AddRange(new HashSet<HashSet<int>>(iter));

                   }
               }
          return newComb;
        } 
        private Func<List<int>,int,bool> GetDescentsChecker(List<int> set, Relationships[,] matrix)
        {
            return (newSet, j) =>
            {
                if (!Enumerable.All<int>(newSet, i => matrix[i, j] == Relationships.choice))
                    return false;
                
                Func<int, bool> f = i =>
                {
                    if (i <= j)
                        return matrix[i, j] == Relationships.descend;
                    else
                        return matrix[j, i] == Relationships.preceder;
                };
                return Enumerable.All<int>(set, f);

            };
        }
        
	        public IPetriNet Mine( int[][] log)
	        {
	        	int first;
	        	int last;
	        	int places;
	        	bool[,] relation;
	        	Relationships[,] structure;
	        	List<int> firsts = new List<int>();
	        	List<int> lasts = new List<int>();
	        	foreach(int[] trace in log)
	        	{
                    if (trace.Count() == 0)
                        continue;
	                first =  trace[0];
	                last = trace[trace.Length -1];
	                if(!firsts.Contains(first))
	                    firsts.Add(first);
	                if(!lasts.Contains(last))
	                    lasts.Add(last);
	        	}
	        	relation = DigRelationships (log);
	        	structure=DigRelation(relation);
	        	relation = null;
                List<Tuple<List<int>, List<int>>> moves = GetMoves(structure);
                places = moves.Count() + 2;
                int[,] transitions = new int[size,places];
                foreach(int f in firsts)
                    transitions[f,0] = -1;
                foreach(int l in lasts)
                    transitions[l, places - 1 ]= 1;
                for(int j=1; j<places-1;j++)
                    {
                    List<int> destinations = moves[j-1].Item1;
                    List<int> sources = moves[j - 1].Item2;
                    foreach(int source in sources)
                        transitions[source, j] = 1;
                    foreach(int dest in destinations)
                        transitions[dest, j] = -1;
                    }   
                    PetriNet pn = new PetriNet(places, size);
                    pn.setTransitions(transitions).Do(p => pn=p, ex);
                    return pn;
	   	}
        private void ex()
    {
            throw new Exception("Incorrect size");
    }
	   	public class TesterOfMiner
	   	{
	   		AlphaMiner parent;
	   		public TesterOfMiner(AlphaMiner miner)
	   		{
	   		 parent = miner;
	   		}
	   		public bool[,] TestMatrix1(int[][] log)
	   		{
	   			return parent.DigRelationships(log);
	   		}
	   		public Relationships[,] TestMatrix2(bool[,] matrix)
	   		{
                return parent.DigRelation(matrix);
	   		}
            //public List<List<int>> TestSetConstructor(int limit, Func<List<int>,int,bool> predicat)
            //{
            //    return parent.GetAllSuchThat(limit, predicat);
            //}
	   		private List<Tuple<List<int>,List<int>>> TestGetMoves(Relationships[,] matrix )
	   		{
	   			return parent.GetMoves(matrix);
	   		}
	   	}
	   	public TesterOfMiner GetTester()
	   	{
	   		return new TesterOfMiner(this);
	   	}
	}
}
