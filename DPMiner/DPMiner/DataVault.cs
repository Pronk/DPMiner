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
        pID, pVal, time, key, fkey
    }
    public struct DataField
    {
        string name;
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
         IView Preview(DataVaultConstructor constructor);
         IView Editor(DataVaultConstructor constructor);
         TableType Type();
         string GetName();
     
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
        public string GetName()
        {
            return name;
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
  
            return new DataField[] {iD};
        }
        public Hub(string name, string iD)
            : base(name)
        {
            this.iD = new DataField(iD) + FieldProperty.key;
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
        List<DataField> joint;
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
                joint[n] = joint[n] <= roles[n]; 
        }
        public List<DataField> Joint
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
            foreach (DataField hub in joint)
                content.Add(hub);
            return content.ToArray();
        }
        public Link(string name, List<Hub> joint, string surID)
            : base(name)
        {
            this.joint = joint.Select<Hub,DataField>(hub => hub.Content()[0] - FieldProperty.key + FieldProperty.fkey).ToList();
            this.surID = new DataField(surID) + FieldProperty.key;
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
        DataField link;
        List<DataField> measures;
        List<DataField> categories;
        DataField key;
        public DataField Link
        {
            set { link = value; }
            get { return link; }
        }
        public List<DataField> Measures
        {
            set { measures = value; }
            get { return measures; }
        }
        public List<DataField> References
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
            content.Add(link);
            foreach (DataField measure in measures)
                content.Add(measure);
            foreach (DataField category in categories)
                content.Add(category);
            return content.ToArray();
        }
        public int Count()
        {
            return measures.Count();
        }
        public Satelite(string name, Link link, string key,  List<string> measures, List<Reference> categories)
            : base(name)
        {
            this.link = link.Content()[0];
            this.measures = new List<DataField>();
            this.key = new DataField(key) + FieldProperty.key;
            foreach (string field in measures)
                this.measures.Add(new DataField(field));
            this.categories = categories.Select<Reference, DataField>(re => re.Content()[0] - FieldProperty.key + FieldProperty.fkey).ToList();
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
        Dictionary<FieldProperty, Dictionary<string, List<DataField>>> Logic();

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
        public Dictionary<FieldProperty, Dictionary<string, List<DataField>>> Logic()
        {
            Dictionary<FieldProperty, Dictionary<string, List<DataField>>> logic = new Dictionary<FieldProperty, Dictionary<string, List<DataField>>>();
            foreach(FieldProperty fp in Enum.GetValues(typeof( FieldProperty)))
            {
                Dictionary<string, List<DataField>> lists = new Dictionary<string, List<DataField>>();
                foreach(IDataTable table in tables)
                {
                    lists.Add(table.GetName(), table.Content().Where(d => d.Roles.Contains(fp)).ToList());
                }
                logic.Add(fp, lists);
            }
            return logic;
        }
        
    }
    public enum TableType:byte
    {
        Hub, Satelite, Link, Reference 
    }
}
