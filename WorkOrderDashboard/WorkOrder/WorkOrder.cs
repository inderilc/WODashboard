using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using WorkOrderDashboard.Configuration;
using System.Collections.ObjectModel;
using Dapper;
using FishbowlSDK;
using WorkOrderDashboard.Controller;

namespace WorkOrderDashboard.Models
{
    class WorkOrder
    {
        public event LogEvent OnLog;
        public delegate void LogEvent(String msg);

        private FbConnection db;
        public FishbowlSDK.Fishbowl api { get; set; }

        private Config cfg;


        /*public WorkOrder(FbConnection db, Config cfg) //Constructor needs to be modified to pass down Config details
        {
            this.db = db;
            this.cfg = cfg;
        }
        */
        public WorkOrder(Config cfg)
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

        private void Log(String msg)
        {
            if (OnLog != null)
            {
                OnLog(msg);
            }
        }

        public ObservableCollection<OrderDataVM> GetWoData()
        {
            List<OrderData> data = db.Query<OrderData>(SQL.data.showdata).ToList();
            if (data != null)
            {
                return new ObservableCollection<OrderDataVM>(data.Select(k => new OrderDataVM(k)).ToList());
            }
            else
            {
                Log("Downloading Data failed.");
                return new ObservableCollection<OrderDataVM>();
            }
        }
        public void ChangeDateScheduled(List<string> woList, DateTime toChange)
        {
            Log(string.Format("Changing dates for {0} {1} to {2}.", woList.Count(), woList.Count() < 2 ? "order" : "orders", toChange.ToString("MM-dd-yyyy")));
            foreach (var wo in woList)
            {
                var rq = new GetWorkOrderRqType();
                rq.WorkOrderNumber = wo.ToString();
                var rs = api.sendAnyRequest(rq) as GetWorkOrderRsType;
                rs.WO.DateScheduled = toChange.ToString("o");
                var saverq = new SaveWorkOrderRqType();
                saverq.WO = rs.WO;
                var savers = api.sendAnyRequest(saverq) as SaveWorkOrderRsType;
                if (savers.statusCode == "1000")
                {
                    Log("-----Date Changed for Work Order: " + rq.WorkOrderNumber);
                }
                else
                {
                    Log("-----Changing Date failed for Work Order: " + rq.WorkOrderNumber);
                }
            }
        }

    }
}
