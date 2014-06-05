using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataVault;

namespace DPMiner
{
    internal  static class Connection
    {
        static bool offline = true;
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
        public static bool Offline
        {
            get { return offline; }
            set { offline = value; }
        }
        public static IDataStream GetStream(DVSetup setup)
        {
            if (offline)
                return new DummyStream("Test.txt", setup);
            return new MySqlStream(setup);
        }
    }
    }

