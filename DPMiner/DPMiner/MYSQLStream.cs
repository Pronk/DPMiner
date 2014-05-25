using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace DPMiner
{

    class MySqlStream : IDataStream
    {
        MySqlConnection connection = new MySqlConnection(Connection.Connect());
        MySqlDataReader reader;
        DVSetup setup;
        public MySqlStream(DVSetup setup)
        {
            this.setup = setup;
            try{
                connection.Open();
            }
            catch (Exception e) { throw new Exception("Connection error:" + e.Message); }
            MySqlCommand command = new MySqlCommand(SelectStatement(setup),connection);
            reader = command.ExecuteReader();
        }
        public override string ToString()
        {
            return "(" + Connection.Connect() + ") WITH:" + SelectStatement(setup);
        }
        public Dictionary<string, string> ReadTuple()
        {
            Dictionary<string,string> fetch = new Dictionary<string,string>();
            string str = "";
            if(reader.Read())
            {
                foreach (KeyValuePair<string, List<string>> pair in setup.ProcessVariables)
                    foreach (string name in pair.Value) 
                    {
                        str = pair.Key + "." + name;
                        fetch.Add(str, reader[str].ToString());
                    }
                foreach (KeyValuePair<string, List<string>> pair in setup.ProcessID)
                    foreach (string name in pair.Value)
                    {
                        str = pair.Key + "." + name;
                        fetch.Add(str, reader[str].ToString());
                    }
                foreach (KeyValuePair<string, List<string>> pair in setup.TimeStamp)
                    foreach (string name in pair.Value)
                    {
                        str = pair.Key + "." + name;
                        fetch.Add(str, reader[str].ToString());
                    }
                return fetch;
            }
            return null;
        }
        protected static string SelectStatement(DVSetup setup)
        {
            ISet<string> from = new HashSet<string>();
            string select = "SELECT";
            string where = "WHERE";
            string sortBy ="SORT BY";
            foreach( KeyValuePair<string,List<string>> pair in  setup.ProcessVariables)
            {
                from.Add(pair.Key);
                foreach (string name in pair.Value)
                    select += pair.Key + "." + name + ","; 
            }
            foreach (KeyValuePair<string, List<string>> pair in setup.ProcessID)
            {
                from.Add(pair.Key);
                foreach (string name in pair.Value)
                {
                    sortBy +=" " + pair.Key + "." + name + ",";
                    select +=" " + pair.Key + "." + name + ",";
                }
            }
            foreach (KeyValuePair<string, List<string>> pair in setup.TimeStamp)
            {
                from.Add(pair.Key);
                foreach (string name in pair.Value)
                {
                    sortBy +=" " + pair.Key + "." + name + ",";
                    select +=" " + pair.Key + "." + name + ",";
                }
            }
            sortBy = sortBy.Remove(sortBy.Length - 1);
            select = select.Remove(select.Length - 1);
            foreach(KeyValuePair<string,List<Tuple<string,string,string>>> pair in setup.ForeignKeyPairs)
            {
                from.Add(pair.Key);
                foreach (Tuple<string, string, string> fk in pair.Value)
                {
                    where += pair.Key + "." + fk.Item1 + "=" + fk.Item2 + "." + fk.Item3 + " AND ";
                    
                }
            }
            where = where.Remove(where.Length  - 6, 5);
            string fr = from.Aggregate<string, string, string>("FROM", (acc, s) => acc + " " + s + ",", (acc) => acc.Remove(acc.Length - 1));
            return select + " " + fr + " " + where + " " + sortBy + ";";
        }
    }
}
