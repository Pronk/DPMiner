using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPMiner
{
    internal  static class Connection
    {
        static string source="";
        static string db = "";
        static string username = "";
        static string pass = "";
        public static string DataSource
        {
            get { return source; }
            set { source = value; }                                                       
        }
        public static string DataBase
        {
            get { return source; }
            set { source = value; }                                                       
        }
        public static string Username
        {
            get { return source; }
            set { source = value; }                                                       
        }
        public static string Password
        {
            get { return pass; }
            set { pass = value; }                                                       
        }
        public static string Connect()
        {
            return "Database="+db+";Data Source="+source+";User Id="+username+";Password=" +pass;
        }
    }
    }

