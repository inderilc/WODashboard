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
        
        private FbController fbc {get;set;}

        private Config cfg;


        public WorkOrder(Config cfg)
        {
            this.cfg = cfg;
            fbc = new FbController(this.cfg);
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
            List<OrderData> data = fbc.db.Query<OrderData>(SQL.data.showdata).ToList();
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
                var rs = fbc.api.sendAnyRequest(rq) as GetWorkOrderRsType;
                rs.WO.DateScheduled = toChange.ToString("o");
                var saverq = new SaveWorkOrderRqType();
                saverq.WO = rs.WO;
                var savers = fbc.api.sendAnyRequest(saverq) as SaveWorkOrderRsType;
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
