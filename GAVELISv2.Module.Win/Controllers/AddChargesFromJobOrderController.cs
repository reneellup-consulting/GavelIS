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
    public partial class AddChargesFromJobOrderController : ViewController
    {
        private PopupWindowShowAction addChargesFromJobOrder;
        private EmployeeChargeSlip _empChargeSlip;
        public AddChargesFromJobOrderController()
        {
            this.TargetObjectType = typeof(EmployeeChargeSlip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "AddChargesFromJobOrder";
            this.addChargesFromJobOrder = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.addChargesFromJobOrder.Caption = "Add Charges From Job Order";
            this.addChargesFromJobOrder.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            AddChargesFromJobOrder_CustomizePopupWindowParams);
            this.addChargesFromJobOrder.Execute += new
            PopupWindowShowActionExecuteEventHandler(AddChargesFromJobOrder_Execute
            );
        }
        private void AddChargesFromJobOrder_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _empChargeSlip = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as EmployeeChargeSlip;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "GenJournalHeader_JobOrderDetails_ListView_ECS";//Application.FindListViewId(typeof(GenJournalDetail));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(JobOrderDetail), listViewId)
            ;
            //if (_empChargeSlip.Employee == null)
            //{
            //    throw new
            //        ApplicationException("Employee not specified");
            //}

            //collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(string.Format("[SubAccountNo.No] = '{0}' And [CVLineDate] is not null", _empChargeSlip.Employee.No));
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void AddChargesFromJobOrder_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (JobOrderDetail item in e.PopupWindow.View.SelectedObjects)
            {
                JobOrderDetail wit = _empChargeSlip.Session.GetObjectByKey<JobOrderDetail>(
                    item.Oid);
                EmployeeChargeSlipExpenseDetail ecsd = ReflectionHelper.CreateObject<EmployeeChargeSlipExpenseDetail>(_empChargeSlip.Session);
                ecsd.GenJournalID = _empChargeSlip;
                ecsd.ItemRef = wit.ItemNo;
                ecsd.JobOrderRef = wit.JobOrderInfo;
                ecsd.CostCenter = wit.CostCenter;
                ecsd.Amount = wit.Total;
                ecsd.Save();
            }
        }
    }
}
