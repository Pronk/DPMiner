using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad;

namespace DPMiner
{
    public  enum FieldProperty : byte
    {
        processID, processVal, time, key
    }
    public class DataField
    {
        string name;
        FieldProperty role;
        public DataField(string name)
        {
            this.name = name;
        }
        FieldProperty Role
        {
            get { return role; }
            set { this.role = value; }
        }
        

    }
    public interface IDataTable
    {
        public DataField[] Content();
    }
    abstract class DataTable : IEquatable<DataTable>,IDataTable
    {
        DataVault vault;
        uint id;
        string name;
        static uint idCount = 0;
       
        public DataTable(string name)
        {
            this.name = name;
            id = idCount;
            idCount++;
        }
        public bool Equals(object obj)
        {
            if (obj is DataTable)
            {
                DataTable that = obj as DataTable;
                return this.id == that.id;
            }
            return false;
        }
        public override string ToString()
        {
            return name + " : " + id.ToString();
        }
    }
    class Hub : DataTable
    {
        DataField surID;
        DataField phisID;
        DataField source;
        DataField audit;
        public bool Surrogate
        {
            get { return surID == null; }
        }
        public bool Audit
        {
            get { return audit == null; }
        }
        public override DataField[] Content()
        {
            if (Surrogate && Audit)
                return new DataField[] { surID, phisID, source, audit };
            if (Surrogate)
                return new DataField[] { surID, phisID, source };
            if (Audit)
                return new DataField[] { phisID, source, audit };
            return new DataField[] { phisID, source };
        }
        public Hub(string name, string phisID, string source, Maybe<string> surID, Maybe<string> audit)
            : base(name)
        {
            this.phisID = new DataField(phisID);
            this.source = new DataField(source);
            this.surID = surID.FinalTransform<DataField>(s => new DataField(s), null);
            this.audit = audit.FinalTransform<DataField>(s => new DataField(s), null);
        }

    }
    public class Link : DataTable
    {
        List<Hub> joint;
        DataField surID;
        DataField source;
        DataField audit;
        public bool Surrogate
        {
            get { return surID == null; }
        }
        public bool Audit
        {
            get { return audit == null; }
        }
        public override DataField[] Content()
        {
            List<DataField> content = new List<DataField>();
            if (Surrogate)
                content.Add(surID);
            foreach (Hub hub in joint)
                content.Add(hub.Content()[0]);
            content.Add(source);
            if (Audit)
                content.Add(audit);
            return content.ToArray();
        }
        public Link(string name, List<Hub> joint, string source, Maybe<string> surID, Maybe<string> audit)
            : base(name)
        {
            this.joint = joint;
            this.source = new DataField(source);
            this.surID = surID.FinalTransform<DataField>(s => new DataField(s), null);
            this.audit = audit.FinalTransform<DataField>(s => new DataField(s), null);
        }

    }
    public class Category : DataTable
    {
        List<DataField> categories;
        DataField key;
        public override DataField[] Content()
        {
            List<DataField> content = new List<DataField>();
            content.Add(key);
            foreach (DataField category in categories)
                content.Add(category);
            return content.ToArray();
        }
        public Category(string name, List<string> categoryNames, string key)
            : base(name)
        {
            categories = new List<DataField>();
            foreach (string cname in categoryNames)
                categories.Add(new DataField(cname));
            this.key = new DataField(key);
        }

    }
    public class Satelite : DataTable
    {
        Link link;
        List<DataField> measures;
        List<Category> categories;
        DataField source;
        DataField audit;
        DataField key;
        public override DataField[] Content()
        {
            List<DataField> content = new List<DataField>();
            content.Add(key);
            content.Add(link.Content()[0]);
            foreach (DataField measure in measures)
                content.Add(measure);
            foreach (Category category in categories)
                content.Add(category.Content()[0]);
            if (audit != null)
                content.Add(audit);
            content.Add(source);
            return content.ToArray();
        }
        public Satelite(string name, string key, string source, List<string> measures, List<Category> categories, Maybe<string> audit)
            : base(name)
        {
            this.key = new DataField(key);
            this.source = new DataField(source);
            foreach (string field in measures)
                this.measures.Add(new DataField(field));
            foreach (Category field in categories)
                this.categories.Add(field);
            this.audit = audit.FinalTransform<DataField>(s => new DataField(s), null);
        }
    }
    interface IDataVault
    {
        public List<IDataTable> Tables;

    }
    public class DataVault:IDataVault
    {
       
        
        string name;
        List<IDataTable> tables;
        public List<IDataTable> Table;
        
         

    }
}
