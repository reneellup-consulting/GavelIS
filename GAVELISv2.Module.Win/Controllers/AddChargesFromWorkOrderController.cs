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
    public partial class AddChargesFromWorkOrderController : ViewController
    {
        private PopupWindowShowAction addChargesFromWorkOrder;
        private EmployeeChargeSlip _empChargeSlip;

        public AddChargesFromWorkOrderController()
        {
            this.TargetObjectType = typeof(EmployeeChargeSlip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "AddChargesFromWorkOrder";
            this.addChargesFromWorkOrder = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.addChargesFromWorkOrder.Caption = "Add Charges From Work Order";
            this.addChargesFromWorkOrder.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            AddChargesFromWorkOrder_CustomizePopupWindowParams);
            this.addChargesFromWorkOrder.Execute += new
            PopupWindowShowActionExecuteEventHandler(AddChargesFromWorkOrder_Execute
            );
        }
        private void AddChargesFromWorkOrder_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e) {
            _empChargeSlip = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as EmployeeChargeSlip;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "GenJournalHeader_WorkOrderItemDetails_ListView_ECS";//Application.FindListViewId(typeof(GenJournalDetail));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(WorkOrderItemDetail), listViewId)
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
        private void AddChargesFromWorkOrder_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (WorkOrderItemDetail item in e.PopupWindow.View.SelectedObjects)
            {
                WorkOrderItemDetail wit = _empChargeSlip.Session.GetObjectByKey<WorkOrderItemDetail>(
                    item.Oid);
                EmployeeChargeSlipExpenseDetail ecsd = ReflectionHelper.CreateObject<EmployeeChargeSlipExpenseDetail>(_empChargeSlip.Session);
                ecsd.GenJournalID = _empChargeSlip;
                ecsd.ItemRef = wit.ItemNo;
                ecsd.WorkOrderRef = wit.WorkOrderInfo;
                ecsd.CostCenter = wit.CostCenter;
                ecsd.Amount = wit.Total;
                ecsd.Save();
            }
        }
    }
}
