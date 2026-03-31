using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    [OptimisticLocking(false)]
    public class PartsUsageIcjDetails : XPObject
    {
        private Guid _RowID;
        private DateTime _EntryDate;
        private PartsPurchasesUsageReporter _ReporterID;
        private PartsPurchasesUsageDetail _DetailID;
        private string _LineNo;
        private Item _ItemNo;
        private string _SeqNo;
        private decimal _Qty;
        private GenJournalHeader _Source;
        private Requisition _RequisitionNo;
        private decimal _CostPrice;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public PartsPurchasesUsageReporter ReporterID
        {
            get { return _ReporterID; }
            set { SetPropertyValue("ReporterID", ref _ReporterID, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("PartsUsageIcjDetails")]
        public PartsPurchasesUsageDetail DetailID
        {
            get { return _DetailID; }
            set { SetPropertyValue("DetailID", ref _DetailID, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string LineNo
        {
            get { return _LineNo; }
            set { SetPropertyValue("LineNo", ref _LineNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Item ItemNo
        {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string SeqNo
        {
            get { return _SeqNo; }
            set { SetPropertyValue("SeqNo", ref _SeqNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public decimal Qty
        {
            get { return _Qty; }
            set { SetPropertyValue("Qty", ref _Qty, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public GenJournalHeader Source
        {
            get { return _Source; }
            set { SetPropertyValue("Source", ref _Source, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Requisition RequisitionNo
        {
            get { return _RequisitionNo; }
            set { SetPropertyValue("RequisitionNo", ref _RequisitionNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Cost/Price")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public decimal CostPrice
        {
            get { return _CostPrice; }
            set { SetPropertyValue("CostPrice", ref _CostPrice, value); }
        }
        [PersistentAlias("Qty * CostPrice")]
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

        public PartsUsageIcjDetails(Session session)
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
