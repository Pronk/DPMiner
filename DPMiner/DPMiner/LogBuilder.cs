using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataVault;
using Logic;


namespace DPMiner
{
    using Extensions;
    enum MultyEventSolution : byte { drop, first, priority, shuffle, parallel  }
    class LogBuilder
    {
        IDataStream stream;
        DVSetup setup;
        List<EventLogic> logic;
        MultyEventSolution solution;
        Dictionary<string,string> current = null;
        Dictionary<string, List<int>> logs = new Dictionary<string, List<int>>();
        List<int> log = new List<int>();
        Dictionary<string, bool> input = new Dictionary<string,bool>();
        Alphabeth alphabeth;
        public LogBuilder(IDataStream stream, DVSetup setup, List<EventLogic> logic, MultyEventSolution solution)
        {
            this.stream = stream;
            this.setup = setup;
            this.logic = logic;
            this.solution = solution;
            foreach (KeyValuePair<string, List<string>> pair in setup.ProcessVariables)
                foreach (string name in pair.Value)
                    input.Add(pair.Key + "." + name, false);
            alphabeth = new Alphabeth(logic);
           
        }

        public int[][] Log
        {
            get
            {
                int[][] output = new int[logs.Count()][];
                int n = 0;
                foreach(KeyValuePair<string,List<int>> pair in logs)
                {
                    output[n] = pair.Value.ToArray();
                    n++;
                }
                return output;
            }
        }
        public Alphabeth Alphabeth
        {
            get { return alphabeth; }
        }
        public bool DoWork()
        {
            if (current == null)
            {

                current = stream.ReadTuple();
                if (current == null)
                    return false;
                else
                    return true;
            }
            Dictionary<string, string> old = current;
            current = stream.ReadTuple();
            if (current == null)
                return false;
            foreach (KeyValuePair<string, List<string>> pair in setup.ProcessID)
                foreach (string name in pair.Value)
                {
                    string s = pair.Key + "." + name;
                    if (old[s] != current[s])
                    {
                        string id = ID(old);
                        if (logs.Keys.Contains(id))
                            logs[id].AddRange(log);
                        else
                            logs.Add(id, log);
                        log = new List<int>();
                    }

                }
            foreach (KeyValuePair<string, List<string>> pair in setup.ProcessVariables)
                foreach (string name in pair.Value)
                {
                    string s = pair.Key + "." + name;
                    input[s] = old[s] != current[s];
                }
            List<int> happened = new List<int>();
            foreach(EventLogic word in logic)
            {
                if (word.isHappened(input))
                    happened.Add(alphabeth.Encode(word.Name));
            }
            if (happened.Count == 1)
                log.Add(happened[0]);
            if (happened.Count > 1)
                Append(happened);
            return true;
            
        }

        protected string ID(Dictionary<string,string> tuple)
        {
            return setup.ProcessID.SelectMany<KeyValuePair<string, List<string>>, string>
                (kvp => kvp.Value.
                Select<string, string>(str => kvp.Key + "." + str)).
                Aggregate<string, string>("", (acc, str) => acc + str);
        }

        protected void Append(List<int> events)
        {
            if (solution == MultyEventSolution.drop)
                return;
            if(solution == MultyEventSolution.first)
            {
                log.Add(events[0]);
                return;
            }
            if (solution == MultyEventSolution.shuffle)
                events.Shuffle();
            if (solution == MultyEventSolution.parallel)
                events = events.Combinations() as List<int>;
            foreach (int x in events)
                log.Add(x);

            

        }
    }
}
