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
    public class DebitMemoDetailTrackingLine : ItemTrackingLine {
        private DebitMemoDetail _DebitMemoDetailID;
        [Association("DebitMemoDetail-DebitMemoDetailTrackingLines")]
        public DebitMemoDetail DebitMemoDetailID {
            get { return _DebitMemoDetailID; }
            set {
                SetPropertyValue("DebitMemoDetailID", ref _DebitMemoDetailID, 
                value);
                if (!IsLoading && _DebitMemoDetailID != null) {
                    ItemNo = _DebitMemoDetailID.ItemNo;
                    SourceRowID = _DebitMemoDetailID.RowID;
                    Source = "Debit Memo Detail";
                }
            }
        }
        public DebitMemoDetailTrackingLine(Session session): base(session) {
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
