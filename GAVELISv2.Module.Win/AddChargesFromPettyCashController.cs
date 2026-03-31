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

namespace GAVELISv2.Module.Win
{
    public partial class AddChargesFromPettyCashController : ViewController
    {
        private PopupWindowShowAction addChargesFromPettyCash;
        private EmployeeChargeSlip _empChargeSlip;

        public AddChargesFromPettyCashController()
        {
            this.TargetObjectType = typeof(EmployeeChargeSlip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "AddChargesFromPettyCash";
            this.addChargesFromPettyCash = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.addChargesFromPettyCash.Caption = "Add Charges From PettyCash";
            this.addChargesFromPettyCash.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            AddChargesFromPettyCash_CustomizePopupWindowParams);
            this.addChargesFromPettyCash.Execute += new
            PopupWindowShowActionExecuteEventHandler(AddChargesFromPettyCash_Execute
            );
        }
        private void AddChargesFromPettyCash_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e) {
            _empChargeSlip = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as EmployeeChargeSlip;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "GenJournalHeader_GenJournalDetails_ListView_ECS";//Application.FindListViewId(typeof(GenJournalDetail));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(GenJournalDetail), listViewId)
            ;
            if (_empChargeSlip.Employee == null)
            {
                throw new
                    ApplicationException("Employee not specified");
            }

            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(string.Format("[SubAccountNo.No] = '{0}' And [CVLineDate] is not null", _empChargeSlip.Employee.No));
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void AddChargesFromPettyCash_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (GenJournalDetail item in e.PopupWindow.View.SelectedObjects){
                GenJournalDetail gjd = _empChargeSlip.Session.GetObjectByKey<GenJournalDetail>(
                    item.Oid);
                EmployeeChargeSlipExpenseDetail ecsd = ReflectionHelper.CreateObject<EmployeeChargeSlipExpenseDetail>(_empChargeSlip.Session);
                ecsd.GenJournalID = _empChargeSlip;
                ecsd.Expense = gjd.Account;
                ecsd.Description = gjd.Description;
                ecsd.IsPettyCash = true;
                ecsd.PettyCashRef = gjd;
                ecsd.CostCenter = gjd.CostCenter;
                ecsd.ExpenseType = gjd.ExpenseType;
                ecsd.SubExpenseType = gjd.SubExpenseType;
                ecsd.Amount = gjd.DebitAmount;
                ecsd.Save();
            }
        }
    }
}
