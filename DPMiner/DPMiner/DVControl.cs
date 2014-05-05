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

                    return  new Maybe<IDataTable>(table);
                }
            return Maybe<IDataTable>.None();
        }
        public Maybe<IDataTable> GetTable(string key, TableType type)
        {
            foreach(IDataTable table in tables)
            {
                if (type == TableType.Hub)
                {
                    if (table.Type() == type)
                    {
                        if (table.Content()[0].ToString() == key || ((table as Hub).Surrogate && table.Content()[2].ToString() == key))
                            return new Maybe<IDataTable>(table);

                    }
                }
                else
                    if(table.Type() == type)
                    {
                        if(table.Content()[0].ToString() == key)
                            return new Maybe<IDataTable>(table);
                    }
                    continue;

                }
            return Maybe<IDataTable>.None();
             
        }
        public bool isConnected()
        {
            throw new MissingMethodException();
        }

    }
}
