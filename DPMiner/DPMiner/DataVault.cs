using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad;



namespace DataVault
{
    using Fkey = Tuple<DataField, IDataTable>;
    public  enum FieldProperty : byte
    {
        pID, pVal, time, key, fkey
    }
   
    public struct DataField
    {
        private string name;
        HashSet<FieldProperty> roles;
        public DataField(string name)
        {
            this.name = name;
            roles = new HashSet<FieldProperty>();
        }
        public FieldProperty Role
        {
            set { roles.Add(value);  }
        }
        public HashSet<FieldProperty> Roles
        {
            get { return roles; }
        }
        public static DataField operator - ( DataField df, FieldProperty fp)
        {
            df.roles.Remove(fp);
            return df;
        }
        public static DataField operator + ( DataField df, FieldProperty fp)
        {
            df.roles.Add(fp);
            return df;
        }
        public static DataField operator  <= (DataField df, HashSet<FieldProperty> set)
        {
            df.roles = set;
            return df;
        }
        public static DataField operator >=(DataField df, HashSet<FieldProperty> set)
        {
            df.roles = new HashSet<FieldProperty>(set.Union(df.roles));
            return df;
        }
        public override string ToString()
        {
 	        return name;
        }
        

      
    }

    public interface IDataTable
    {
         DataField[] Content();
         IView Preview(IDataVaultConstructor constructor);
         IView Editor(IDataVaultConstructor constructor);
         TableType Type();
         string GetName();
     
    }
    public abstract class DataTable : IEquatable<DataTable>,IDataTable
    {
        uint id;
        string name;
        static uint idCount = 0;
        protected DataField timeStamp;
        public abstract DataField[] Content();
        public abstract TableType Type();
        public IView Preview(IDataVaultConstructor constructor)
        {
            return new DataTableView(this, constructor);
        }
        public virtual IView Editor(IDataVaultConstructor constructor)
        {
            return new TableEditor(this, constructor);
        }
        public DataTable(string name)
        {
            this.name = name;
            id = idCount;
            idCount++;
        }
        public string GetName()
        {
            return name;
        }
        public DataField TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
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
        DataField iD;
        public DataField ID
        {
            set {  iD=value; }
            get { return iD; }
        }
        public void IDRole(HashSet<FieldProperty> set)
        {
            iD = iD <= set;
        }
        public override DataField[] Content()
        {
  
            return new DataField[] {iD,timeStamp};
        }
        public Hub(string name, string iD)
            : base(name)
        {
            this.iD = new DataField(iD) + FieldProperty.key;
        }
        public override IView Editor(IDataVaultConstructor constructor)
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
        List<Fkey> joint;
        DataField surID;
        public void IDRole(HashSet<FieldProperty> set)
        {
             surID = surID <= set;
        }
        public void FKeyRoles(List<HashSet<FieldProperty>> roles)
        {
            if (joint.Count != roles.Count)
                return;
            foreach (int n in Enumerable.Range(0, joint.Count))
                joint[n] = new Fkey (joint[n].Item1 <= roles[n], joint[n].Item2) ; 
        }
        public List<Fkey> Joint
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
        public override DataField[] Content()
        {
            List<DataField> content = new List<DataField>();
            content.Add(surID);
            foreach (DataField hub in joint.Select<Fkey,DataField>(t => t.Item1))
                content.Add(hub);
            content.Add(timeStamp);
            return content.ToArray();
        }
        public Link(string name, List<Hub> joint,List<string> fKeys, string surID)
            : base(name)
        {
            this.joint = Enumerable.Zip<string, IDataTable, Fkey>(fKeys, joint, (str, table) => new Fkey(new DataField(str), table)).ToList();
            this.surID = new DataField(surID) + FieldProperty.key;
        }
        public override IView Editor(IDataVaultConstructor constructor)
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
            get { return key; }
        }
        public  List<DataField> Fields
        {
            set { categories = value; }
            get { return categories;  }
        }
        public void IDRole(HashSet<FieldProperty> set)
        {
            key = key <= set;
        }
        public void FieldRoles(List<HashSet<FieldProperty>> roles)
        {
            if (categories.Count != roles.Count)
                return;
            foreach (int n in Enumerable.Range(0, roles.Count))
                categories[n] = categories[n] <= roles[n];
        }
        public override DataField[] Content()
        {
            List<DataField> content = new List<DataField>();
            content.Add(key);
            content.Add(timeStamp);
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
            this.key = new DataField(key)  + FieldProperty.key;
        }
       public override TableType Type()
       {
           return TableType.Reference;
       }
       public override IView Editor(IDataVaultConstructor constructor)
       {
           IView view = base.Editor(constructor);
           ReferenceControls edits = new ReferenceControls(this, constructor);
           edits.Publish();
           return view;
       }
    }
    public class Satelite : DataTable
    {
       
        Fkey link;
        List<DataField> measures;
        List<Fkey> categories;
        DataField key;
        public Fkey Link
        {
            set { link = value; }
            get { return link; }
        }
        public List<DataField> Measures
        {
            set { measures = value; }
            get { return measures; }
        }
        public List<Fkey> References
        {
            set { categories = value; }
            get { return categories; }
        }
        public DataField Key
        {
            set { key = value; }
            get { return key; }
        }
        public override DataField[] Content()
        {
            List<DataField> content = new List<DataField>();
            content.Add(key);
            content.Add(link.Item1);
            content.Add(timeStamp);
            foreach (DataField measure in measures)
                content.Add(measure);
            foreach (Fkey category in categories)
                content.Add(category.Item1);
            return content.ToArray();
        }
        public int Count()
        {
            return measures.Count();
        }
        public Satelite(string name, Link link, string linkName, string key,  List<string> measures, List<Reference> categories, List<string> refNames )
            : base(name)
        {
            this.link = new Fkey(new DataField(linkName), link);
            this.measures = new List<DataField>();
            this.key = new DataField(key) + FieldProperty.key;
            foreach (string field in measures)
                this.measures.Add(new DataField(field));
            this.categories = Enumerable.Zip<string, IDataTable, Fkey>(refNames, categories, (s, t) => new Fkey(new DataField(s), t)).ToList();
        }
       public override TableType Type()
       {
           return TableType.Satelite;
       }
       public override IView Editor(IDataVaultConstructor constructor)
       {
           IView view = base.Editor(constructor);
           SateliteControls edits = new SateliteControls(this, constructor);
           edits.Publish();
           return view;
       }
    }
    public interface IDataVault
    {
       
        DataVaultFormView View(IDataVaultConstructor constructor);
        IDataVaultControl Control();
        DVSetup Logic();

    }
    [Serializable]
    public class DataVault:IDataVault
    {
       
        
        string name;
        List<IDataTable> tables;
        public DataVault(string str)
        {
            name = str;
            tables = new List<IDataTable>();
        }
        public DataVaultFormView View(IDataVaultConstructor constructor)
        {
            return new DataVaultFormView(tables, constructor);
        }
        public IDataVaultControl Control()
        {
            return new DataVualtControl(tables);
        }
        public DVSetup Logic()
        {
             return new DVSetup(tables); 
        }
        
    }
    public enum TableType:byte
    {
        Hub, Satelite, Link, Reference 
    }
}
