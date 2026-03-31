using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class PurchasesMovementBuffer : XPObject
    {
        private Guid _RowID;
        private PurchasesMovementAnalysis _HeaderId;
        private InventoryControlJournal _IcjID;
        private string _SeqID;
        private string _SeqStr;
        private GenJournalHeader _Source;
        private DateTime _EntryDate;
        private DateTime _PostedDate;
        private BusinessObjects.SourceType _SourceType;
        private BusinessObjects.OperationType _OperationType;
        private decimal _InQty;
        private decimal _OutQty;
        private decimal _QtyValue;
        private BusinessObjects.Item _Item;
        private decimal _Cost;
        private UnitOfMeasure _Uom;
        private BusinessObjects.Warehouse _Warehouse;
        private string _IcjRowID;
        private BusinessObjects.Requisition _Requisition;
        private Employee _RequestedBy;
        private CostCenter _ChargeTo;
        private decimal _Price;
        private DateTime _IssueDate;
        private Contact _IssuedTo;
        private ContactTypeEnum _IssuedType;
        private BusinessObjects.ExpenseType _ExpenseType;
        private BusinessObjects.SubExpenseType _SubExpenseType;
        private decimal _Income;
        private decimal _Expense;
        private decimal _WhseRunQty;

        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("PurchasesMovement-Buffer")]
        public PurchasesMovementAnalysis HeaderId
        {
            get { return _HeaderId; }
            set { SetPropertyValue("HeaderId", ref _HeaderId, value); }
        }

        // IcjID
        public InventoryControlJournal IcjID
        {
            get { return _IcjID; }
            set { SetPropertyValue("IcjID", ref _IcjID, value); }
        }
        // SeqID
        public string SeqID
        {
            get { return _SeqID; }
            set { SetPropertyValue("SeqID", ref _SeqID, value); }
        }
        // SeqStr
        public string SeqStr
        {
            get { return _SeqStr; }
            set { SetPropertyValue("SeqStr", ref _SeqStr, value); }
        }
        // Source = GenJournalID
        public GenJournalHeader Source
        {
            get { return _Source; }
            set { SetPropertyValue("Source", ref _Source, value); }
        }
        // EntryDate
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        // PostedDate
        public DateTime PostedDate
        {
            get { return _PostedDate; }
            set { SetPropertyValue("PostedDate", ref _PostedDate, value); }
        }
        // SourceType
        public SourceType SourceType
        {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }
        // OperationType
        public OperationType OperationType
        {
            get { return _OperationType; }
            set { SetPropertyValue("OperationType", ref _OperationType, value); }
        }
        // InQty
        public decimal InQty
        {
            get { return _InQty; }
            set { SetPropertyValue("InQty", ref _InQty, value); }
        }
        // OutQty
        public decimal OutQty
        {
            get { return _OutQty; }
            set { SetPropertyValue("OutQty", ref _OutQty, value); }
        }
        // QtyValue = If not PI then (InQty - OutQty) else PI.ActualQty
        public decimal QtyValue
        {
            get { return _QtyValue; }
            set { SetPropertyValue("QtyValue", ref _QtyValue, value); }
        }
        // Item
        public Item Item
        {
            get { return _Item; }
            set { SetPropertyValue("Item", ref _Item, value); }
        }
        // Cost
        public decimal Cost
        {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        // Uom
        public UnitOfMeasure Uom
        {
            get { return _Uom; }
            set { SetPropertyValue("Uom", ref _Uom, value); }
        }
        // Warehouse
        public Warehouse Warehouse
        {
            get { return _Warehouse; }
            set { SetPropertyValue("Warehouse", ref _Warehouse, value); }
        }
        // IcjRowID
        public string IcjRowID
        {
            get { return _IcjRowID; }
            set { SetPropertyValue("IcjRowID", ref _IcjRowID, value); }
        }
        // Requisition
        public Requisition Requisition
        {
            get { return _Requisition; }
            set { SetPropertyValue("Requisition", ref _Requisition, value); }
        }
        // RequestedBy
        public Employee RequestedBy
        {
            get { return _RequestedBy; }
            set { SetPropertyValue("RequestedBy", ref _RequestedBy, value); }
        }
        // ChargeTo
        public CostCenter ChargeTo
        {
            get { return _ChargeTo; }
            set { SetPropertyValue("ChargeTo", ref _ChargeTo, value); }
        }
        // Price
        public decimal Price
        {
            get { return _Price; }
            set { SetPropertyValue("Price", ref _Price, value); }
        }
        // IssueDate
        public DateTime IssueDate
        {
            get { return _IssueDate; }
            set { SetPropertyValue("IssueDate", ref _IssueDate, value); }
        }
        // IssuedType
        public ContactTypeEnum IssuedType
        {
            get { return _IssuedType; }
            set { SetPropertyValue("IssuedType", ref _IssuedType, value); }
        }
        // IssuedTo
        public Contact IssuedTo
        {
            get { return _IssuedTo; }
            set { SetPropertyValue("IssuedTo", ref _IssuedTo, value); }
        }
        // ExpenseType
        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set { SetPropertyValue("ExpenseType", ref _ExpenseType, value); }
        }
        // SubExpenseType
        public SubExpenseType SubExpenseType
        {
            get { return _SubExpenseType; }
            set { SetPropertyValue("SubExpenseType", ref _SubExpenseType, value); }
        }
        public decimal Income
        {
            get { return _Income; }
            set { SetPropertyValue("Income", ref _Income, value); }
        }
        public decimal Expense
        {
            get { return _Expense; }
            set { SetPropertyValue("Expense", ref _Expense, value); }
        }

        public decimal WhseRunQty
        {
            get { return _WhseRunQty; }
            set { SetPropertyValue("WhseRunQty", ref _WhseRunQty, value); }
        }

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum GMonth
        {
            get
            {
                _Month = EntryDate.Month == 1 ? MonthsEnum.January : EntryDate.Month
                 == 2 ? MonthsEnum.February : EntryDate.Month == 3 ? MonthsEnum.
                March : EntryDate.Month == 4 ? MonthsEnum.April : EntryDate.Month ==
                5 ? MonthsEnum.May : EntryDate.Month == 6 ? MonthsEnum.June :
                EntryDate.Month == 7 ? MonthsEnum.July : EntryDate.Month == 8 ?
                MonthsEnum.August : EntryDate.Month == 9 ? MonthsEnum.September
                 : EntryDate.Month == 10 ? MonthsEnum.October : EntryDate.Month == 11
                 ? MonthsEnum.November : EntryDate.Month == 12 ? MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal January
        {
            get { return GMonth == MonthsEnum.January ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal February
        {
            get { return GMonth == MonthsEnum.February ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal March
        {
            get { return GMonth == MonthsEnum.March ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal April
        {
            get { return GMonth == MonthsEnum.April ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal May
        {
            get { return GMonth == MonthsEnum.May ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal June
        {
            get { return GMonth == MonthsEnum.June ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal July
        {
            get { return GMonth == MonthsEnum.July ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal August
        {
            get { return GMonth == MonthsEnum.August ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal September
        {
            get { return GMonth == MonthsEnum.September ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal October
        {
            get { return GMonth == MonthsEnum.October ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal November
        {
            get { return GMonth == MonthsEnum.November ? QtyValue : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal December
        {
            get { return GMonth == MonthsEnum.December ? QtyValue : 0; }
        }

        [NonPersistent]
        public string GQuarter
        {
            get
            {
                switch (GMonth)
                {
                    case MonthsEnum.None:
                        break;
                    case MonthsEnum.January:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.February:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.March:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.April:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.May:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.June:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.July:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.August:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.September:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.October:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.November:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.December:
                        _Quarter = "4th QTR";
                        break;
                    default:
                        break;
                }
                return _Quarter;
            }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "d")]
        public int GYear
        {
            get
            {
                return EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string GMonthSorter
        {
            get
            {
                switch (GMonth)
                {
                    case MonthsEnum.None:
                        return "00 NONE";
                    case MonthsEnum.January:
                        return "01 JANUARY";
                    case MonthsEnum.February:
                        return "02 FEBRUARY";
                    case MonthsEnum.March:
                        return "03 MARCH";
                    case MonthsEnum.April:
                        return "04 APRIL";
                    case MonthsEnum.May:
                        return "05 MAY";
                    case MonthsEnum.June:
                        return "06 JUNE";
                    case MonthsEnum.July:
                        return "07 JULY";
                    case MonthsEnum.August:
                        return "08 AUGUST";
                    case MonthsEnum.September:
                        return "09 SEPTEMBER";
                    case MonthsEnum.October:
                        return "10 OCTOBER";
                    case MonthsEnum.November:
                        return "11 NOVEMBER";
                    case MonthsEnum.December:
                        return "12 DECEMBER";
                    default:
                        return "00 NONE";
                }
            }
        }

        #endregion

        #region Get Current User

        private SecurityUser _CurrentUser;
        
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser
        {
            get
            {
                if (SecuritySystem.CurrentUser != null)
                {
                    _CurrentUser = Session.GetObjectByKey<SecurityUser>(
                    Session.GetKeyValue(SecuritySystem.CurrentUser));
                }
                return _CurrentUser;
            }
        }

        #endregion

        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }
        #endregion

        public PurchasesMovementBuffer(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            RowID = Guid.NewGuid();
            #region Saving Creation
            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }
            #endregion
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            #region Saving Modified
            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }
            #endregion
        }
    }

}
