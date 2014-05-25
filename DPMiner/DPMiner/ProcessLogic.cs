using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPMiner
{
    public interface  LState
    {
        bool Eval(Dictionary<string, bool> values);
    }
    class LogVal:LState
    {
        string var;
        public LogVal( string var)
        {
            this.var = var;
        }
        public bool Eval(Dictionary<string, bool> values)
        {
            return values[var];
        }
        public override string ToString()
        {
            return var;
        }
    }
 class LogAnd : LState
    {
        LState first;
        LState second;
        public LogAnd(LState first, LState second )
        {
            this.first = first;
            this.second = second;
        }
        public bool Eval(Dictionary<string, bool> values)
        {
            return first.Eval(values) && second.Eval(values);
        }
        public override string ToString()
        {
            return "(" + first.ToString() +" & " + second.ToString() +")";
        }
    }
     class LogOr : LState
    {
        LState first;
        LState second;
        public LogOr(LState first, LState second )
        {
            this.first = first;
            this.second = second;
        }
        public bool Eval(Dictionary<string, bool> values)
        {
            return first.Eval(values) || second.Eval(values);
        }
        public override string ToString()
        {
            return "(" + first.ToString() + " v " + second.ToString() + ")";
        }
    }
     class LogNeg : LState
    {
        LState neg;
        public LogNeg(LState negated)
        {
            neg = negated;
        }
        public bool Eval(Dictionary<string, bool> values)
        {
            return !neg.Eval(values);
        }
        public override string ToString()
        {
            return "~" + neg.ToString();
        }            
    }
    public class EventLogic 
    {
        string name;
        public EventLogic(string name, LState condition)
        {
            this.name = name;
            this.condition = condition;
        }
        public string Name
        {
            get { return name; }
        }
        LState condition;
        public LState Condition
        {
            get { return condition; }
        }
        public bool isHappened(Dictionary<string, bool> values)
        {
            return condition.Eval(values);
        }
        public new string ToString()
        {
            return name + " = " + condition.ToString();
        }
    }
    public class ProcessLogic 
    {                                            
         Dictionary<string, List<string>> raf;
         List<EventLogic> process = new List<EventLogic>();
         public ProcessLogic (DVSetup setup )
         {
             raf = setup.ProcessVariables;
             foreach(KeyValuePair<string,List<string>> pair in raf)
             {
                 string table = pair.Key;
                 foreach (string df in pair.Value)
                     process.Add(new EventLogic(table +"." + df, new LogVal(table+ "." + df)));
             }
         }
         public List<EventLogic> ProcessEvents
        {
              get{ return process;}
            set { process = value; }
        }
        public List<string> ProcessValues
         {
             get 
             {
                 List<string> values = new List<string>();
                 foreach (KeyValuePair<string, List<string>> pair in raf)
                 {
                     string table = pair.Key;
                     foreach (string df in pair.Value)
                         values.Add(table + "." + df);
                 }
                 return values;
             }
         }
    }
}
