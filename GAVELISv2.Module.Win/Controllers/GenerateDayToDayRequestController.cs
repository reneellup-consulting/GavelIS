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
    public partial class GenerateDayToDayRequestController : ViewController
    {
        private SimpleAction generateDayToDayRequest;
        private DayToDayItemRequestHeader _Header;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateDayToDayRequestController()
        {
            this.TargetObjectType = typeof(DayToDayItemRequestHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DayToDayItemRequestHeader.GenerateDayToDayRequest";
            this.generateDayToDayRequest = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.generateDayToDayRequest.Caption = "Generate";
            this.generateDayToDayRequest.Execute += new
            SimpleActionExecuteEventHandler(
            generateDayToDayRequest_Execute);
        }
        List<DayToDayRequestTmp> dayToDayRequestTmp = new List<DayToDayRequestTmp>();
        private void generateDayToDayRequest_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _Header = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as DayToDayItemRequestHeader;

            try
            {
                for (int i = _Header.DayToDayItemRequestDetails.Count - 1;
                i >= 0; i--)
                {
                    _Header.DayToDayItemRequestDetails[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();
            dayToDayRequestTmp.Clear();
            // [Entry Date] >= #2018-01-01# And [Entry Date] < #2018-02-01#
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now; ;
            switch (_Header.GenerationType)
            {
                case DtdGenerationTypeEnum.Monthly:
                    startDate = new DateTime(_Header.Year, (int)_Header.Month, 1);
                    endDate = (new DateTime(_Header.Year, (int)_Header.Month, DateTime.DaysInMonth(_Header.Year, (int)_Header.Month))).AddDays(1);
                    break;
                case DtdGenerationTypeEnum.Range:
                    startDate = _Header.StartDate;
                    endDate = _Header.EndDate.AddDays(1);
                    break;
                default:
                    break;
            }
            switch (_Header.ReportType)
            {
                case DtdReportTypeEnum.ItemsReleases:
                    string crit = string.Format("[EntryDate] >= #{0}# And [EntryDate] < #{1}# And [SourceType] In ('IN','DM','WO','RQ','FPR','ECS')", startDate.ToString("yyy-MM-dd"), endDate.ToString("yyy-MM-dd"));
                    IList<DayToDayItemReports> dtdir = ObjectSpace.GetObjects<DayToDayItemReports>(CriteriaOperator.Parse(crit));
                    //int lngth = dtdir.Count;
                    foreach (var item in dtdir)
                    {
                        InventoryControlJournal icj = ObjectSpace.GetObjectByKey<InventoryControlJournal>(item.OID);
                        if (item.SourceNo ==  "RQ0000099137")
                        {
                            Console.WriteLine("Trap");
                        }
                        Employee naEmp = ObjectSpace.GetObjectByKey<Employee>(new Guid("140e189e-6d9e-48f2-9455-51d38e5d8e39"));
                        CostCenter naCostCenter = ObjectSpace.GetObjectByKey<CostCenter>(new Guid("e67be00e-e88a-4045-8b6d-a44c89b1b9e1"));
                        Employee requestedBy = icj.RequestedBy != null ? icj.RequestedBy : naEmp;
                        DateTime dateIssued = icj.DateIssued;
                        CostCenter issuedTo = icj.CostCenter != null ? icj.CostCenter : naCostCenter;
                        decimal sQty = 0;
                        MovementTypeEnum mType = MovementTypeEnum.None;
                        decimal sCost = 0;
                        UnitOfMeasure sUom = null;
                        Requisition treq = null;
                        decimal sPrice = 0;
                        switch (item.SourceType)
                        {
                            case "IN":
                                InvoiceDetail invd = ObjectSpace.FindObject<InvoiceDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                sQty = invd.Quantity;
                                mType = MovementTypeEnum.Issued;
                                sCost = invd.Cost;
                                sUom = invd.UOM;
                                sPrice = invd.Price;
                                break;
                            case "DM":
                                DebitMemoDetail dmd = ObjectSpace.FindObject<DebitMemoDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                sQty = dmd.Quantity;
                                mType = MovementTypeEnum.Issued;
                                sCost = dmd.Cost;
                                sUom = dmd.UOM;
                                break;
                            case "WO":
                                WorkOrderItemDetail wod = ObjectSpace.FindObject<WorkOrderItemDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                sQty = wod.Quantity;
                                mType = MovementTypeEnum.Issued;
                                sUom = wod.UOM;
                                sPrice = wod.Price;
                                break;
                            case "RQ":
                                RwsTireDetail rwstd = ObjectSpace.FindObject<RwsTireDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                // RequisitionWorksheet rqd = ObjectSpace.FindObject<RequisitionWorksheet>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                sQty = 1;
                                mType = MovementTypeEnum.Issued;
                                sCost = rwstd.Cost;
                                sPrice = rwstd.Cost;
                                sUom = icj.UOM;
                                treq = rwstd.ReqWorksheetId.GenJournalID as Requisition;
                                break;
                            case "FPR":
                                FuelPumpRegisterDetail fprd = ObjectSpace.FindObject<FuelPumpRegisterDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                sQty = fprd.Quantity;
                                mType = MovementTypeEnum.Issued;
                                sCost = fprd.Cost;
                                sUom = fprd.UOM;
                                break;
                            case "ECS":
                                EmployeeChargeSlipItemDetail ecsd = ObjectSpace.FindObject<EmployeeChargeSlipItemDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                sQty = ecsd.Quantity;
                                mType = MovementTypeEnum.Issued;
                                sCost = ecsd.Cost;
                                sUom = ecsd.UOM;
                                sPrice = ecsd.Price;
                                requestedBy = icj.RequisitionNo !=null ? icj.RequisitionNo.RequestedBy : ecsd.EmployeeChargeSlipInfo.Employee;
                                dateIssued = ecsd.EmployeeChargeSlipInfo.EntryDate;
                                issuedTo = ObjectSpace.FindObject<CostCenter>(BinaryOperator.Parse("[Code]=?", requestedBy.No)); //ecsd.EmployeeChargeSlipInfo.Empl;
                                break;
                            default:
                                break;
                        }
                        dayToDayRequestTmp.Add(new DayToDayRequestTmp()
                        {
                            IcjID = icj,
                            EntryDate = icj.EntryDate,
                            Source = icj.GenJournalID,
                            SourceQty = sQty,
                            IcjQty = icj.Qty,
                            MovementType = mType,
                            Item = icj.ItemNo,
                            SourceCost = sCost,
                            SourceUOM = sUom,
                            SourcePrice = sPrice,
                            Whse = icj.Warehouse,
                            IcjCost = icj.Cost,
                            IcjUOM = icj.UOM,
                            IcjPrice = icj.Price,
                            Request = treq != null ? treq : icj.RequisitionNo,
                            RequestedBy = requestedBy,
                            DateIssued = dateIssued,
                            IssuedTo = issuedTo,
                        });
                    }
                    break;
                case DtdReportTypeEnum.ItemsReceipts:
                    string crit2 = string.Format("[EntryDate] >= #{0}# And [EntryDate] < #{1}# And [SourceType] In ('RC','CM')", startDate.ToString("yyy-MM-dd"), endDate.ToString("yyy-MM-dd"));
                    IList<DayToDayItemReports> dtdir2 = ObjectSpace.GetObjects<DayToDayItemReports>(CriteriaOperator.Parse(crit2));
                    //int lngth = dtdir.Count;
                    foreach (var item in dtdir2)
                    {
                        InventoryControlJournal icj = ObjectSpace.GetObjectByKey<InventoryControlJournal>(item.OID);
                        if (icj != null && icj.CostCenter != null && icj.CostCenter.IsStocking)
                        {
                            continue;
                        }
                        decimal sQty = 0;
                        MovementTypeEnum mType = MovementTypeEnum.None;
                        decimal sCost = 0;
                        UnitOfMeasure sUom = null;
                        decimal sPrice = 0;
                        switch (item.SourceType)
                        {
                            case "RC":
                                ReceiptDetail rcptd = ObjectSpace.FindObject<ReceiptDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                sQty = rcptd.Quantity;
                                mType = MovementTypeEnum.Receipt;
                                sCost = rcptd.Cost;
                                sUom = rcptd.UOM;
                                break;
                            case "CM":
                                CreditMemoDetail cmd = ObjectSpace.FindObject<CreditMemoDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                sQty = cmd.Quantity;
                                mType = MovementTypeEnum.Receipt;
                                sUom = cmd.UOM;
                                sPrice = cmd.Price;
                                break;
                            default:
                                break;
                        }
                        dayToDayRequestTmp.Add(new DayToDayRequestTmp()
                        {
                            IcjID = icj,
                            EntryDate = icj.EntryDate,
                            Source = icj.GenJournalID,
                            SourceQty = sQty,
                            IcjQty = icj.Qty,
                            MovementType = mType,
                            Item = icj.ItemNo,
                            SourceCost = sCost,
                            SourceUOM = sUom,
                            SourcePrice = sPrice,
                            Whse = icj.Warehouse,
                            IcjCost = icj.Cost,
                            IcjUOM = icj.UOM,
                            IcjPrice = icj.Price,
                            Request = icj.RequisitionNo,
                            RequestedBy = icj.RequestedBy,
                            DateIssued = icj.DateIssued,
                            IssuedTo = icj.CostCenter,
                        });
                    }
                    break;
                case DtdReportTypeEnum.ItemsForStocking:
                    string crit3 = string.Format("[EntryDate] >= #{0}# And [EntryDate] < #{1}# And [SourceType] In ('RC','CM')", startDate.ToString("yyy-MM-dd"), endDate.ToString("yyy-MM-dd"));
                    IList<DayToDayItemReports> dtdir3 = ObjectSpace.GetObjects<DayToDayItemReports>(CriteriaOperator.Parse(crit3));
                    //int lngth = dtdir.Count;
                    foreach (var item in dtdir3)
                    {
                        InventoryControlJournal icj = ObjectSpace.GetObjectByKey<InventoryControlJournal>(item.OID);
                        if (icj != null && icj.CostCenter != null && icj.CostCenter.IsStocking)
                        {
                            decimal sQty = 0;
                            MovementTypeEnum mType = MovementTypeEnum.None;
                            decimal sCost = 0;
                            UnitOfMeasure sUom = null;
                            decimal sPrice = 0;
                            switch (item.SourceType)
                            {
                                case "RC":
                                    ReceiptDetail rcptd = ObjectSpace.FindObject<ReceiptDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                    sQty = rcptd.Quantity;
                                    mType = MovementTypeEnum.Receipt;
                                    sCost = rcptd.Cost;
                                    sUom = rcptd.UOM;
                                    break;
                                case "CM":
                                    CreditMemoDetail cmd = ObjectSpace.FindObject<CreditMemoDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID));
                                    sQty = cmd.Quantity;
                                    mType = MovementTypeEnum.Receipt;
                                    sUom = cmd.UOM;
                                    sPrice = cmd.Price;
                                    break;
                                default:
                                    break;
                            }
                            dayToDayRequestTmp.Add(new DayToDayRequestTmp()
                            {
                                IcjID = icj,
                                EntryDate = icj.EntryDate,
                                Source = icj.GenJournalID,
                                SourceQty = sQty,
                                IcjQty = icj.Qty,
                                MovementType = mType,
                                Item = icj.ItemNo,
                                SourceCost = sCost,
                                SourceUOM = sUom,
                                SourcePrice = sPrice,
                                Whse = icj.Warehouse,
                                IcjCost = icj.Cost,
                                IcjUOM = icj.UOM,
                                IcjPrice = icj.Price,
                                Request = icj.RequisitionNo,
                                RequestedBy = icj.RequestedBy,
                                DateIssued = icj.DateIssued,
                                IssuedTo = icj.CostCenter,
                            });
                        }
                    }
                    break;
                default:
                    break;
            }

            if (dayToDayRequestTmp.Count == 0)
            {
                throw new UserFriendlyException("There are no records found");
            }

            _FrmProgress = new ProgressForm("Generating data...", dayToDayRequestTmp.Count,
            "Records processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(dayToDayRequestTmp);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            List<DayToDayRequestTmp> _list = (List<DayToDayRequestTmp>)e.Argument;
            DayToDayItemRequestHeader dtdItemReq = session.GetObjectByKey<DayToDayItemRequestHeader>(_Header.Oid);
            try
            {
                foreach (DayToDayRequestTmp item in _list)
                {
                    index++;
                    _message = string.Format("Processing record {0} succesfull.",
                    item.IcjID.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    if (item.Source.Code=="IN0000012180")
                    {
                        Console.WriteLine("Trap");
                    }
                    InventoryControlJournal icjobj = session.GetObjectByKey<InventoryControlJournal>(item.IcjID.Oid);
                    DayToDayItemRequestDetail dtdird = ReflectionHelper.CreateObject<DayToDayItemRequestDetail>(session);
                    dtdird.ParentID = dtdItemReq;
                    dtdird.IcjID = icjobj;
                    dtdird.EntryDate = item.EntryDate;
                    dtdird.Source = icjobj.GenJournalID;
                    dtdird.SourceQty = item.SourceQty;
                    dtdird.IcjQty = item.IcjQty;
                    dtdird.AbsQty = Math.Abs(item.IcjQty);
                    dtdird.MovementType = item.MovementType;
                    dtdird.Item = icjobj.ItemNo;
                    dtdird.SourceCost = item.SourceCost;
                    dtdird.SourceUOM = item.SourceUOM != null ? session.GetObjectByKey<UnitOfMeasure>(item.SourceUOM.Oid) : null;
                    dtdird.SourcePrice = item.SourcePrice;
                    dtdird.Whse = icjobj.Warehouse;
                    dtdird.IcjCost = item.IcjCost;
                    dtdird.ICjUOM = icjobj.UOM;
                    dtdird.IcjPrice = item.IcjPrice;
                    dtdird.Request = item.Request != null ? session.GetObjectByKey<Requisition>(item.Request.Oid) : null;
                    dtdird.RequestedBy = item.RequestedBy != null ? session.GetObjectByKey<Employee>(item.RequestedBy.Oid) : null;
                    dtdird.DateIssued = item.DateIssued != DateTime.MinValue ? item.DateIssued : item.EntryDate;
                    dtdird.IssuedTo = item.IssuedTo != null ? session.GetObjectByKey<CostCenter>(item.IssuedTo.Oid) : null;
                    dtdird.Save();
                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                }

            }
            finally
            {
                if (index == _list.Count)
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
                    "Generation of Day to Day Item Request data is cancelled.", "Cancelled",
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
                    "Day to Day Item Request data has been successfully generated.");
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

    public class DayToDayRequestTmp
    {
        // IcjID
        public InventoryControlJournal IcjID { get; set; }
        // EntryDate
        public DateTime EntryDate { get; set; }
        // Source
        public GenJournalHeader Source { get; set; }
        // SourceQty
        public decimal SourceQty { get; set; }
        // IcjQty
        public decimal IcjQty { get; set; }
        // MovementType
        public MovementTypeEnum MovementType { get; set; }
        // Item
        public Item Item { get; set; }
        // SourceCost
        public decimal SourceCost { get; set; }
        // SourceUOM
        public UnitOfMeasure SourceUOM { get; set; }
        // SourcePrice
        public decimal SourcePrice { get; set; }
        // Whse
        public Warehouse Whse { get; set; }
        // IcjCost
        public decimal IcjCost { get; set; }
        // IcjUOM
        public UnitOfMeasure IcjUOM { get; set; }
        // IcjPrice
        public decimal IcjPrice { get; set; }
        // Request
        public Requisition Request { get; set; }
        // RequestedBy
        public Employee RequestedBy { get; set; }
        // DateIssued
        public DateTime DateIssued { get; set; }
        // IssuedTo
        public CostCenter IssuedTo { get; set; }
    }
}