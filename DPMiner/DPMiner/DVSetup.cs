using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPMiner
{
    using Fields = Dictionary<string, List<string>>;
    using Bindings = Dictionary<string, List<Tuple<string,string,string>>>;
    using FKey = Tuple<DataField, IDataTable>;
    [Serializable]
    public struct DVSetup
    {
        Fields pID;
        Fields pVar;
        Fields time;
        Bindings fKeyBound;
        public DVSetup(List<IDataTable> tables)
        {
            pID = new Fields();
            pVar = new Fields();
            time = new Fields();
            fKeyBound = new Bindings();
            foreach(IDataTable table in tables)
            {
                pID.Add(table.GetName(), table.Content().Where(df => df.Roles.Contains(FieldProperty.pID)).Select<DataField, string>(df => df.ToString()).ToList());
                pVar.Add(table.GetName(), table.Content().Where(df => df.Roles.Contains(FieldProperty.pVal)).Select<DataField, string>(df => df.ToString()).ToList());
                time.Add(table.GetName(), table.Content().Where(df => df.Roles.Contains(FieldProperty.time)).Select<DataField, string>(df => df.ToString()).ToList());
                if (table.Type() == TableType.Hub || table.Type() == TableType.Reference)
                    continue;
                List<Tuple<string, string, string>> keyPairs = new List<Tuple<string,string,string>>();
               if(table.Type() == TableType.Link)
               {
                   Link link = table as Link;
                   foreach (FKey fKey in link.Joint)
                       keyPairs.Add(new Tuple<string, string, string>(fKey.Item1.ToString(), fKey.Item2.GetName(), fKey.Item2.Content()[0].ToString()));
                   fKeyBound.Add(table.GetName(), keyPairs);
                   continue;
               }
               if(table.Type() == TableType.Satelite)
               {
                   Satelite sat = table as Satelite;
                   keyPairs.Add(new Tuple<string, string, string>(sat.Link.Item1.ToString(), sat.Link.Item2.GetName(), sat.Link.Item2.Content()[0].ToString()));
                   foreach (FKey fKey in sat.References)
                       keyPairs.Add(new Tuple<string, string, string>(fKey.Item1.ToString(), fKey.Item2.GetName(), fKey.Item2.Content()[0].ToString()));
                        continue;
               }
            }
        }
        public Fields ProcessID
        {
            get { return pID; }
        }
        public Fields ProcessVariables
        {
            get { return pVar; }
        }
        public Fields TimeStamp
        {
            get { return time; }
        }
        public Bindings ForeignKeyPairs
        {
            get { return fKeyBound; }
        }
    }
}
