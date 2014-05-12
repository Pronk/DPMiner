using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPMiner
{
    public class LogicControl
    {
        private string[] text;
        private ProcessLogic parent;
        public LogicControl(string[] text, ProcessLogic parent)
        {
            this.text = text;
            this.parent = parent;
        }
        public Action<object, EventArgs> Update(Action errorCase)
        {
            
           return (o, e) =>
            {
                try
                {
                    List<EventLogic> newLogic = new List<EventLogic>();
                    foreach (string line in text)
                        newLogic.Add(Parse(line));
                    parent.ProcessEvents = newLogic;
                }
                catch (ArgumentException) { errorCase(); }
            };
        }

        private  EventLogic Parse(string str)
        {
            string[] arr = str.Split(new char[] { '=' });
            try
            {
                return new EventLogic(arr[0], ParseToLogic(arr[1]));
            }
            catch (IndexOutOfRangeException e) { throw new ArgumentException(); }
        }
        private LState ParseToLogic(string str)
        {
            if (str[0] == '~')
                return new LogNeg(ParseToLogic(str.Substring(1, str.Length - 1)));
            if(str[0] == '(')
            {
                Tuple<string,string,string> splited = Split(str.Trim(new char[]{'(',')'}));
                if(splited.Item2 =="&")
                    return new LogAnd(ParseToLogic(splited.Item1),ParseToLogic(splited.Item2));
                if (splited.Item2 == "v")
                    return new LogOr(ParseToLogic(splited.Item1), ParseToLogic(splited.Item2));
                else
                    throw new ArgumentException();
            }
            return new LogVal(str);
        }
            
        private Tuple<string,string,string> Split(string str)
        {
            StringBuilder[] sb = new StringBuilder[]{new StringBuilder(), new StringBuilder(), new StringBuilder()};
            int n = 0;
            int count = 0;
            bool nothing=false;
            foreach(char c in str.ToCharArray())
            {
                if (n > 2)
                    break;
                if(c ==' ')
                {
                    if (nothing)
                        continue;
                    if(count == 0)
                    {
                        n++;
                        nothing = true;
                        continue;
                    }
                    sb[n].Append(c);

                }
                if(c == '(')
                {
                    if (nothing)
                        nothing = false;
                    sb[n].Append(c);
                    count++;
                    continue;
                }
                if (c == ')')
                {
                    sb[n].Append(c);
                    count--;
                    continue;
                }
                if (nothing)
                    nothing = false;
                sb[n].Append(c);
            }
            return new Tuple<string, string, string>(sb[0].ToString(), sb[1].ToString(), sb[2].ToString());
        }
    }
}
