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
    //[NonPersistent]
    public class PdReqReportItemSelection : XPObject
    {
        private PeriodicRequisitionReport _PerReqId;
        private ItemType _TypeOfItem;
        [Association("PdReqReportItemSelections")]
        public PeriodicRequisitionReport PerReqId
        {
            get { return _PerReqId; }
            set { SetPropertyValue("PerReqId", ref _PerReqId, value); }
        }
        public ItemType TypeOfItem
        {
            get { return _TypeOfItem; }
            set { SetPropertyValue("TypeOfItem", ref _TypeOfItem, value); }
        }
        public PdReqReportItemSelection(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
    }

}
