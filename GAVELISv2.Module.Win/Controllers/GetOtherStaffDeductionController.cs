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
    public partial class GetOtherStaffDeductionController : ViewController
    {
        private PopupWindowShowAction getOtherStaffDeduction;
        private StaffPayrollBatch _staffPayrollBatch;
        public GetOtherStaffDeductionController()
        {
            this.TargetObjectType = typeof(StaffPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "StaffPayrollBatch.GetOtherStaffDeduction";
            this.getOtherStaffDeduction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.getOtherStaffDeduction.Caption = "Get Staff Other Deduction";
            this.getOtherStaffDeduction.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            GetOtherStaffDeduction_CustomizePopupWindowParams);
            this.getOtherStaffDeduction.Execute += new
            PopupWindowShowActionExecuteEventHandler(GetOtherStaffDeduction_Execute
            );
        }
        private void GetOtherStaffDeduction_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _staffPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as StaffPayrollBatch;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(EmpOtherDed));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(EmpOtherDed), listViewId)
            ;
            //if (_staffPayrollBatch.Customer == null)
            //{
            //    throw new
            //        ApplicationException("Customer not specified");
            //}

            //collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse("[TripID.TripCustomer.No] = '" + _driverPayrollBatch.Customer.No + "' And [Status] = 'Current'");

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void GetOtherStaffDeduction_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (EmpOtherDed item in e.PopupWindow.View.SelectedObjects)
            {
                EmpOtherDed dr = _staffPayrollBatch.Session.GetObjectByKey<EmpOtherDed>(
                    item.Oid);
                Employee emp = _staffPayrollBatch.Session.GetObjectByKey<Employee>(
                    item.Employee.Oid);
                OtherDeduction od = _staffPayrollBatch.Session.GetObjectByKey<OtherDeduction>(
                    item.DedCode.Oid);
                PayrollDeductionOther pdo = ReflectionHelper.CreateObject<PayrollDeductionOther>(_staffPayrollBatch.Session);
                pdo.PayrollBatchID = _staffPayrollBatch;
                pdo.Employee = emp;
                pdo.DeductionDate = _staffPayrollBatch.EntryDate;
                pdo.DeductionType = DeductionType.Other;
                pdo.DeductionCode = od;
                pdo.AdvanceEntryDate = item.EntryDate;
                pdo.Explanation = item.Explanation;
                pdo.RefNo = item.RefNo;
                if (item.Balance < item.Deduction)
                {
                    pdo.Amount = item.Balance;
                }
                else
                {
                    pdo.Amount = item.Deduction;
                }
                pdo.Balance = item.Balance - pdo.Amount;
                pdo.Save();

            }
        }
    }
}
