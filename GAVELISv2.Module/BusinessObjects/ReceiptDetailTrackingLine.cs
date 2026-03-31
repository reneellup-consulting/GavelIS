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
    public class ReceiptDetailTrackingLine : ItemTrackingLine {
        private ReceiptDetail _ReceiptDetailID;
        [Association("ReceiptDetail-ReceiptDetailTrackingLines")]
        public ReceiptDetail ReceiptDetailID {
            get { return _ReceiptDetailID; }
            set {
                SetPropertyValue("ReceiptDetailID", ref _ReceiptDetailID, value)
                ;
                if (!IsLoading && _ReceiptDetailID != null) {
                    ItemNo = _ReceiptDetailID.ItemNo;
                    SourceRowID = _ReceiptDetailID.RowID;
                    Source = "Receipt Detail";
                }
            }
        }
        public ReceiptDetailTrackingLine(Session session): base(session) {
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
