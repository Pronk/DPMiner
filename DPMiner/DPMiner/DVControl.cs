using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad;

namespace DPMiner
{
    public interface IDataVaultControl
    {
         void Add(IDataTable table);
         bool TryRemove(string tableName);
         Maybe<IDataTable> GetTable(string tableName);
         Maybe<IDataTable> GetTable(string key, TableType type);
         bool isConnected();
    }
    public class DataVualtControl:IDataVaultControl
    {
        List<IDataTable> tables;
        public DataVualtControl(List<IDataTable> tables)
        {
            this.tables = tables;
        }
        public void Add(IDataTable table)
        {
            tables.Add(table);
        }
        public bool TryRemove(string tableName)
        {
            foreach(int n in Enumerable.Range(0,tables.Count))
                if(tables[n].ToString()== tableName )
                {
                    tables.RemoveAt(n);
                    return true;
                }
            return false;
            
        }
        public Maybe<IDataTable> GetTable(string tableName)
        {
             foreach(IDataTable table in tables)
                if(table.ToString() == tableName )
                {

                    return  Maybe<IDataTable>.Something(table);
                }
            return Maybe<IDataTable>.None();
        }
        public Maybe<IDataTable> GetTable(string key, TableType type)
        {
            foreach(IDataTable table in tables)
            {
                    if(table.Type() == type)
                    {
                        if(table.Content()[0].ToString() == key)
                            return Maybe<IDataTable>.Something(table);
                    }
                    continue;

                }
            return Maybe<IDataTable>.None();
             
        }
        public bool isConnected()
        {
            List<int> connect = new List<int>{0};
            foreach(int i in Enumerable.Range(0,tables.Count))
                foreach(int j in Enumerable.Range(i+1,tables.Count-i-1))
                {
                    if (connect.Contains(i))
                        if (!connect.Contains(j) && areConnected(tables[i], tables[j]))
                            connect.Add(j);
                    if (connect.Count  == tables.Count)
                        return true;
                            
                }
            return false;

        }
        private bool areConnected(IDataTable t1, IDataTable t2)
        {
            foreach (DataField df in t1.Content())
                if (df.ToString() == t2.Content()[0].ToString())
                    return true;
            foreach (DataField df in t2.Content())
                if (df.ToString() == t1.Content()[0].ToString())
                    return true;
            return false;
        }
    }
}
