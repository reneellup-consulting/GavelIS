using System;
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
    [NavigationItem(false)]
    public class FuelStatementAdjustment : XPObject
    {
        private Guid _RowID;
        private FuelStatementOfAccount _OwnerId;
        private decimal _Amount;
        private BusinessObjects.Account _Account;
        private string _Description;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("FuelStatementOfAccount-FuelStatementAdjustments")]
        public FuelStatementOfAccount OwnerId
        {
            get { return _OwnerId; }
            set
            {
                FuelStatementOfAccount oldOwnerId = _OwnerId;
                SetPropertyValue("OwnerId", ref _OwnerId, value);
                if (!IsLoading && !IsSaving && oldOwnerId != null)
                {
                    #region Aggregated Calculation
                    oldOwnerId.UpdateAdjustments(true);
                    #endregion
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account Account
        {
            get { return _Account; }
            set
            {
                SetPropertyValue("Account", ref _Account, value);
                if (!IsLoading && !IsSaving)
                {
                    Description = _Account.Name;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
                if (!IsLoading && !IsSaving && _OwnerId != null)
                {
                    _OwnerId.UpdateAdjustments(true);
                }
            }
        }
        public FuelStatementAdjustment(Session session)
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
