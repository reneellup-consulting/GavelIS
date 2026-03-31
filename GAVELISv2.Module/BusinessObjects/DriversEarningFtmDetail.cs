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
    public class DriversEarningFtmDetail : XPObject
    {
        private DriversEarningsFtm _ParentId;
        private Employee _Employeee;
        private decimal _Adjustments;
        private decimal _Deductions;
        private decimal _TotalPayValue;
        private decimal _GrossPay;
        private decimal _NetPay;
        [Custom("AllowEdit", "False")]
        [Association("DriversEarningFtmDetails")]
        public DriversEarningsFtm ParentId
        {
            get { return _ParentId; }
            set { SetPropertyValue("ParentId", ref _ParentId, value); }
        }

        [Custom("AllowEdit", "False")]
        public Employee Employeee
        {
            get { return _Employeee; }
            set { SetPropertyValue("Employeee", ref _Employeee, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Adjustments
        {
            get { return _Adjustments; }
            set { SetPropertyValue("Adjustments", ref _Adjustments, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Deductions
        {
            get { return _Deductions; }
            set { SetPropertyValue("Deductions", ref _Deductions, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal TotalPayValue
        {
            get { return _TotalPayValue; }
            set { SetPropertyValue("TotalPayValue", ref _TotalPayValue, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal GrossPay
        {
            get { return _GrossPay; }
            set { SetPropertyValue("GrossPay", ref _GrossPay, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NetPay
        {
            get { return _NetPay; }
            set { SetPropertyValue("NetPay", ref _NetPay, value); }
        }

        public DriversEarningFtmDetail(Session session)
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
        }
    }

}
