using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad;

namespace DPMiner
{
    using Fkey = Tuple<DataField, IDataTable>;
    public interface IDataVaultControl
    {
         void Add(IDataTable table);
         bool TryRemove(string tableName);
         Maybe<IDataTable> GetTable(string tableName);
         Maybe<IDataTable> GetTable(string key, TableType type);
         List<IDataTable> GetTables(TableType type);
         bool IsConnected();
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
        public bool IsConnected()
        {
            if (tables.Count == 0)
                return true;
            List<int> connect = new List<int>{0};
            for (int i = 0; i < connect.Count;i++ )
                foreach (int j in Enumerable.Range(0, tables.Count))
                {
                    if (!connect.Contains(j) && areConnected(tables[connect[i]], tables[j]))
                        connect.Add(j);
                    if (connect.Count == tables.Count)
                        return true;

                }
            return false;

        }
        private bool areConnected(IDataTable t1, IDataTable t2)
        {
            if (t1.Type() == TableType.Hub)
                if (!(t2.Type() == TableType.Link))
                    return false;
                else
                    return (t2 as Link).Joint.Select<Fkey, IDataTable>(fk => fk.Item2).Contains(t1);
            if (t1.Type() == TableType.Link)
                if (t2.Type() == TableType.Link || t2.Type() == TableType.Reference)
                    return false;
                else
                    if (t2.Type() == TableType.Hub)
                        return (t1 as Link).Joint.Select<Fkey, IDataTable>(fk => fk.Item2).Contains(t2);
                    else
                        return (t2 as Satelite).Link.Item2 == t1;
            if (t1.Type() == TableType.Satelite)
                if (t2.Type() == TableType.Satelite || t2.Type() == TableType.Hub)
                    return false;
                else
                    if (t2.Type() == TableType.Link)
                        return (t1 as Satelite).Link.Item2 == t2;
                    else
                        return (t1 as Satelite).References.Select<Fkey, IDataTable>(fk => fk.Item2).Contains(t2);
            if(t1.Type() == TableType.Reference)
                if(t2.Type() != TableType.Satelite)
                    return (t2 as Satelite).References.Select<Fkey, IDataTable>(fk => fk.Item2).Contains(t1);
            return false;
        }
        public List<IDataTable> GetTables(TableType type)
       {
           return tables.Where(t => t.Type() == type).ToList();
       }
    }
}
