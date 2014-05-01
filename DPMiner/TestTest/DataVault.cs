using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad;

namespace DPMiner
{
    interface IDataVault
    {

    }
    public class DataVault:IDataVault
    {
       
        enum FieldProperty
        {

        }
        class DataField
        {
            string name;
            List<FieldProperty> properties;
            public DataField(string name)
            {
                this.name = name;
                properties = new List<FieldProperty>();
            }

        }
        abstract class DataTable:IEquatable<DataTable>
        {
           DataVault vault;
           uint id;
           static uint idCount = 0;
           public abstract DataField[] Content;
           public DataTable()
           {
               id = idCount;
               idCount++;
           }
           public bool Equals(object obj)
           {
               if(obj is DataTable)
               {
                   DataTable that = obj as DataTable;
                   return this.id == that.id;
               }
               return false;
           }
        }
        class Hub:DataTable
        {
            DataField surID;
            DataField phisID;
            DataField source;
            DataField audit;
            public bool Surrogate {
                get { return surID == null; }
            }
            public bool Audit
            {
                get { return audit == null; }
            }
            public override DataField[] Content()
            {
                if (Surrogate && Audit)
                    return  new DataField[] {surID, phisID, source, audit};
                if(Surrogate)
                    return new DataField[] { surID, phisID, source};
                if(Audit)
                    return new DataField[] { phisID, source, audit };
                return new DataField[] { phisID, source };
            }
            public Hub(string phisID, string source, Maybe<string> surID, Maybe<string> audit ):base()
            {
                this.phisID = new DataField(phisID);
                this.source = new DataField(source);
                this.surID = surID.FinalTransform<DataField>(s=> new DataField(s), null);
                this.audit = audit.FinalTransform<DataField>(s=> new DataField(s), null);
            }
           
        }
        public class Link:DataTable
        {
            List<Hub> joint;
            DataField surID;
            DataField source;
            DataField audit;
            public bool Surrogate {
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
            public Link(List<Hub> joint, string source, Maybe<string> surID, Maybe<string> audit ):base()
            {
                this.joint = joint;
                this.source = new DataField(source);
                this.surID = surID.FinalTransform<DataField>(s=> new DataField(s), null);
                this.audit = audit.FinalTransform<DataField>(s=> new DataField(s), null);
            }
            
        }
        public class Category:DataTable
        {
            List<DataField> categories;
            DataField key;
            public override DataField[] content()
            {
                List<DataField> content = new List<DataField>();
                content.Add(key);
                foreach (DataField category in categories)
                    content.Add(category);
                return content.ToArray();
            }
            public Category(List<string> categoryNames, string key  ):base()
            {
                categories  = new List<DataField>();
                foreach (string name in categoryNames)
                    categories.Add(new DataField(name));
                this.key = new DataField(key);
            }

        }
        public class Satelite:DataTable
        {
            Link link;
            List<DataField> measures;
            List<Category> categories;
            DataField source;
            DataField audit;
            DataField key;
            public override DataField[] Content()
            {
                DataField
            }
        }

    }
}
