using System;
using System.Activities;
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
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.Xpo;
namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class OfrsBuffer : XPObject
    {
        private DateTime _RequestDate;
        private string _FrsNo;
        private string _TripDocNo;
        private string _ReleasedBy;
        private DateTime _ReleasedDate;
        private string _CheckedBy;
        private DateTime _CheckedDate;
        private string _ApprovedBy;
        private DateTime _ApprovedDate;
        private string _Purpose;
        private string _Remarks;
        private string _UnitNo;
        private string _UnitType;
        private string _Operator;
        private decimal _Odometer;
        private string _Tariff;
        private string _FuelStation;
        private string _ItemNo;
        private decimal _Cost;
        private decimal _Quantity;
        private bool _IsPoCreated;
        private PurchaseOrder _PoCreatedRef;
        // RequestDate
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime RequestDate
        {
            get { return _RequestDate; }
            set { SetPropertyValue("RequestDate", ref _RequestDate, value); }
        }
        // FrsNo
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string FrsNo
        {
            get { return _FrsNo; }
            set { SetPropertyValue("FrsNo", ref _FrsNo, value); }
        }
        // TripDocNo
        [RuleRequiredField("", DefaultContexts.Save)]
        public string TripDocNo
        {
            get { return _TripDocNo; }
            set { SetPropertyValue("TripDocNo", ref _TripDocNo, value); }
        }
        // ReleasedBy
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ReleasedBy
        {
            get { return _ReleasedBy; }
            set { SetPropertyValue("ReleasedBy", ref _ReleasedBy, value); }
        }
        // ReleasedDate
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ReleasedDate
        {
            get { return _ReleasedDate; }
            set { SetPropertyValue("ReleasedDate", ref _ReleasedDate, value); }
        }
        // CheckedBy
        [RuleRequiredField("", DefaultContexts.Save)]
        public string CheckedBy
        {
            get { return _CheckedBy; }
            set { SetPropertyValue("CheckedBy", ref _CheckedBy, value); }
        }
        // CheckedDate
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime CheckedDate
        {
            get { return _CheckedDate; }
            set { SetPropertyValue("CheckedDate", ref _CheckedDate, value); }
        }
        // ApprovedBy
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ApprovedBy
        {
            get { return _ApprovedBy; }
            set { SetPropertyValue("ApprovedBy", ref _ApprovedBy, value); }
        }
        // ApprovedDate
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ApprovedDate
        {
            get { return _ApprovedDate; }
            set { SetPropertyValue("ApprovedDate", ref _ApprovedDate, value); }
        }
        // Purpose
        [Size(SizeAttribute.Unlimited)]
        public string Purpose
        {
            get { return _Purpose; }
            set { SetPropertyValue("Purpose", ref _Purpose, value); }
        }
        // Remarks
        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }
        // UnitNo
        [RuleRequiredField("", DefaultContexts.Save)]
        public string UnitNo
        {
            get { return _UnitNo; }
            set { SetPropertyValue("UnitNo", ref _UnitNo, value); }
        }
        // UnitType
        [RuleRequiredField("", DefaultContexts.Save)]
        public string UnitType
        {
            get { return _UnitType; }
            set { SetPropertyValue("UnitType", ref _UnitType, value); }
        }
        // Operator
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Operator
        {
            get { return _Operator; }
            set { SetPropertyValue("Operator", ref _Operator, value); }
        }
        // Odometer
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Odometer
        {
            get { return _Odometer; }
            set { SetPropertyValue("Odometer", ref _Odometer, value); }
        }
        // Tariff
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Tariff
        {
            get { return _Tariff; }
            set { SetPropertyValue("Tariff", ref _Tariff, value); }
        }
        // FuelStation
        [RuleRequiredField("", DefaultContexts.Save)]
        public string FuelStation
        {
            get { return _FuelStation; }
            set { SetPropertyValue("FuelStation", ref _FuelStation, value); }
        }
        // ItemNo
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ItemNo
        {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }
        // Cost
        [Custom("DisplayFormat", "n")]
        public decimal Cost
        {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        // Quantity
        [Custom("DisplayFormat", "n")]
        public decimal Quantity
        {
            get { return _Quantity; }
            set { SetPropertyValue("Quantity", ref _Quantity, value); }
        }
        // Total
        [PersistentAlias("Cost * Quantity")]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }
        // IsPoCreated
        [Custom("AllowEdit", "False")]
        public bool IsPoCreated
        {
            get { return _IsPoCreated; }
            set { SetPropertyValue("IsPoCreated", ref _IsPoCreated, value); }
        }
        // PoCreatedRef
        [Custom("AllowEdit", "False")]
        public PurchaseOrder PoCreatedRef
        {
            get { return _PoCreatedRef; }
            set { SetPropertyValue("PoCreatedRef", ref _PoCreatedRef, value); }
        }
        public OfrsBuffer(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        //public void ActivateWorkflowDefinition() {
        //    IWorkflowDefinition definition = Session.FindObject<XpoWorkflowDefinition>(CriteriaOperator.Parse("Name = 'My workflow'"));
        //    if (definition != null)
        //    {
        //        XpoStartWorkflowRequest request = new XpoStartWorkflowRequest(Session);
        //        request.TargetObjectKey = Oid;
        //        request.TargetWorkflowUniqueId = definition.GetUniqueId();
        //    }
        //}
    }

}
