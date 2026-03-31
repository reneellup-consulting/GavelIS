using System;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using System.Collections.Generic;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class EmployeeChargeSlip : GenJournalHeader {
        private Employee _Employee;
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private TypeOfEmployeeChargesEnum _TypeOfCharge;
        private OtherDeduction _DeductionCode;
        private Account _AdvancesAccount;
        private EmployeeChargeSlipStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private EmpOtherDed _PayrollDeductionRef;
        private bool _Released = false;

        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue<Employee>("Employee", ref _Employee, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [Size(1000)]
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue<string>("ReferenceNo", ref _ReferenceNo, value); }
        }

        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue<string>("Memo", ref _Memo, value); }
        }

        [Size(500)]
        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue<string>("Comments", ref _Comments, value); }
        }
        [ImmediatePostData]
        public TypeOfEmployeeChargesEnum TypeOfCharge {
            get { return _TypeOfCharge; }
            set { SetPropertyValue<TypeOfEmployeeChargesEnum>("TypeOfCharge", ref _TypeOfCharge, value);
            if (!IsLoading)
            {

                switch (_TypeOfCharge)
                {
                    case TypeOfEmployeeChargesEnum.ToolsRequest:
                        // Tools
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Tools")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.ItemRequest:
                        // Others
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Others")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.PartsDamage:
                        // Damages
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Damages")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.TireDamage:
                        // Damages
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Damages")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.FlapDamage:
                        // Damages
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Damages")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.TubesDamage:
                        // Damages
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Damages")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.RimDamage:
                        // Damages
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Damages")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.PartLost:
                        // Others
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Others")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.TireLost:
                        // Others
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Others")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.BatteryLost:
                        // Others
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Others")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.BatteryDamage:
                        // Damages
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Damages")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.CargoDamage:
                        // Damages
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Damages")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.CargoLoss:
                        // Others
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Others")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.VulcateCharge:
                        // Cash Advance
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Cash Advance")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.FuelChargeTractor:
                        // Hi-Gas Tractor
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Hi-Gas Tractor")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.AccidentCharge:
                        // Damages
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Damages")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.Violation:
                        // Cash Advance
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Cash Advance")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.HealthAndServices:
                        // Others
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Others")) ?? null;
                        break;
                    case TypeOfEmployeeChargesEnum.FuelChargeGenset:
                        // Hi-Gas Genset
                            DeductionCode = Session.FindObject<OtherDeduction>(new BinaryOperator("Description", "Hi-Gas Genset")) ?? null;
                        break;
                    default:
                        break;
                }
            }
            }
        }
        private List<string> _Refs;
        [Custom("AllowEdit", "False")]
        public List<string> Refs
        {
            get { return _Refs; }
            set { SetPropertyValue<List<string>>("Refs", ref _Refs, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public OtherDeduction DeductionCode {
            get { return _DeductionCode; }
            set
            {
                SetPropertyValue<OtherDeduction>("DeductionCode", ref _DeductionCode, value);
                if (!IsLoading)
                {
                    AdvancesAccount = _DeductionCode.OtherDeductionAccount ?? null;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account AdvancesAccount {
            get { return _AdvancesAccount; }
            set { SetPropertyValue<Account>("AdvancesAccount", ref _AdvancesAccount, value); }
        }
        [Action(AutoCommit = true, Caption = "Approve All Selected")]
        public void ApproveAllSelected()
        {
            var admin = CurrentUser.Roles.Where(o => o.Name == "Administrator").FirstOrDefault();
            if (admin != null)
            {
                if (!string.IsNullOrEmpty(this.Memo))
                {
                    this.Status = EmployeeChargeSlipStatusEnum.Approved;
                }
            }
            else
            {
                throw new UserFriendlyException("Non admin cannot use this action.");
            }
        }
        public EmployeeChargeSlipStatusEnum Status {
            get { return _Status; }
            set { SetPropertyValue<EmployeeChargeSlipStatusEnum>("Status", ref _Status, value);
            if (!IsLoading)
            {
                if (_Status != EmployeeChargeSlipStatusEnum.Current) { Approved = true; }
                else
                {
                    Approved = false;
                }
            }
            if (!IsLoading && SecuritySystem.CurrentUser != null)
            {
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
            set { SetPropertyValue<string>("StatusBy", ref _StatusBy, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue<DateTime>("StatusDate", ref _StatusDate, value); }
        }
        [Custom("AllowEdit", "False")]
        public EmpOtherDed PayrollDeductionRef {
            get { return _PayrollDeductionRef; }
            set { SetPropertyValue<EmpOtherDed>("PayrollDeductionRef", ref _PayrollDeductionRef, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool Released
        {
            get { return _Released; }
            set { SetPropertyValue<bool>("Released", ref _Released, value); }
        }

        #region Calculated Details

        [Persistent("NetOfItems")]
        private decimal? _NetOfItems;
        [Persistent("TotalTax")]
        private decimal? _TotalTax;
        [Persistent("TotalChargeOfExpense")]
        private decimal? _TotalChargeOfExpense;
        [PersistentAlias("_NetOfItems")]
        [Custom("DisplayFormat", "n")]
        public decimal? NetOfItems {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _NetOfItems == null)
                    {
                        UpdateNetOfItems(false);
                    }
                } catch (Exception)
                {
                }
                return _NetOfItems;
            }
        }

        public void UpdateNetOfItems(bool forceChangeEvent) {
            decimal? oldTotal = _NetOfItems;
            decimal tempTotal = 0m;
            foreach (EmployeeChargeSlipItemDetail detail in EmployeeChargeSlipItemDetails)
            {
                tempTotal += detail.Total;
            }
            _NetOfItems = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("NetOfItems", NetOfItems,
                _NetOfItems);
            }
            ;
        }

        [PersistentAlias("_TotalTax")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalTax {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalTax == null)
                    {
                        UpdateTotalTax(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalTax;
            }
        }

        public void UpdateTotalTax(bool forceChangeEvent) {
            decimal? oldTotal = _TotalTax;
            decimal tempTotal = 0m;
            foreach (EmployeeChargeSlipItemDetail detail in EmployeeChargeSlipItemDetails)
            {
                if (detail.Tax !=
                null && detail.Tax.Taxable)
                {
                    tempTotal += detail.Total - (detail
                    .Total / (1 + (detail.Tax.Rate / 100)));
                }
            }
            _TotalTax = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalTax", TotalTax,
                _TotalTax);
            }
            ;
        }

        [PersistentAlias("_TotalChargeOfExpense")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalChargeOfExpense {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalChargeOfExpense == null)
                    {
                        UpdateTotalChargeOfExpense(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalChargeOfExpense;
            }
        }

        public void UpdateTotalChargeOfExpense(bool forceChangeEvent) {
            decimal? oldTotal = _TotalChargeOfExpense;
            decimal tempTotal = 0m;
            foreach (EmployeeChargeSlipExpenseDetail detail in EmployeeChargeSlipExpenseDetails)
            {
                tempTotal += detail.Amount;
            }
            _TotalChargeOfExpense = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalChargeOfExpense", TotalChargeOfExpense,
                _TotalChargeOfExpense);
            }
            ;
        }

        [PersistentAlias("NetOfItems + TotalChargeOfExpense")]
        [Custom("DisplayFormat", "n")]
        public decimal GrossTotal {
            get
            {
                object tempObject = null;
                try
                {
                    tempObject = EvaluateAlias("GrossTotal");
                } catch (Exception)
                {
                }
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        #endregion

        public EmployeeChargeSlip(Session session)
            : base(session) {
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator("Code", "ECS")
            );
            OperationType = Session.FindObject<OperationType>(new BinaryOperator("Code",
            "ECS"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new BinaryOperator("Code"
            , "ECS"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ?
                source.GetNewNo() :
                null;
                source.Save();
                session.CommitChanges();
            }
        }
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            //this.IsIncExpNeedUpdate = true;
            base.TriggerObjectChanged(args);
        }
        protected override void OnSaving()
        {
            //this.AutoRegisterIncomeExpenseVer();
            base.OnSaving();
        }
        protected override void OnSaved()
        {
            this.AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            base.OnSaved();
        }
        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Employee Charge Slip transactions."
                );
            }
        }

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            //_Total = null;
            _NetOfItems = null;
            _TotalTax = null;
            _TotalChargeOfExpense = null;
        }
    }

}
