using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkOrderDashboard.Configuration;
using FishbowlSDK;
using FirebirdSql.Data.FirebirdClient;

namespace WorkOrderDashboard.Controller
{
    public class FbController
    {

        private Config cfg;
        public FbConnection db { get; set; }
        public FishbowlSDK.Fishbowl api { get; set; }
        public FbController(Config cfg)
        {
            this.cfg = cfg;
            db = InitDB();
            api = InitAPI();
        }
        private FbConnection InitDB()
        {
            String CSB = InitCSB();
            FbConnection db = new FbConnection(CSB);
            db.Open();
            return db;
        }

        private string InitCSB()
        {
            FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
            csb.DataSource = cfg.Fishbowl.ServerAddress;
            csb.Database = cfg.Fishbowl.Database;
            csb.UserID = cfg.Fishbowl.User;
            csb.Password = cfg.Fishbowl.Pass;
            csb.Port = cfg.Fishbowl.Port;
            csb.ServerType = FbServerType.Default;
            return csb.ToString();
        }

        private FishbowlSDK.Fishbowl InitAPI()
        {
            var newfb = new FishbowlSDK.Fishbowl(cfg.Fishbowl.ServerAddress, cfg.Fishbowl.ServerPort, cfg.Fishbowl.FBIAKey, cfg.Fishbowl.FBIAName, cfg.Fishbowl.FBIADesc, cfg.Fishbowl.Persistent, cfg.Fishbowl.Username, cfg.Fishbowl.Password);
            return newfb;
        }
    }
}
