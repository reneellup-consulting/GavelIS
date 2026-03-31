using System;
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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateTOperations2 : ViewController {
        private SimpleAction generateTruckingOperations;
        private TruckingOperation2 _TruckingOperation;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateTOperations2() {
            this.TargetObjectType = typeof(TruckingOperation2);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GenerateTruckingOperations2";
            this.generateTruckingOperations = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.generateTruckingOperations.Caption = 
            "Generate Trucking Operations";
            this.generateTruckingOperations.Execute += new 
            SimpleActionExecuteEventHandler(
            GenerateTruckingOperationsAction_Execute);
        }

        private void GenerateTruckingOperationsAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _TruckingOperation = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as TruckingOperation2;
            try {
                for (int i = _TruckingOperation.Operations.Count - 1; i >= 0; i
                --) {_TruckingOperation.Operations[i].Delete();}
            } catch(Exception) {
            }
            ObjectSpace.CommitChanges();
            _FrmProgress = new ProgressForm("Generating data...", 12, 
            "Operation types processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_TruckingOperation);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) 
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            TruckingOperation2 _to = (TruckingOperation2)e.Argument;
            TruckingOperation2 thisIS = session.GetObjectByKey<TruckingOperation2>(
            _to.Oid);
            int aCount = 12;

            try
            {
                // Create Month Lines
                for (int i = 1; i < 13; i++)
                {
                    string gMonth = i == 1 ?
                        "01 JANUARY" :
                        i == 2 ?
                            "02 FEBRUARY" :
                            i == 3 ?
                                "03 MARCH" :
                                i == 4 ?
                                    "04 APRIL" :
                                    i == 5 ?
                                        "05 MAY" :
                                        i == 6 ?
                                            "06 JUNE" :
                                            i == 7 ?
                                                "07 JULY" :
                                                i == 8 ?
                                                    "08 AUGUST" :
                                                    i == 9 ?
                                                        "09 SEPTEMBER" :
                                                        i ==10 ?
                                                            "10 OCTOBER" :
                                                            i == 11 ?
                                                                "11 NOVEMBER" :
                                                                i == 12 ?
                                                                    "12 DECEMBER" 
                                                                    :
                                                                    string.Empty
;
                    TruckingOperation2Detail tod2 = ReflectionHelper.
                    CreateObject<TruckingOperation2Detail>(thisIS.Session);
                    tod2.TOID = thisIS;
                    tod2.Month = gMonth;

                    string critString = string.Empty;
                    DevExpress.Data.Filtering.CriteriaOperator criteria;
                    DevExpress.Xpo.SortingCollection sortProps;
                    DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;

                    // Get Stanfilco
                    ICollection sttrips;
                    DevExpress.Xpo.Metadata.XPClassInfo sttripsClass = null;
                    sttripsClass = thisIS.Session.GetClassInfo(typeof(StanfilcoTrip)
                    );
                    criteria = CriteriaOperator.Parse("[GYear]= " + thisIS.Year + " And [GMonthSorter] ='" + gMonth + "'");
                    sortProps = new SortingCollection(null);
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                    sttrips = thisIS.Session.GetObjects(sttripsClass, criteria,
                    sortProps, 0, false, true);
                    foreach (var st in sttrips)
                    {
                        StanfilcoTrip stanf = st as StanfilcoTrip;
                        tod2.Stanfilco = tod2.Stanfilco + stanf.NetBilling;

                        // --> Get Drivers Allowance
                        tod2.DriversAllowance = tod2.DriversAllowance + stanf.
                        Allowance;

                    }
                    // Get Dole
                    ICollection doletrips;
                    DevExpress.Xpo.Metadata.XPClassInfo doletripsClass = null;
                    doletripsClass = thisIS.Session.GetClassInfo(typeof(DolefilTrip)
                    );
                    criteria = CriteriaOperator.Parse("[GYear]= " + thisIS.Year + " And [GMonthSorter] ='" + gMonth + "'");
                    sortProps = new SortingCollection(null);
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                    doletrips = thisIS.Session.GetObjects(doletripsClass, criteria,
                    sortProps, 0, false, true);
                    foreach (var st in doletrips)
                    {
                        DolefilTrip stanf = st as DolefilTrip;
                        tod2.Dole = tod2.Dole + stanf.NetBilling;

                        tod2.DriversAllowance = tod2.DriversAllowance + stanf.
                        Allowance.Value;

                    }

                    // --> Get Drivers Allowance
                    // Get Others
                    ICollection OtherTrip;
                    DevExpress.Xpo.Metadata.XPClassInfo OtherTripClass = null;
                    OtherTripClass = thisIS.Session.GetClassInfo(typeof(OtherTrip)
                    );
                    criteria = CriteriaOperator.Parse("[GYear]= " + thisIS.Year + " And [GMonthSorter] ='" + gMonth + "'");
                    sortProps = new SortingCollection(null);
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                    OtherTrip = thisIS.Session.GetObjects(OtherTripClass, criteria,
                    sortProps, 0, false, true);
                    foreach (var st in OtherTrip)
                    {
                        OtherTrip stanf = st as OtherTrip;
                        tod2.Others = tod2.Others + stanf.GrossBilling;

                        tod2.DriversAllowance = tod2.DriversAllowance + stanf.
                        Allowance;

                    }

                    // --> Get Drivers Allowance
                    // Get IH Parts
                    ICollection IHPart;
                    DevExpress.Xpo.Metadata.XPClassInfo IHPartClass = null;
                    IHPartClass = thisIS.Session.GetClassInfo(typeof(Invoice)
                    );
                    criteria = CriteriaOperator.Parse("[GYear]= " + thisIS.Year + " And [GMonthSorter] ='" + gMonth + "'");
                    sortProps = new SortingCollection(null);
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                    IHPart = thisIS.Session.GetObjects(IHPartClass, criteria,
                    sortProps, 0, false, true);
                    foreach (var st in IHPart)
                    {
                        Invoice stanf = st as Invoice;
                        tod2.IHParts = tod2.IHParts + stanf.GrossTotal.Value;
                    }


                    // Get Tire and Battery
                    // Get Fuel
                    ICollection Fuels;
                    DevExpress.Xpo.Metadata.XPClassInfo FuelsClass = null;
                    FuelsClass = thisIS.Session.GetClassInfo(typeof(ReceiptFuel)
                    );
                    criteria = CriteriaOperator.Parse("[GYear]= " + thisIS.Year + " And [GMonthSorter] ='" + gMonth + "'");
                    sortProps = new SortingCollection(null);
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                    Fuels = thisIS.Session.GetObjects(FuelsClass, criteria,
                    sortProps, 0, false, true);
                    foreach (var st in Fuels)
                    {
                        ReceiptFuel stanf = st as ReceiptFuel;
                        tod2.Fuel = tod2.Fuel + stanf.Total.Value;
                    }

                    // Get Job Orders
                    ICollection Jobs;
                    DevExpress.Xpo.Metadata.XPClassInfo JobsClass = null;
                    JobsClass = thisIS.Session.GetClassInfo(typeof(JobOrder)
                    );
                    criteria = CriteriaOperator.Parse("[GYear]= " + thisIS.Year + " And [GMonthSorter] ='" + gMonth + "'");
                    sortProps = new SortingCollection(null);
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                    Jobs = thisIS.Session.GetObjects(JobsClass, criteria,
                    sortProps, 0, false, true);
                    foreach (var st in Jobs)
                    {
                        JobOrder stanf = st as JobOrder;
                        tod2.JobOrders = tod2.JobOrders + stanf.Total.Value;
                    }

                    // Get Parts and Services
                    ICollection Wos;
                    DevExpress.Xpo.Metadata.XPClassInfo WosClass = null;
                    WosClass = thisIS.Session.GetClassInfo(typeof(WorkOrder)
                    );
                    criteria = CriteriaOperator.Parse("[GYear]= " + thisIS.Year + " And [GMonthSorter] ='" + gMonth + "'");
                    sortProps = new SortingCollection(null);
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                    Wos = thisIS.Session.GetObjects(WosClass, criteria,
                    sortProps, 0, false, true);
                    foreach (var st in Wos)
                    {
                        WorkOrder stanf = st as WorkOrder;
                        tod2.PartsServices = tod2.PartsServices + stanf.TotalParts.Value;
                    }

                    tod2.Save();
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

        private UnitOfWork CreateUpdatingSession() {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }

        private void CommitUpdatingSession(UnitOfWork session) {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }

        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session) { 
            if (UpdatingSessionCommitted != null) {UpdatingSessionCommitted(this
                , new SessionEventArgs(session));} }

        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) { if 
            (UpdatingSessionCreated != null) {UpdatingSessionCreated(this, new 
                SessionEventArgs(session));} }

        private void BgWorkerProgressChanged(object sender, 
        ProgressChangedEventArgs e) { if (_FrmProgress != null) {_FrmProgress.
                DoProgress(e.ProgressPercentage);} }

        private void BgWorkerRunWorkerCompleted(object sender, 
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Generation of trucking operations data is cancelled.", 
                "Cancelled", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "Trucking operations data has been successfully generated.")
                    ;
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
