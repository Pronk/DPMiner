using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace DPMiner
{

    class MySqlStream : IDataStream
    {
        MySqlConnection connection = new MySqlConnection(Connection.Connection());
        public MySqlStream(DVSetup setup)
        {
            try{
                connection.Open();
            }
            catch (Exception e) { throw new Exception("Connection error:" + e.Message); }
            MySqlCommand command = new MySqlCommand(SelectStatement(setup),connection);
        }
        protected string SelectStatement(DVSetup setup)
        {
            ISet<string> from = new HashSet<string>
            string 
        }
    }
}
