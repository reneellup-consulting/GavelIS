using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class PoSyncDeletionsQueue : XPObject
    {
        private string _POType;
        private string _RowType;
        private int _RowId;
        // POType (General, Fuel)
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string POType
        {
            get { return _POType; }
            set { SetPropertyValue("POType", ref _POType, value); }
        }
        // RowType (Parent, Line)
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string RowType
        {
            get { return _RowType; }
            set { SetPropertyValue("RowType", ref _RowType, value); }
        }
        // RowId
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public int RowId
        {
            get { return _RowId; }
            set { SetPropertyValue("RowId", ref _RowId, value); }
        }
        public PoSyncDeletionsQueue(Session session)
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
