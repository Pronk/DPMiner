﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad;

namespace DPMiner
{
    public  enum FieldProperty : byte
    {
        processID, processVal, time, key, fkey
    }
    public class DataField
    {
        string name;
        FieldProperty role;
        public DataField(string name)
        {
            this.name = name;
        }
        public FieldProperty Role
        {
            get { return role; }
            set { this.role = value; }
        }

        public override string ToString()
        {
 	        return name;
        }
        

    }
   
    public interface IDataTable
    {
         DataField[] Content();
         IView Preview(DataVaultConstructor constructor);
         IView Editor(DataVaultConstructor constructor);
         TableType Type();
     
    }
    public abstract class DataTable : IEquatable<DataTable>,IDataTable
    {
        uint id;
        string name;
        static uint idCount = 0;
        public abstract DataField[] Content();
        public abstract TableType Type();
        public IView Preview(DataVaultConstructor constructor)
        {
            return new DataTableView(this, constructor);
        }
        public virtual IView Editor(DataVaultConstructor constructor)
        {
            return new TableEditor(this, constructor);
        }
        public DataTable(string name)
        {
            this.name = name;
            id = idCount;
            idCount++;
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public bool Equals(DataTable obj)
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
   public class Hub : DataTable
    {
        DataField surID;
        DataField phisID;
        DataField source;
        DataField audit;
        public DataField PhisicalID
        {
            set {  phisID=value; }
        }
        public DataField SurogateID
        {
            set { surID =value; }
        }
        public bool Surrogate
        {
            get { return surID != null; }
        }
        public bool Audit
        {
            get { return audit != null; }
        }
        public override DataField[] Content()
        {
            if (Surrogate && Audit)
                return new DataField[] {  phisID, surID,   audit, source };
            if (Surrogate)
                return new DataField[] { phisID, surID, source };
            if (Audit)
                return new DataField[] { phisID, audit, source };
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
        public override IView Editor(DataVaultConstructor constructor)
        {
            IView view = base.Editor(constructor);
            HubControls edits = new HubControls(this, constructor);
            edits.Publish();
            return view;
        }
       public override TableType Type()
       {
           return TableType.Hub;
       }

        
    }
    public class Link : DataTable
    {
        List<Hub> joint;
        DataField surID;
        DataField source;
        DataField audit;
        public List<Hub> Joint
        {
            set{joint = value;}
            get{return joint;}
        }
        public DataField Key
        {
            set{surID = value;}
            get{return surID;}
        }
        public override TableType Type()
       {
           return TableType.Link;
       }
        public bool Surrogate
        {
            get { return surID != null; }
        }
        public bool Audit
        {
            get { return audit != null; }
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
        public override IView Editor(DataVaultConstructor constructor)
        {
            IView view = base.Editor(constructor);
            LinkControls edits = new LinkControls(this, constructor);
            edits.Publish();
            return view;
        }
    }
    public class Reference: DataTable
    {
        List<DataField> categories;
        DataField key;
        public DataField Key
        {
            set { key = value; }
        }
        public  List<DataField> Fields
        {
            set { categories = value; }
        }
        public override DataField[] Content()
        {
            List<DataField> content = new List<DataField>();
            content.Add(key);
            foreach (DataField category in categories)
                content.Add(category);
            return content.ToArray();
        }
        public Reference(string name, List<string> categoryNames, string key)
            : base(name)
        {
            categories = new List<DataField>();
            foreach (string cname in categoryNames)
                categories.Add(new DataField(cname));
            this.key = new DataField(key);
        }
       public override TableType Type()
       {
           return TableType.Reference;
       }
       public override IView Editor(DataVaultConstructor constructor)
       {
           IView view = base.Editor(constructor);
           ReferenceControls edits = new ReferenceControls(this, constructor);
           edits.Publish();
           return view;
       }
    }
    public class Satelite : DataTable
    {
        Link link;
        List<DataField> measures;
        List<Reference> categories;
        DataField source;
        DataField audit;
        DataField key;
        public Link Link
        {
            set { link = value; }
        }
        public List<DataField> Measures
        {
            set { measures = value; }
        }
        public List<Reference> References
        {
            set { categories = value; }
        }
        public DataField Key
        {
            set { key = value; }
        }
        public override DataField[] Content()
        {
            List<DataField> content = new List<DataField>();
            content.Add(key);
            content.Add(link.Content()[0]);
            foreach (DataField measure in measures)
                content.Add(measure);
            foreach (Reference category in categories)
                content.Add(category.Content()[0]);
            if (audit != null)
                content.Add(audit);
            content.Add(source);
            return content.ToArray();
        }
        public int Count()
        {
            return measures.Count();
        }
        public Satelite(string name, Link link, string key, string source, List<string> measures, List<Reference> categories, Maybe<string> audit)
            : base(name)
        {
            this.link = link;
            this.measures = new List<DataField>();
            this.categories = new List<Reference>();
            this.key = new DataField(key);
            this.source = new DataField(source);
            foreach (string field in measures)
                this.measures.Add(new DataField(field));
            foreach (Reference field in categories)
                this.categories.Add(field);
            this.audit = audit.FinalTransform<DataField>(s => new DataField(s), null);
        }
       public override TableType Type()
       {
           return TableType.Satelite;
       }
       public override IView Editor(DataVaultConstructor constructor)
       {
           IView view = base.Editor(constructor);
           SateliteControls edits = new SateliteControls(this, constructor);
           edits.Publish();
           return view;
       }
    }
    public interface IDataVault
    {
       
         DataVaultFormView View(DataVaultConstructor constructor);
        IDataVaultControl Control();

    }
    public class DataVault:IDataVault
    {
       
        
        string name;
        List<IDataTable> tables;
        public DataVault(string str)
        {
            name = str;
            tables = new List<IDataTable>();
        }
        public DataVaultFormView View(DataVaultConstructor constructor)
        {
            return new DataVaultFormView(tables, constructor);
        }
        public IDataVaultControl Control()
        {
            return new DataVualtControl(tables);
        }
        
    }
    public enum TableType:byte
    {
        Hub, Satelite, Link, Reference 
    }
}
