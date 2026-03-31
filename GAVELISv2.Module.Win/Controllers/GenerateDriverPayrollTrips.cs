using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class GenerateDriverPayrollTrips : ViewController {
        private SimpleAction generateDriverPayrollTripsAction;
        private DriverPayrollBatch _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateDriverPayrollTrips() {
            this.TargetObjectType = typeof(DriverPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch.GenerateDriverPayrollTrips";
            this.generateDriverPayrollTripsAction = new SimpleAction(this, 
            actionID, PredefinedCategory.RecordEdit);
            this.generateDriverPayrollTripsAction.Execute += new 
            SimpleActionExecuteEventHandler(
            GenerateDriverPayrollTripsAction_Execute);
        }
        private void GenerateDriverPayrollTripsAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {

            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as DriverPayrollBatch;

            try
            {
                for (int i = _DriverPayrollBatch.DriverPayrollTrips.Count - 1;
                i >= 0; i--)
                {
                    _DriverPayrollBatch.DriverPayrollTrips[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();

            DateTime fDate;
            DateTime tDate;
            string critString = string.Empty;
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
            ICollection driverTripsType;
            DevExpress.Xpo.Metadata.XPClassInfo driverTripClass;
            driverTripClass = _DriverPayrollBatch.Session.GetClassInfo(typeof(
            DriverRegistry));
            fDate = new DateTime(_DriverPayrollBatch.PeriodStart.Year, _DriverPayrollBatch.PeriodStart.Month, _DriverPayrollBatch.PeriodStart.Day, 01, 0, 0);
            tDate = new DateTime(_DriverPayrollBatch.PeriodEnd.Year, _DriverPayrollBatch.PeriodEnd.Month, _DriverPayrollBatch.PeriodEnd.Day, 23, 59, 59); ;
            if (_DriverPayrollBatch.Customer != null)
            {
                critString = "[TripID.TripCustomer.No] = '" + _DriverPayrollBatch.Customer.No + "' And [ExactEntryDate] >= #" + fDate + "# And [ExactEntryDate] <= #" + tDate + "# And [Status] = 'Current'";
            }
            else
            {
                critString = "[ExactEntryDate] >= #" + fDate + "# And [ExactEntryDate] <= #" + tDate + "# And [Status] = 'Current'";
            }
            criteria = CriteriaOperator.Parse(critString);
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("[ExactEntryDate]", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            sortProps.Add(new SortProperty("[ReferenceNo]", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            false, _DriverPayrollBatch.Session.TypesManager);
            driverTripsType = _DriverPayrollBatch.Session.GetObjects(driverTripClass
            , criteria, sortProps, 0, false, true);

            if (driverTripsType.Count==0)
            {
                throw new UserFriendlyException("There are no registered driver trips found");
            }

            _FrmProgress = new ProgressForm("Generating data...", driverTripsType.Count,
            "Driver trip registration processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_DriverPayrollBatch);
            _FrmProgress.ShowDialog();

        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            DriverPayrollBatch _payrollBatch = (DriverPayrollBatch)e.Argument;
            DriverPayrollBatch thisIS = session.GetObjectByKey<DriverPayrollBatch>(
            _payrollBatch.Oid);
            int aCount = 0;

            try
            {
                // Filter Stanfilco Trips Order by EntryDate then Reference No
                DateTime fDate;
                DateTime tDate;
                string critString = string.Empty;
                DevExpress.Data.Filtering.CriteriaOperator criteria;
                DevExpress.Xpo.SortingCollection sortProps;
                DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
                ICollection driverTripsType;
                DevExpress.Xpo.Metadata.XPClassInfo driverTripClass;
                driverTripClass = thisIS.Session.GetClassInfo(typeof(
                DriverRegistry));
                fDate = new DateTime(thisIS.PeriodStart.Year, thisIS.PeriodStart.Month, thisIS.PeriodStart.Day, 01, 0, 0);
                tDate = new DateTime(thisIS.PeriodEnd.Year, thisIS.PeriodEnd.Month, thisIS.PeriodEnd.Day, 23, 59, 59); ;
                if (thisIS.Customer != null)
                {
                    critString = "[TripID.TripCustomer.No] = '" + thisIS.Customer.No + "' And [ExactEntryDate] >= #" + fDate + "# And [ExactEntryDate] <= #" + tDate + "# And [Status] = 'Current'";
                }
                else
                {
                    critString = "[ExactEntryDate] >= #" + fDate + "# And [ExactEntryDate] <= #" + tDate + "# And [Status] = 'Current'";
                }
                criteria = CriteriaOperator.Parse(critString);
                sortProps = new SortingCollection(null);
                sortProps.Add(new SortProperty("[ExactEntryDate]", DevExpress.Xpo.DB.
                SortingDirection.Ascending));
                sortProps.Add(new SortProperty("[ReferenceNo]", DevExpress.Xpo.DB.
                SortingDirection.Ascending));
                patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
                false, thisIS.Session.TypesManager);
                driverTripsType = thisIS.Session.GetObjects(driverTripClass
                , criteria, sortProps, 0, false, true);

                aCount = driverTripsType.Count;
                foreach (DriverRegistry item in driverTripsType)
                {

                    DriverPayrollTrip driverTrip=ReflectionHelper.CreateObject<DriverPayrollTrip>(thisIS.Session);
                    driverTrip.PayrollBatchID=thisIS;
                    driverTrip.TripDate=item.ExactEntryDate;
                    driverTrip.TripNo=item.TripID;
                    driverTrip.DocumentNo=item.ReferenceNo;
                    driverTrip.Driver=item.Driver;
                    driverTrip.RegID=item;
                    // Tariff Class
                    StringBuilder sb = new StringBuilder();
                    bool hasError = false;
                    sb.AppendFormat("Problems found in Driver Registry ID#{0}. ", item.Oid);
                    if (item.Tariff == null)
                    {
                        hasError = true;
                        sb.Append("Tariff is not specified and ");
                    }
                    if (item.Driver.DriverClassification == null)
                    {
                        hasError = true;
                        sb.AppendFormat("Driver {0} Driver Classification is not specified     ", item.Driver.Name);
                    }
                    if (hasError)
                    {
                        sb.Remove(sb.Length - 5, 5);
                        sb.Append(".");
                        throw new ApplicationException(sb.ToString());
                    }
                    //item.DriverClass = item.Driver.DriverClassification;
                    TariffDriversClassifier trfclass = thisIS.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse(string.Format("[TariffID.Code] = '{0}' And [DriverClass.Code] = '{1}'", item.Tariff.Code, item.Driver.DriverClassification.Code)));
                    //
                    if (trfclass==null)
                    {
                        throw new ApplicationException(string.Format("Tariff driver classifiers has not been set up for Tariff #{0}. Please check", item.Tariff.Code));
                    }
                    driverTrip.Basic = trfclass.BaseShare * (trfclass.ShareRate / 100);
                    driverTrip.AdlMiscExp = trfclass.BaseShare - driverTrip.Basic;

                    if (item.TripID.GetType()==typeof(StanfilcoTrip))
                    {
                        driverTrip.MiscExp=((StanfilcoTrip)item.TripID).Allowance;
                    }
                    if (item.TripID.GetType() == typeof(DolefilTrip))
                    {
                        driverTrip.MiscExp = ((DolefilTrip)item.TripID).Allowance.Value;
                    }
                    if (item.TripID.GetType() == typeof(OtherTrip))
                    {
                        driverTrip.MiscExp = ((OtherTrip)item.TripID).Allowance;
                    }
                    if (driverTrip.MiscExp == 0)
                    {
                        driverTrip.MiscExp = trfclass.TariffID.Allowance;
                    }
                    ICollection kds;
                    SortingCollection sorts = new SortingCollection(null);
                    DevExpress.Xpo.Metadata.XPClassInfo kdsClassInfo = thisIS.Session.GetClassInfo(typeof(KDEntry)); ;
                    kds = thisIS.Session.GetObjects(kdsClassInfo, CriteriaOperator.Parse(string.Format("[TripID.SourceNo] = '{0}'", item.TripID.SourceNo)), sorts, 0, false, true);
                    foreach (KDEntry kd in kds)
                    {
                        driverTrip.KDs = driverTrip.KDs + trfclass.KDShare;
                    }
                    
                    ICollection shunts;
                    DevExpress.Xpo.Metadata.XPClassInfo shuntingClassInfo = thisIS.Session.GetClassInfo(typeof(ShuntingEntry));
                    shunts = thisIS.Session.GetObjects(shuntingClassInfo, CriteriaOperator.Parse(string.Format("[TripID.SourceNo] = '{0}'", item.TripID.SourceNo)), sorts, 0, false, true);
                    //StringBuilder sbShunt = new StringBuilder();
                    foreach (ShuntingEntry sh in shunts)
                    {
                        driverTrip.Shunting=driverTrip.Shunting + trfclass.ShuntingShare;
                        //sbShunt.AppendFormat("{0},", sh.ThisShuntingTo.Code);
                    }
                    //if (sbShunt.ToString().LastOrDefault() == ',')
                    //    sbShunt.Remove(sb.Length - 1, 1);
                    //driverTrip.ShuntingTo = shunts.Count > 0 ? (item.TripID as StanfilcoTrip).Destination.Code:null;
                    if (item.TripID.GetType()==typeof(StanfilcoTrip))
                    {
                        driverTrip.ShuntingTo = (item.TripID as StanfilcoTrip).Origin.Code;
                    }
                    if (item.TripID.GetType() == typeof(DolefilTrip))
                    {
                        driverTrip.ShuntingTo = (item.TripID as DolefilTrip).Origin.Code;
                    }
                    if (item.TripID.GetType() == typeof(OtherTrip))
                    {
                        driverTrip.ShuntingTo = (item.TripID as OtherTrip).Origin.Code;
                    }
                    item.Status=DriverRegistryStatusEnum.Processed;
                    item.Save();
                    driverTrip.Save();
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;

                }


            }
            finally
            {
                if (index == aCount)
                {
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private UnitOfWork CreateUpdatingSession()
        {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }
        private void CommitUpdatingSession(UnitOfWork session)
        {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }
        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session)
        {
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                    , new SessionEventArgs(session));
            }
        }
        protected virtual void OnUpdatingSessionCreated(UnitOfWork session)
        {
            if
                (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                    SessionEventArgs(session));
            }
        }

        private void BgWorkerProgressChanged(object sender, 
        ProgressChangedEventArgs e) { if (_FrmProgress != null) {_FrmProgress.
                DoProgress(e.ProgressPercentage);} }
        private void BgWorkerRunWorkerCompleted(object sender, 
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Generation of payroll trips data is cancelled.", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "Payroll trips data has been successfully generated.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
