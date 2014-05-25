using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPMiner
{
    public class Alphabeth
    {
        Dictionary<string, int> encoder = new Dictionary<string,int>();
        string[] decoder;
        public Alphabeth(List<EventLogic> events)
        {
            List<string> names = new List<string>();
            foreach(EventLogic logic in events)
            {
                names.Add(logic.Name);
                encoder.Add(logic.Name, names.Count - 1);
            }
            decoder = names.ToArray();
        }
        public int Size
        {
            get { return decoder.Length; }
        }
        public int Encode(string name)
        {
            return encoder[name];
        }
        public string Decode(int code)
        {
            return decoder[code];
        }
    }
}
