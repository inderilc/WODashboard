using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
//using EDIApp.UIModels;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using System.Windows;
using WorkOrderDashboard.Configuration;
using Dapper;
//using EDIApp.Data;
using System.ComponentModel;
using System.Text;
using System.Globalization;
using System.Threading;

using WorkOrderDashboard.Models;


namespace WorkOrderDashboard.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private BackgroundWorker bwUpdate { get; set; }
        public FbConnection db { get; set; }
        public Config cfg { get; set; }
        private StringBuilder sb = new StringBuilder();

        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            CultureInfo ci = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;


            toChange = DateTime.Now;
            bwUpdate = new BackgroundWorker();
            bwUpdate.DoWork += UpdateWork;
            bwUpdate.WorkerReportsProgress = true;
            bwUpdate.ProgressChanged += LogUpdate;
            bwUpdate.RunWorkerCompleted += BwUpdate_RunWorkerCompleted;
            try
            {
                cfg = Config.Load();
                Config.Save(cfg);
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Starting. Check Configuration File" + ex.ToString());
            }

            cmdRefresh();
        }
        public DateTime date;
        public ObservableCollection<OrderDataVM> PendingOrders { get; set; }
        public DateTime toChange { get{ return date;} set { date = value;} }
        public String Status => sb.ToString();
        private void LogUpdate(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is String)
            {
                sb.AppendLine(e.UserState as String);
            }
            RaisePropertyChanged("Status");
        }

        private void UpdateWork(object sender, DoWorkEventArgs e)
        {
            
                var obj = new WorkOrder(cfg);
                obj.OnLog += WO_OnLog;
                obj.ChangeDateScheduled(PendingOrders.Where(k => k.Update).Select(x => x.h.OrderNum).ToList(), toChange);
                RaisePropertyChanged("PendingOrders");
         
            
        }

        private void BwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cmdRefresh();
        }
        private void WO_OnLog(string msg)
        {
            bwUpdate.ReportProgress(0, msg);
        }
        private void cmdRefresh()
        {
            var obj = new WorkOrder(cfg);
            obj.OnLog += WO_OnLog;
            PendingOrders=obj.GetWoData();
            RaisePropertyChanged("PendingOrders");
        }
        public ICommand CheckAllWO => new RelayCommand(cmdCheckAllWO);

        private void cmdCheckAllWO()
        {
            foreach (var i in PendingOrders)
            {
                i.Update = true;
            }
            RaisePropertyChanged("PendingOrders");
        }

        public ICommand CheckNoneWO => new RelayCommand(cmdCheckNoneWO);

        private void cmdCheckNoneWO()
        {
            foreach (var i in PendingOrders)
            {
                i.Update = false;
            }
            RaisePropertyChanged("PendingOrders");
        }
        public ICommand Update => new RelayCommand(cmdUpdate);
        private void cmdUpdate()
        {
            if (!bwUpdate.IsBusy)
            {
                bwUpdate.RunWorkerAsync();
            }
            
        }
    }
}