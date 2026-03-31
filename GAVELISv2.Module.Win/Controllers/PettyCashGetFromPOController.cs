using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using System.Threading.Tasks;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class PettyCashGetFromPOController : ViewController
    {
        private PopupWindowShowAction PettyCashGetFromPOAction;
        private CheckVoucher _CheckVoucher;
        public PettyCashGetFromPOController()
        {
            this.TargetObjectType = typeof(CheckVoucher);
            this.TargetViewId = "CheckVoucher_DetailView";
            string actionID = "PettyCash.PettyCashGetFromPO";
            this.PettyCashGetFromPOAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.PettyCashGetFromPOAction.Caption = "Get from PO";
            this.PettyCashGetFromPOAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(PettyCashGetFromPOAction_CustomizePopupWindowParams);
            this.PettyCashGetFromPOAction.Execute += new PopupWindowShowActionExecuteEventHandler(PettyCashGetFromPOAction_Execute);
        }

        void PettyCashGetFromPOAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var selectedObjects = e.PopupWindow.View.SelectedObjects;
            int index = 0;
            foreach (PurchaseOrderDetail item in selectedObjects)
            {
                PurchaseOrderDetail detail = _CheckVoucher.Session.GetObjectByKey<PurchaseOrderDetail>(item.Oid);
                if (_CheckVoucher.ReceiptDetails.Count > 0)
                {
                    var exist = _CheckVoucher.GenJournalDetails.Where(o => o.PODetailID == detail);
                    if (exist.Count() > 0)
                    {
                        throw new ApplicationException(string.Format("Purchase Order Detail ID # {0} already exist", detail.Oid));
                    }
                }
                if (detail.RemainingQty > 0)
                {
                    GenJournalDetail gdet = new GenJournalDetail(_CheckVoucher.Session);
                    gdet.GenJournalID = _CheckVoucher;
                    gdet.PODetailID = detail;
                    gdet.Description = string.Format("#{0} #{1} {2}", detail.PurchaseInfo.SourceNo, detail.ItemNo.No, detail.Description);
                    gdet.DebitAmount = detail.Total;
                    gdet.SubAccountNo = detail.PurchaseInfo.Vendor;
                    gdet.SubAccountType = gdet.SubAccountNo.ContactType;
                    gdet.Account = detail.PurchaseInfo.Vendor.Account ?? null;
                    gdet.CVLineDate = detail.PurchaseInfo.EntryDate;
                    gdet.CostCenter = detail.CostCenter != null ? detail.CostCenter : null;
                    if (detail.RequisitionNo != null && detail.RequestID != Guid.Empty)
                    {
                        RequisitionWorksheet rws = _CheckVoucher.Session.FindObject<RequisitionWorksheet>(BinaryOperator.Parse("[RowID]=?", detail.RequestID));
                        if (rws != null)
                        {
                            gdet.ExpenseType = rws.ExpenseType ?? null;
                            gdet.SubExpenseType = rws.SubExpenseType ?? null;
                        }
                    }
                    gdet.Save();
                    detail.PettyCashID = gdet;
                    detail.Save();
                }
            }
        }

        void PettyCashGetFromPOAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _CheckVoucher = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as CheckVoucher;
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "PurchaseOrderDetail_ListView_ToReceive";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(PurchaseOrderDetail),
            listViewId);
            // [PurchaseInfo.Status] In ('Approved', 'PartiallyReceived') 
            // And [LineApprovalStatus] = 'Released' And [RemainingQty] <> 0.0m
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse("[PurchaseInfo.Status] In ('Approved', 'PartiallyReceived')And [LineApprovalStatus] = 'Released' And [RemainingQty] <> 0.0m");
            
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}
