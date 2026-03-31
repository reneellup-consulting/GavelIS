using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class PayBillSuppressConfirmation : DevExpress.ExpressApp.Win
    .SystemModule.WinDetailViewController {
        protected override void OnActivated() {
            base.OnActivated();
            if (View.Id == "PayBill_DetailView" || View.Id == 
            "CheckPayment_DetailView" || View.Id == "ReceivePayment_DetailView" 
            || View.Id == "InvoiceReconciliation_DetailView") {
                SuppressConfirmation = true;}
        }
    }
}
