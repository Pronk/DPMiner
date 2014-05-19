using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace DPMiner
{
    public interface ILogicControl
    {
        EventHandler Compile(Action successCase, Action<int> errorCase);
        EventHandler Return();
        EventHandler AddSelected();
        ProcessLogic Logic();
    }
    public class TextToLogicParser:ILogicControl
    {
        private RichTextBox text;
        private ProcessLogic parent;
        public TextToLogicParser(RichTextBox text, ProcessLogic parent)
        {
            this.text = text;
            this.parent = parent;
        }
        public EventHandler Compile(Action successCase, Action<int> errorCase)
        {
            
           return (o, e) =>
            {
               
                 
                    List<EventLogic> newLogic = new List<EventLogic>();
                    foreach (string line in text.Lines)
                        try
                        {
                        newLogic.Add(Parse(line));
                        }
                        catch (ArgumentException) { errorCase(text.Lines.ToList().IndexOf(line) + 1); return; }
                    parent.ProcessEvents = newLogic;
                
                successCase();
            };
        }
        public EventHandler Return()
        {
            return (sender, e) =>
                {
                    text.Lines = parent.ProcessEvents.Select<EventLogic, string>(v => v.ToString()).ToArray();
                };
        }
        public EventHandler AddSelected()
        {
           return (sender, e) =>
           {
               ListControl list = sender as ListControl;
               if(list != null && list.SelectedValue != null)
                text.Text += list.SelectedValue.ToString();
           };
        }
        public ProcessLogic Logic()
        { return parent; }

        private  EventLogic Parse(string str)
        {
            string[] arr = str.Split(new char[] { '=' });
            try
            {
                return new EventLogic(arr[0].Trim(), ParseToLogic(arr[1].Trim()));
            }
            catch (IndexOutOfRangeException e) { throw new ArgumentException(); }
        }
        private LState ParseToLogic(string str)
        {
            if (str[0] == '~')
                return new LogNeg(ParseToLogic(str.Substring(1, str.Length - 1)));
            if(str[0] == '(')
            {
                Tuple<string,string,string> splited = Split(str.Substring(1,str.Length -2));
                if(splited.Item2 =="&")
                    return new LogAnd(ParseToLogic(splited.Item1),ParseToLogic(splited.Item3));
                if (splited.Item2 == "v")
                    return new LogOr(ParseToLogic(splited.Item1), ParseToLogic(splited.Item3));
                else
                    throw new ArgumentException();
            }
            if (parent.ProcessValues.Contains(str))
                return new LogVal(str);
            else 
                throw new ArgumentException();
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
