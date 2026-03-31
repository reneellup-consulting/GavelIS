using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.DC;
namespace GAVELISv2.Module.BusinessObjects {
    //[DefaultClassOptions]
    [DomainComponent]
    //[DeferredDeletion(false)]
    //[CreatableItem(false)]
    public class CarryOutRequest {
        [Custom("AllowEdit", "False")]
        public RequisitionActionsEnum Action { get; set; }
        public PurchaseOrder POrders { get; set; }
        public TransferOrder TOrders { get; set; }
        public SalesOrder SOrders { get; set; }
        public WorkOrder WOrders { get; set; }
        public Warehouse FromWarehouse { get; set; }
        public EmployeeChargeSlip ECSlip { get; set; }
        public PurchaseOrderFuel FOrders { get; set; }
        //public Vendor Vendor { get; set; }
        //private RequisitionActionsEnum _Action;
        //[NonPersistent]
        //public RequisitionActionsEnum Action {
        //    get { return _Action; }
        //    set { SetPropertyValue("Action", ref _Action, value); }
        //}
        //public CarryOutRequest(Session session): base(session) {
        //    // This constructor is used when an object is loaded from a persistent storage.
        //    // Do not place any code here or place it only when the IsLoading property is false:
        //    // if (!IsLoading){
        //    //    It is now OK to place your initialization code here.
        //    // }
        //    // or as an alternative, move your initialization code into the AfterConstruction method.
        //}
        //public override void AfterConstruction() {
        //    base.AfterConstruction();
        //    // Place here your initialization code.
        //}
    }
}
