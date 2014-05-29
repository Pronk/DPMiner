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
                (table => setup.ProcessVariables[table].Select(s => table + "." + s)));
            select.AddRange(setup.TimeStamp.Keys.SelectMany  
                (table => setup.ProcessVariables[table].Select(s => table + "." + s)));
        }

        public Dictionary<string,string> ReadTuple()
        {
            string input = reader.ReadLine();
            if(input == null)
                return null;
            Dictionary<string,string> output = new Dictionary<string,string>();
            string[] tuple = input.Split(new char[] {';'});
            tuple.Where
                (field => select.Contains(field.Split(new char[]{'='})[0]))
                .Select<string, object>(field =>
                {
                    output.Add(field.Split(new char[] { '=' })[0], field.Split(new char[] { '=' })[1]);
                    return null;});
            return output;
        }

    }
}
