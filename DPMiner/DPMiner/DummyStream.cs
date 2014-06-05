using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataVault;

namespace DPMiner
{
    class DummyStream:IDataStream
    {
        List<string> select;
        StreamReader reader;
        public DummyStream(string source, DVSetup setup)
        {
            reader = new StreamReader(source);
            select = setup.ProcessVariables.Keys.SelectMany  
                (table => setup.ProcessVariables[table].Select(s => table +"."+ s)).ToList();
            select.AddRange(setup.ProcessID.Keys.SelectMany  
                (table => setup.ProcessID[table].Select(s => table + "." + s)));
            select.AddRange(setup.TimeStamp.Keys.SelectMany  
                (table => setup.TimeStamp[table].Select(s => table + "." + s)));
        }

        public Dictionary<string,string> ReadTuple()
        {
            string input = reader.ReadLine();
            if(input == null || input=="")
                return null;
            Dictionary<string,string> output = new Dictionary<string,string>();
            string[] tuple = input.Split(new char[] {';'});
            foreach(string field in tuple)
            {
                string[] asignment = field.Split(new char[] { '=' }).Select(s => s.Trim()).ToArray();
                if (select.Contains(asignment[0]))
                    output.Add(asignment[0], asignment[1]);
            }
            return output;
        }

    }
}
