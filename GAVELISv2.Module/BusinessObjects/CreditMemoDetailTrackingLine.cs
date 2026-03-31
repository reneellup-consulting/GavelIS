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
    public class CreditMemoDetailTrackingLine : ItemTrackingLine {
        private CreditMemoDetail _CreditMemoDetailID;
        [Association("CreditMemoDetail-CreditMemoDetailTrackingLines")]
        public CreditMemoDetail CreditMemoDetailID {
            get { return _CreditMemoDetailID; }
            set {
                SetPropertyValue("CreditMemoDetailID", ref _CreditMemoDetailID, 
                value);
                if (!IsLoading && _CreditMemoDetailID != null) {
                    ItemNo = _CreditMemoDetailID.ItemNo;
                    SourceRowID = _CreditMemoDetailID.RowID;
                }
            }
        }
        public CreditMemoDetailTrackingLine(Session session): base(session) {
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
