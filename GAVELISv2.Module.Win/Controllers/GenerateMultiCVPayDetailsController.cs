using System;
using System.ComponentModel;
using System.Collections;
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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateMultiCVPayDetailsController : ViewController
    {
        private MultiCheckVoucher _MultiCV;
        public GenerateMultiCVPayDetailsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void GenerateMultiCVPayDetailsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _MultiCV = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as MultiCheckVoucher;
            try
            {
                for (int i = _MultiCV.MultiCheckVouchPayDetails.Count - 1; i >= 0; i--)
                {
                    _MultiCV.MultiCheckVouchPayDetails[i].Delete();
                }
            }
            catch (Exception) { }
            // Memo ***********         xxx.xx <- TotalAmount
            MultiCheckVouchPayDetail mcpd = ReflectionHelper.CreateObject<MultiCheckVouchPayDetail>(_MultiCV.Session);
            mcpd.MCheckVoucherID = _MultiCV;
            mcpd.Particulars = _MultiCV.Memo;
            mcpd.Amount = _MultiCV.TotalAmount.Value;
            mcpd.Save();
            foreach (MCheckEffectiveDetails item in _MultiCV.MCheckEffectiveDetails)
            {
                if (item.Amount == _MultiCV.TotalCheckAmt)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(item.Account.MultiCVPayDetCaption))
                {
                    throw new ApplicationException("A payment detail caption in one of the effective expense account has not been set");
                }
                MultiCheckVouchPayDetail mcpd1 = ReflectionHelper.CreateObject<MultiCheckVouchPayDetail>(_MultiCV.Session);
                mcpd1.MCheckVoucherID = _MultiCV;
                mcpd1.Particulars = item.Account.MultiCVPayDetCaption;
                if (item.Account!=null && item.Account.LessInMultiCV)
                {
                    // If (Account.IsLessInMCV != false)
                    // Account.MCVCaption       xxx.xx
                    mcpd1.Amount = 0 - item.Amount;
                }
                else
                {
                    // Else
                    // Account.MCVCaption      (xxx.xx)
                    mcpd1.Amount = item.Amount;
                }
                mcpd1.Save();
            }
            _MultiCV.Save();
            //_MultiCV.Session.CommitTransaction();
            ObjectSpace.CommitChanges();
        }
    }
}
