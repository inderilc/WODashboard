using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace WorkOrderDashboard.Models
{
    public partial class OrderData
    {
        public int OrderID { get; set; }
        public String OrderNum { get; set; }
        public String BillNum { get; set; }
        public String BillDesc { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class OrderDataVM : ViewModelBase
    {
        public OrderData h { get; set; }
        public OrderDataVM(OrderData h)
        {
            this.h = h;
        }
        private Boolean _t;
        public Boolean Update { get { return _t; } set { _t = value; RaisePropertyChanged(); } }
    }
}
