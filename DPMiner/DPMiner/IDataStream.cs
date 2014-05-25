using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;


namespace DPMiner
{
    interface IDataStream
    {
        Dictionary<string, string> ReadTuple();
        
    }
}
