using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers {
    public partial class TenderCashPaymentController : ViewController {
        private IObjectSpace _ObjectSpace;
        private Invoice _Invoice;
        private PaymentsApplied _Obj;
        private PopupWindowShowAction tenderCashPayment;

        public TenderCashPaymentController()
        {
            this.TargetObjectType = typeof(Invoice);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("Tender Payment");
            this.tenderCashPayment = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.tenderCashPayment.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            TenderCashPayment_CustomizePopupWindowParams);
            this.tenderCashPayment.Execute += new
            PopupWindowShowActionExecuteEventHandler(TenderCashPayment_Execute);
        }

        private void TenderCashPayment_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Invoice = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
             as Invoice;
            Invoice inv = ObjectSpace.GetObjectByKey<Invoice>(_Invoice.Oid);
            inv.RequireTenderPayment = false;
            inv.Save();
            ObjectSpace.CommitChanges();
            Invoice thisInvoice = _ObjectSpace.GetObjectByKey<Invoice>(
            _Invoice.Oid);
            _Obj = _ObjectSpace.CreateObject<PaymentsApplied>();
            _Obj.GenJournalID = thisInvoice;
            _Obj.PayeeName = thisInvoice.Customer.Name;
            _Obj.Amount = thisInvoice.GrossTotal.Value;
            _Obj.CheckDate = thisInvoice.EntryDate;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "PaymentsApplied_DetailView", true, _Obj);
        }

        private void TenderCashPayment_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e) {
            if (_Obj.PaymentTenderedType == PaymentTenderedTypeEnum.Memo && _Obj.Memo != null)
            {
                if (_Obj.Memo.OpenAmount == 0m)
                {
                    if (_Obj.Memo.GrossTotal.Value < _Obj.Amount)
                    {
                        throw new ApplicationException("Tendered amount cannot exceed memo open amount");
                    }
                }
                else if (_Obj.Memo.OpenAmount > 0m)
                {
                    if (_Obj.Memo.OpenAmount < _Obj.Amount)
                    {
                        throw new ApplicationException("Tendered amount cannot exceed memo open amount");
                    }
                }
            }
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_Invoice);
            ObjectSpace.Refresh();
        }
    }
}
