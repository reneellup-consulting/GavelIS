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
using DevExpress.ExpressApp.Reports;
using System.IO;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class Requisition : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private RequisitionStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Employee _RequestedBy;
        private DateTime _ExpectedDate;
        private CostCenter _CostCenter;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private Facility _Facility;
        private Department _Department;
        private Employee _FacilityHead;
        private Employee _DepartmentInCharge;
        private Guid _RowID;
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        [Size(500)]
        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }
        public RequisitionStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    switch (_Status) {
                        case RequisitionStatusEnum.Current:
                            Approved = false;
                            break;
                        case RequisitionStatusEnum.Approved:
                            //var cuser = CurrentUser.Roles.Where(o => o.Name == "REQApprover").FirstOrDefault();
                            //if (cuser == null)
                            //{
                            //    throw new UserFriendlyException("You are not authorized to approve request");
                            //}
                            Approved = true;
                            break;
                        case RequisitionStatusEnum.Cancelled:
                            foreach (var item in RequisitionWorksheetLines)
                            {
                                if (item.Status != RequisitionWSStateEnum.Open)
                                {
                                    throw new UserFriendlyException("Requisition can no longer be cancelled because there are active request already");
                                }
                                item.Cancelled = true;
                            }
                            Approved = false;
                            break;
                        case RequisitionStatusEnum.Served:
                            Approved = true;
                            break;
                        default:
                            break;
                    }
                    //if (_Status != RequisitionStatusEnum.Current) {Approved = 
                    //    true;} else {
                    //    Approved = false;
                    //}
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null) {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee RequestedBy {
            get { return _RequestedBy; }
            set { SetPropertyValue("RequestedBy", ref _RequestedBy, value); }
        }
        public DateTime ExpectedDate {
            get { return _ExpectedDate; }
            set { SetPropertyValue("ExpectedDate", ref _ExpectedDate, value); }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue("CostCenter", ref _CostCenter, value);
            if (!IsLoading && !IsSaving && _CostCenter!=null)
            {
                Facility = _CostCenter.Facility ?? null;
                Department = _CostCenter.Department ?? null;
            }
            }
        }
        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set
            {
                SetPropertyValue<ExpenseType>("ExpenseType", ref _ExpenseType, value);
                if (!IsLoading)
                {
                    SubExpenseType = null;
                }
            }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType
        {
            get { return _SubExpenseType; }
            set { SetPropertyValue<SubExpenseType>("SubExpenseType", ref _SubExpenseType, value); }
        }
        public Facility Facility
        {
            get { return _Facility; }
            set { SetPropertyValue("Facility", ref _Facility, value);
            if (!IsLoading)
            {
                FacilityHead = null;
                DepartmentInCharge = null;
            }
            }
        }
        [DataSourceProperty("Facility.Departments")]
        public Department Department
        {
            get { return _Department; }
            set { SetPropertyValue("Department", ref _Department, value);
            if (!IsLoading && !IsSaving && _Department != null)
            {
                FacilityHead = _Department.DepartmentHead ?? null;
                DepartmentInCharge = _Department.InCharge ?? null;
            }
            }
        }
        [Custom("AllowEdit", "False")]
        public Employee FacilityHead
        {
            get { return _FacilityHead; }
            set { SetPropertyValue("FacilityHead", ref _FacilityHead, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee DepartmentInCharge
        {
            get { return _DepartmentInCharge; }
            set { SetPropertyValue("DepartmentInCharge", ref _DepartmentInCharge, value); }
        }
        
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        private bool _Printed;
        [Custom("AllowEdit", "False")]
        public bool Printed
        {
            get { return _Printed; }
            set { SetPropertyValue<bool>("Printed", ref _Printed, value); }
        }
        [Action(Caption = "Unmarked Printed", AutoCommit = true)]
        public void UnmarkPrinted() {
            Printed = false;
        }
        [Action(Caption = "Print PRS")]
        public void PrintVoucher()
        {
            if (Status != RequisitionStatusEnum.Approved)
            {
                throw new ApplicationException("Must be in approved state");
            }
            if (_Printed)
            {
                throw new ApplicationException("Requisition already printed");
            }
            //Printed = true;
            //this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\PartsRequisitionSlipDot.repx";
            IObjectSpace objs = ObjectSpace.FindObjectSpaceByObject(Session);
            rep.LoadLayout(path);
            rep.ObjectSpace = objs;
            XPCollection<Requisition> xpc = new XPCollection<Requisition>(Session)
            {
                LoadingEnabled = true
            };
            xpc.Filter = new InOperator(rep.ObjectSpace.GetKeyPropertyName(typeof(Requisition)), new string[] { this.Oid.ToString()
            });
            //xpc.Add(this);
            //System.Collections.Generic.IList<MultiCheckVoucher> rds = rep.ObjectSpace.GetObjects<MultiCheckVoucher>(CriteriaOperator.Parse(string.Format("[Oid] = '{0}'", this.Oid)));
            //XPDataView xpdbv = new XPDataView();
            //string sql = string.Format("select Oid, EntryDate,Payee from MultiCheckVoucher where Oid = '{0}'", this.Oid);
            //DevExpress.Xpo.DB.SelectedData selectedData = this.Session.ExecuteQuery(sql);
            //xpdbv.AddProperty("Oid", typeof(Guid));
            //xpdbv.AddProperty("EntryDate", typeof(DateTime));
            //xpdbv.AddProperty("Payee", typeof(Contact));
            //xpdbv.LoadData(selectedData);
            rep.DataSource = xpc;
            rep.PrintingSystem.StartPrint += new DevExpress.XtraPrinting.PrintDocumentEventHandler(PrintingSystem_StartPrint);
            rep.Print();
            objs.CommitChanges();
        }

        void PrintingSystem_StartPrint(object sender, DevExpress.XtraPrinting.PrintDocumentEventArgs e)
        {
            e.PrintDocument.EndPrint += new System.Drawing.Printing.PrintEventHandler(PrintDocument_EndPrint);
        }

        void PrintDocument_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            IObjectSpace objs = ObjectSpace.FindObjectSpaceByObject(Session);
            Printed = true;
            objs.CommitChanges();
            //this.Session.CommitTransaction();
        }

        public Requisition(Session session): base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            RowID = Guid.NewGuid();
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "RQ"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "RQ"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "RQ"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
            Memo = "Requisition #" + SourceNo;
        }
        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved Requistion transactions."
                );} }
    }
}
