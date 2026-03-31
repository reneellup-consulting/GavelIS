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
    public class TransferOrderDetailTrackingLine : ItemTrackingLine {
        private TransferOrderDetail _TransferOrderDetailID;
        [Association("TransferOrderDetail-TransferOrderDetailTrackingLines")]
        public TransferOrderDetail InvoiceDetailID {
            get { return _TransferOrderDetailID; }
            set {
                SetPropertyValue("TransferOrderDetailDetailID", ref 
                _TransferOrderDetailID, value);
                if (!IsLoading && _TransferOrderDetailID != null) {
                    ItemNo = _TransferOrderDetailID.ItemNo;
                    SourceRowID = _TransferOrderDetailID.RowID;
                    Source = "Transfer Order Detail";
                }
            }
        }
        public TransferOrderDetailTrackingLine(Session session): base(session) {
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
