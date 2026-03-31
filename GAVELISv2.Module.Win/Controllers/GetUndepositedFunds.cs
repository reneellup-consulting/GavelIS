using System;
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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class GetUndepositedFunds : ViewController {
        private PopupWindowShowAction getUndepositedFunds;
        private Deposit _Deposit;
        public GetUndepositedFunds() {
            this.TargetObjectType = typeof(Deposit);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "Deposit.GetUndepositedFunds";
            this.getUndepositedFunds = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getUndepositedFunds.Caption = "Get Undeposited Funds";
            this.getUndepositedFunds.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            GetUndepositedFunds_CustomizePopupWindowParams);
            this.getUndepositedFunds.Execute += new 
            PopupWindowShowActionExecuteEventHandler(GetUndepositedFunds_Execute
            );
        }
        private void GetUndepositedFunds_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _Deposit = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as Deposit;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(
            GenJournalHeader));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(GenJournalHeader), 
            listViewId);
            if (_Deposit.AccountToDeposit == null) {throw new 
                ApplicationException("Please specify account to deposit");}
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(
            "[SourceType.Code] = 'CR' And [Voided] = False And [OperationType.Code] = 'PR' And [CRPaymentMode] In ('Check', 'Cash', 'Others') And [CRDeposited] = False"
            );
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void GetUndepositedFunds_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            foreach (GenJournalHeader item in e.PopupWindow.View.SelectedObjects
            ) {
                GenJournalHeader gjh = _Deposit.Session.GetObjectByKey<
                GenJournalHeader>(item.Oid);
                DepositDetail depositDet = new DepositDetail(_Deposit.Session);
                depositDet.DepositID = _Deposit;
                depositDet.SourceType = gjh.SourceType;
                depositDet.SourceNo = gjh.SourceNo;
                depositDet.SourceID = gjh.Oid;
                depositDet.Date = gjh.EntryDate;
                depositDet.CheckNo = gjh.CRCheckNo;
                depositDet.ReferenceNo = gjh.CRReferenceNo;
                depositDet.Name = gjh.CRName;
                depositDet.Mode = gjh.CRPaymentMode;
                depositDet.Memo = gjh.CRMemo;
                depositDet.Amount = gjh.CRDeposit;
                depositDet.Save();
            }
        }
    }
}
