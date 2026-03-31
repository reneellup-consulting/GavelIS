using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class InvoiceDetailTrackingLine : ItemTrackingLine {
        private InvoiceDetail _InvoiceDetailID;
        [Association("InvoiceDetail-InvoiceDetailTrackingLines")]
        public InvoiceDetail InvoiceDetailID {
            get { return _InvoiceDetailID; }
            set {
                SetPropertyValue("InvoiceDetailID", ref _InvoiceDetailID, value)
                ;
                if (!IsLoading && _InvoiceDetailID != null) {
                    ItemNo = _InvoiceDetailID.ItemNo;
                    SourceRowID = _InvoiceDetailID.RowID;
                    Source = "Invoice Detail";
                }
            }
        }
        public InvoiceDetailTrackingLine(Session session): base(session) {
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
        }
    }
}
