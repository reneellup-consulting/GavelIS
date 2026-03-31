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
    public partial class GenerateTruckingOperations : ViewController {
        private SimpleAction generateTruckingOperations;
        private TruckingOperation _TruckingOperation;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateTruckingOperations() {
            this.TargetObjectType = typeof(TruckingOperation);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GenerateTruckingOperations";
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
            CurrentObject as TruckingOperation;
            try {
                for (int i = _TruckingOperation.Operations.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.Operations[i].Delete(
                        );
                }
                // Delete trip lines
                for (int i = _TruckingOperation.TOTrips.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOTrips[i].Delete(
                        );
                }
                // Delete trailer lines
                for (int i = _TruckingOperation.TOTrailers.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOTrailers[i].Delete(
                        );
                }
                // Delete shunting lines
                for (int i = _TruckingOperation.TOShuntings.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOShuntings[i].Delete(
                        );
                }
                // Delete genset lines
                for (int i = _TruckingOperation.TOGensets.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOGensets[i].Delete(
                        );
                }
                // Delete KD lines
                for (int i = _TruckingOperation.TOKnockDowns.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOKnockDowns[i].Delete(
                        );
                }
                // Delete Fuel lines
                for (int i = _TruckingOperation.TOFuels.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOFuels[i].Delete(
                        );
                }
                // Delete Spare Part lines
                for (int i = _TruckingOperation.TOSpareParts.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOSpareParts[i].Delete(
                        );
                }
                // Delete Job Order lines
                for (int i = _TruckingOperation.TOJobOrders.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOJobOrders[i].Delete(
                        );
                }
                // Delete Tire lines
                for (int i = _TruckingOperation.TOTires.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOTires[i].Delete(
                        );
                }
                // Delete Battery lines
                for (int i = _TruckingOperation.TOBatterys.Count - 1;
                i >= 0; i--)
                {
                    _TruckingOperation.TOBatterys[i].Delete(
                        );
                }
            } catch(Exception) {
            }

            ObjectSpace.CommitChanges();

            _FrmProgress = new ProgressForm("Generating data...", 10, 
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
            TruckingOperation _to = (TruckingOperation)e.Argument;
            TruckingOperation thisIS = session.GetObjectByKey<TruckingOperation>(
            _to.Oid);
            string frm = thisIS.FromDate.Date.ToShortDateString();
            string to = thisIS.ToDate.Date.ToShortDateString();
            int aCount = 10;
            try {
                string critString = string.Empty;
                DevExpress.Data.Filtering.CriteriaOperator criteria;
                DevExpress.Xpo.SortingCollection sortProps;
                DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
                // Generate trip lines
                // --> Stanfilco Trips
                ICollection sttrips;
                DevExpress.Xpo.Metadata.XPClassInfo sttripsClass = null;
                sttripsClass = thisIS.Session.GetClassInfo(typeof(StanfilcoTrip)
                );
                if (thisIS.Fleet != null) {criteria = CriteriaOperator.Parse(
                    "[ExactEntryDate] >= #" + frm + 
                    "# And [ExactEntryDate] <= #" + to + 
                    "# And [SourceType.Code] = 'ST' And [TruckNo.SeriesNo] = '" 
                    + thisIS.Fleet.SeriesNo + "'");} else {
                        criteria = CriteriaOperator.Parse(
                            "[ExactEntryDate] >= #" + frm +
                            "# And [ExactEntryDate] <= #" + to +
                            "# And [SourceType.Code] = 'ST'");
                }
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.
                CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                sttrips = thisIS.Session.GetObjects(sttripsClass, criteria, 
                sortProps, 0, false, true);
                foreach (var st in sttrips) {
                    StanfilcoTrip stanf = st as StanfilcoTrip;
                    TOTrip ntrp = ReflectionHelper.CreateObject<TOTrip>(thisIS.
                    Session);
                    ntrp.TOID = thisIS;
                    ntrp.Fleet = stanf.TruckNo;
                    ntrp.Account = stanf.Customer;
                    ntrp.ReferenceNo = stanf;
                    ntrp.Date = stanf.ExactEntryDate;
                    ntrp.Amount = stanf.NetBilling;
                    ntrp.Save();

                    TruckingOperationDetail opers = ReflectionHelper.CreateObject<TruckingOperationDetail>(thisIS.
                    Session);
                    opers.TOID = thisIS;
                    opers.OperationType = TruckingOperationType.IncomeFromTrips;
                    opers.Fleet = stanf.TruckNo;
                    opers.Account = stanf.Customer;
                    opers.ReferenceNo = stanf;
                    opers.Date = stanf.ExactEntryDate;
                    opers.Amount = stanf.NetBilling;
                    opers.Save();

                    if (stanf.RentTrailer) {
                        TOTrailer trl = ReflectionHelper.CreateObject<TOTrailer>
                        (thisIS.Session);
                        trl.TOID = thisIS;
                        trl.Fleet = stanf.TruckNo;
                        trl.Account = stanf.Customer;
                        trl.ReferenceNo = stanf;
                        trl.Date = stanf.ExactEntryDate;
                        trl.Amount = stanf.Tariff.TrailerRental;
                        trl.Save();

                        TruckingOperationDetail trlopers = ReflectionHelper.CreateObject<TruckingOperationDetail>
                        (thisIS.Session);
                        trlopers.TOID = thisIS;
                        trlopers.OperationType = TruckingOperationType.
                        IncomeFromTrailers;
                        trlopers.Fleet = stanf.TruckNo;
                        trlopers.Account = stanf.Customer;
                        trlopers.ReferenceNo = stanf;
                        trlopers.Date = stanf.ExactEntryDate;
                        trlopers.Amount = stanf.Tariff.TrailerRental;
                        trlopers.Save();
                    }
                }
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                // --> Dolefil Trips
                ICollection dftrips;
                DevExpress.Xpo.Metadata.XPClassInfo dftripsClass = null;
                dftripsClass = thisIS.Session.GetClassInfo(typeof(DolefilTrip));
                if (thisIS.Fleet != null)
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [SourceType.Code] = 'DF' And [TruckNo.SeriesNo] = '"
                        + thisIS.Fleet.SeriesNo + "'");
                }
                else
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [SourceType.Code] = 'DF'");
                }
                //criteria = CriteriaOperator.Parse("[ExactEntryDate] >= #" + frm 
                //+ "# And [ExactEntryDate] <= #" + to + 
                //"# And [SourceType.Code] = 'DF'");
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.
                CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                dftrips = thisIS.Session.GetObjects(dftripsClass, criteria, 
                sortProps, 0, false, true);
                foreach (var df in dftrips) {
                    DolefilTrip dolf = df as DolefilTrip;
                    TOTrip ntrp = ReflectionHelper.CreateObject<TOTrip>(thisIS.
                    Session);
                    ntrp.TOID = thisIS;
                    ntrp.Fleet = dolf.TruckNo;
                    ntrp.Account = dolf.Customer;
                    ntrp.ReferenceNo = dolf;
                    ntrp.Date = dolf.ExactEntryDate;
                    ntrp.Amount = dolf.NetBilling;
                    ntrp.Save();

                    TruckingOperationDetail opers = ReflectionHelper.CreateObject<TruckingOperationDetail>(thisIS.
Session);
                    opers.TOID = thisIS;
                    opers.OperationType = TruckingOperationType.IncomeFromTrips;
                    opers.Fleet = dolf.TruckNo;
                    opers.Account = dolf.Customer;
                    opers.ReferenceNo = dolf;
                    opers.Date = dolf.ExactEntryDate;
                    opers.Amount = dolf.NetBilling;
                    opers.Save();

                    if (dolf.TrailerRental.Value != 0) {
                        TOTrailer trl = ReflectionHelper.CreateObject<TOTrailer>
                        (thisIS.Session);
                        trl.TOID = thisIS;
                        trl.Fleet = dolf.TruckNo;
                        trl.Account = dolf.Customer;
                        trl.ReferenceNo = dolf;
                        trl.Date = dolf.ExactEntryDate;
                        trl.Amount = dolf.TrailerRental.Value;
                        trl.Save();

                        TruckingOperationDetail trlopers = ReflectionHelper.CreateObject<TruckingOperationDetail>
(thisIS.Session);
                        trlopers.TOID = thisIS;
                        trlopers.OperationType = TruckingOperationType.
                        IncomeFromTrailers;
                        trlopers.Fleet = dolf.TruckNo;
                        trlopers.Account = dolf.Customer;
                        trlopers.ReferenceNo = dolf;
                        trlopers.Date = dolf.ExactEntryDate;
                        trlopers.Amount = dolf.TrailerRental.Value;
                        trlopers.Save();

                    }
                }
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                // --> Other Trips
                ICollection ottrips;
                DevExpress.Xpo.Metadata.XPClassInfo ottripsClass = null;
                ottripsClass = thisIS.Session.GetClassInfo(typeof(OtherTrip));
                if (thisIS.Fleet != null)
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [SourceType.Code] = 'OT' And [TruckNo.SeriesNo] = '"
                        + thisIS.Fleet.SeriesNo + "'");
                }
                else
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [SourceType.Code] = 'OT'");
                }

                //criteria = CriteriaOperator.Parse("[ExactEntryDate] >= #" + frm 
                //+ "# And [ExactEntryDate] <= #" + to + 
                //"# And [SourceType.Code] = 'OT'");
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.
                CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                ottrips = thisIS.Session.GetObjects(dftripsClass, criteria, 
                sortProps, 0, false, true);
                foreach (var ot in ottrips) {
                    OtherTrip other = ot as OtherTrip;
                    TOTrip ntrp = ReflectionHelper.CreateObject<TOTrip>(thisIS.
                    Session);
                    ntrp.TOID = thisIS;
                    ntrp.Fleet = other.TruckNo;
                    ntrp.Account = other.Customer;
                    ntrp.ReferenceNo = other;
                    ntrp.Date = other.ExactEntryDate;
                    ntrp.Amount = other.GrossBilling;
                    ntrp.Save();

                    TruckingOperationDetail opers = ReflectionHelper.CreateObject<TruckingOperationDetail>(thisIS.
Session);
                    opers.TOID = thisIS;
                    opers.OperationType = TruckingOperationType.IncomeFromTrips;
                    opers.Fleet = other.TruckNo;
                    opers.Account = other.Customer;
                    opers.ReferenceNo = other;
                    opers.Date = other.ExactEntryDate;
                    opers.Amount = other.GrossBilling;
                    opers.Save();

                    if (other.TrailerRental != 0) {
                        TOTrailer trl = ReflectionHelper.CreateObject<TOTrailer>
                        (thisIS.Session);
                        trl.TOID = thisIS;
                        trl.Fleet = other.TruckNo;
                        trl.Account = other.Customer;
                        trl.ReferenceNo = other;
                        trl.Date = other.ExactEntryDate;
                        trl.Amount = other.TrailerRental;
                        trl.Save();

                        TruckingOperationDetail trlopers = ReflectionHelper.CreateObject<TruckingOperationDetail>
(thisIS.Session);
                        trlopers.TOID = thisIS;
                        trlopers.OperationType = TruckingOperationType.
                        IncomeFromTrailers;
                        trlopers.Fleet = other.TruckNo;
                        trlopers.Account = other.Customer;
                        trlopers.ReferenceNo = other;
                        trlopers.Date = other.ExactEntryDate;
                        trlopers.Amount = other.TrailerRental;
                        trlopers.Save();

                    }
                }
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                // Generate shunting lines
                ICollection shunts;
                DevExpress.Xpo.Metadata.XPClassInfo shuntsClass = null;
                shuntsClass = thisIS.Session.GetClassInfo(typeof(ShuntingEntry))
                ;
                if (thisIS.Fleet != null)
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [TruckNo.SeriesNo] = '"
                        + thisIS.Fleet.SeriesNo + "'");
                }
                else
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "#");
                }

                //criteria = CriteriaOperator.Parse("[ExactEntryDate] >= #" + frm 
                //+ "# And [ExactEntryDate] <= #" + to + "#");
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.
                CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                shunts = thisIS.Session.GetObjects(shuntsClass, criteria, 
                sortProps, 0, false, true);
                foreach (var sh in shunts) {
                    ShuntingEntry shunt = sh as ShuntingEntry;
                    TOShunting ntrp = ReflectionHelper.CreateObject<TOShunting>(
                    thisIS.Session);
                    ntrp.TOID = thisIS;
                    ntrp.Fleet = shunt.TruckNo;
                    ntrp.Account = shunt.Customer;
                    ntrp.ReferenceNo = shunt;
                    ntrp.Date = shunt.ExactEntryDate;
                    ntrp.Amount = shunt.NetBilling;
                    ntrp.Save();

                    TruckingOperationDetail opers = ReflectionHelper.CreateObject<TruckingOperationDetail>(
thisIS.Session);
                    opers.TOID = thisIS;
                    opers.OperationType = TruckingOperationType.
                    IncomeFromShunting;
                    opers.Fleet = shunt.TruckNo;
                    opers.Account = shunt.Customer;
                    opers.ReferenceNo = shunt;
                    opers.Date = shunt.ExactEntryDate;
                    opers.Amount = shunt.NetBilling;
                    opers.Save();

                }
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                // Generate genset lines
                ICollection genst;
                DevExpress.Xpo.Metadata.XPClassInfo genstClass = null;
                genstClass = thisIS.Session.GetClassInfo(typeof(GensetEntry));
                if (thisIS.Fleet != null)
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [TruckNo.SeriesNo] = '"
                        + thisIS.Fleet.SeriesNo + "'");
                }
                else
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "#");
                }

                //criteria = CriteriaOperator.Parse("[ExactEntryDate] >= #" + frm 
                //+ "# And [ExactEntryDate] <= #" + to + "#");
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.
                CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                genst = thisIS.Session.GetObjects(genstClass, criteria, 
                sortProps, 0, false, true);
                foreach (var gs in genst) {
                    GensetEntry gens = gs as GensetEntry;
                    TOGenset ntrp = ReflectionHelper.CreateObject<TOGenset>(
                    thisIS.Session);
                    ntrp.TOID = thisIS;
                    ntrp.Fleet = gens.TruckNo;
                    ntrp.Account = gens.Customer;
                    ntrp.ReferenceNo = gens;
                    ntrp.Date = gens.ExactEntryDate;
                    ntrp.Amount = gens.NetBilling;
                    ntrp.Save();

                    TruckingOperationDetail opers = ReflectionHelper.CreateObject<TruckingOperationDetail>(
thisIS.Session);
                    opers.TOID = thisIS;
                    opers.OperationType = TruckingOperationType.
                    IncomeFromGensets;
                    opers.Fleet = gens.TruckNo;
                    opers.Account = gens.Customer;
                    opers.ReferenceNo = gens;
                    opers.Date = gens.ExactEntryDate;
                    opers.Amount = gens.NetBilling;
                    opers.Save();

                }
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                // Generate kd lines
                ICollection kds;
                DevExpress.Xpo.Metadata.XPClassInfo kdsClass = null;
                kdsClass = thisIS.Session.GetClassInfo(typeof(KDEntry));
                if (thisIS.Fleet != null)
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [TruckNo.SeriesNo] = '"
                        + thisIS.Fleet.SeriesNo + "'");
                }
                else
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "#");
                }

                //criteria = CriteriaOperator.Parse("[ExactEntryDate] >= #" + frm 
                //+ "# And [ExactEntryDate] <= #" + to + "#");
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.
                CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                kds = thisIS.Session.GetObjects(kdsClass, criteria, sortProps, 0
                , false, true);
                foreach (var kd in kds) {
                    KDEntry gens = kd as KDEntry;
                    TOKnockDown ntrp = ReflectionHelper.CreateObject<TOKnockDown
                    >(thisIS.Session);
                    ntrp.TOID = thisIS;
                    ntrp.Fleet = gens.TruckNo;
                    ntrp.Account = gens.Customer;
                    ntrp.ReferenceNo = gens;
                    ntrp.Date = gens.ExactEntryDate;
                    ntrp.Amount = gens.NetBilling;
                    ntrp.Save();

                    TruckingOperationDetail opers = ReflectionHelper.CreateObject<TruckingOperationDetail
>(thisIS.Session);
                    opers.TOID = thisIS;
                    opers.OperationType = TruckingOperationType.IncomeFromKDS;
                    opers.Fleet = gens.TruckNo;
                    opers.Account = gens.Customer;
                    opers.ReferenceNo = gens;
                    opers.Date = gens.ExactEntryDate;
                    opers.Amount = gens.NetBilling;
                    opers.Save();

                }
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                // Generate fuel lines
                ICollection fuel;
                DevExpress.Xpo.Metadata.XPClassInfo fuelClass = null;
                fuelClass = thisIS.Session.GetClassInfo(typeof(ReceiptFuel));
                if (thisIS.Fleet != null)
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [TruckNo.SeriesNo] = '"
                        + thisIS.Fleet.SeriesNo + "'");
                }
                else
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "#");
                }

                //criteria = CriteriaOperator.Parse("[ExactEntryDate] >= #" + frm 
                //+ "# And [ExactEntryDate] <= #" + to + "#");
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.
                CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                fuel = thisIS.Session.GetObjects(fuelClass, criteria, sortProps, 
                0, false, true);
                foreach (var fl in fuel) {
                    ReceiptFuel fls = fl as ReceiptFuel;
                    TOFuel ntrp = ReflectionHelper.CreateObject<TOFuel>(thisIS.
                    Session);
                    ntrp.TOID = thisIS;
                    ntrp.Fleet = fls.TruckNo;
                    ntrp.Account = fls.Vendor;
                    ntrp.ReferenceNo = fls;
                    ntrp.Date = fls.ExactEntryDate;
                    ntrp.Amount = fls.Total.Value;
                    ntrp.Save();

                    TruckingOperationDetail opers = ReflectionHelper.CreateObject<TruckingOperationDetail>(thisIS.
Session);
                    opers.TOID = thisIS;
                    opers.OperationType = TruckingOperationType.ExpensesFromFuel
                    ;
                    opers.Fleet = fls.TruckNo;
                    opers.Account = fls.Vendor;
                    opers.ReferenceNo = fls;
                    opers.Date = fls.ExactEntryDate;
                    opers.Amount = fls.Total.Value;
                    opers.Save();

                }
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                // Generate Spare Part lines
                ICollection work;
                DevExpress.Xpo.Metadata.XPClassInfo workClass = null;
                workClass = thisIS.Session.GetClassInfo(typeof(WorkOrder));
                if (thisIS.Fleet != null)
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "# And [Fleet.PlateNo] = '"
                        + thisIS.Fleet.PlateNo + "'");
                }
                else
                {
                    criteria = CriteriaOperator.Parse(
                        "[ExactEntryDate] >= #" + frm +
                        "# And [ExactEntryDate] <= #" + to +
                        "#");
                }

                //criteria = CriteriaOperator.Parse("[ExactEntryDate] >= #" + frm 
                //+ "# And [ExactEntryDate] <= #" + to + "#");
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.
                CollectionCriteriaPatcher(false, thisIS.Session.TypesManager);
                work = thisIS.Session.GetObjects(workClass, criteria, sortProps, 
                0, false, true);
                foreach (var wo in work) {
                    WorkOrder wos = wo as WorkOrder;
                    if (wos.Fleet != null && wos.Fleet.GetType() == typeof(
                    FATruck)) {
                        TOSparePart ntrp = ReflectionHelper.CreateObject<
                        TOSparePart>(thisIS.Session);
                        ntrp.TOID = thisIS;
                        ntrp.Fleet = wos.Fleet as FATruck;
                        ntrp.Account = wos.Driver;
                        ntrp.ReferenceNo = wos;
                        ntrp.Date = wos.ExactEntryDate;
                        ntrp.Amount = wos.TotalParts.Value;
                        ntrp.Save();

                        TruckingOperationDetail opers = ReflectionHelper.CreateObject<
TruckingOperationDetail>(thisIS.Session);
                        opers.TOID = thisIS;
                        opers.OperationType = TruckingOperationType.
                        ExpensesFromSpareParts;
                        opers.Fleet = wos.Fleet as FATruck;
                        opers.Account = wos.Driver;
                        opers.ReferenceNo = wos;
                        opers.Date = wos.ExactEntryDate;
                        opers.Amount = wos.TotalParts.Value;
                        opers.Save();

                        if (wos.TotalWithJO != 0) {
                            foreach (var jr in wos.JobOrderDetails) {
                                TOJobOrder jor = ReflectionHelper.CreateObject<
                                TOJobOrder>(thisIS.Session);
                                jor.TOID = thisIS;
                                jor.Fleet = wos.Fleet as FATruck;
                                jor.Account = jr.JobOrderInfo.Vendor;
                                jor.ReferenceNo = jr.JobOrderInfo;
                                jor.Date = jr.JobOrderInfo.ExactEntryDate;
                                jor.Amount = jr.JobOrderInfo.Total.Value;
                                jor.Save();

                                TruckingOperationDetail joropers = ReflectionHelper.CreateObject<
TruckingOperationDetail>(thisIS.Session);
                                joropers.TOID = thisIS;
                                joropers.OperationType = TruckingOperationType.
                                ExpensesFromJobOrders;
                                joropers.Fleet = wos.Fleet as FATruck;
                                joropers.Account = jr.JobOrderInfo.Vendor;
                                joropers.ReferenceNo = jr.JobOrderInfo;
                                joropers.Date = jr.JobOrderInfo.ExactEntryDate;
                                joropers.Amount = jr.JobOrderInfo.Total.Value;
                                joropers.Save();

                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                //// Generate Job Order lines
                //ICollection jorder;
                //DevExpress.Xpo.Metadata.XPClassInfo jorderClass = null;
                //jorderClass = thisIS.Session.GetClassInfo(typeof(
                //JobOrder));
                //criteria = CriteriaOperator
                //    .Parse("[ExactEntryDate] >= #" + frm + "# And [ExactEntryDate] <= #" + to + "#");
                //sortProps = new SortingCollection(null);
                //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
                //false, thisIS.Session.TypesManager);
                //jorder = thisIS.Session.GetObjects(jorderClass
                //, criteria, sortProps, 0, false, true);
                //foreach (var jo in jorder)
                //{
                //    JobOrder jos = jo as JobOrder;
                //    TOJobOrder ntrp = ReflectionHelper.CreateObject<TOJobOrder>(thisIS.Session)
                //    ;
                //    ntrp.TOID = thisIS;
                //    //ntrp.Fleet = jos.;
                //    ntrp.Account = jos.Vendor;
                //    ntrp.ReferenceNo = jos;
                //    ntrp.Date = jos.ExactEntryDate;
                //    ntrp.Amount = jos.Total.Value;
                //    ntrp.Save();
                //}
                //System.Threading.Thread.Sleep(20);
                //_BgWorker.ReportProgress(1, _message);
                //index++;
                // Generate Tire lines
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
                // Generate Battery lines
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
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
        ProgressChangedEventArgs e)
        {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                    DoProgress(e.ProgressPercentage);
            }
        }
        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Generation of trucking operations data is cancelled.", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                        Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show(
                    "Trucking operations data has been successfully generated.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
