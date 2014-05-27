using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Petri;
using DataVault;
using Logic;

namespace DPMiner
{
    public static class Global
    {
        static DVSetup setup;
        static List<EventLogic> logic;
        static IPetriNet model;
        static IDataVault dv;
        public static DVSetup Setup
        {
            get { return setup; }
            set { setup = value; }
        }
        public static List<EventLogic> Logic
        {
            get { return logic; }
            set { logic = value; }
        }
        public static IPetriNet Model
        {
            get { return model; }
            set { model = value; }
        }
        public static IDataVault DataVault
        {
            get { return dv; }
            set { dv = value; }
        }
    }
}
