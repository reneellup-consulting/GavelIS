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
    public partial class GetOtherDriverDeduction : ViewController
    {
        private PopupWindowShowAction getOtherDriverDeduction;
        private DriverPayrollBatch _driverPayrollBatch;

        public GetOtherDriverDeduction()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch.GetOtherDriverDeduction";
            this.getOtherDriverDeduction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.getOtherDriverDeduction.Caption = "Get Drivers Other Deductions";
            this.getOtherDriverDeduction.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            GetOtherDriverDeduction_CustomizePopupWindowParams);
            this.getOtherDriverDeduction.Execute += new
            PopupWindowShowActionExecuteEventHandler(GetOtherDriverDeduction_Execute
            );
        }
        private void GetOtherDriverDeduction_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _driverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as DriverPayrollBatch;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(EmpOtherDed));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(EmpOtherDed), listViewId)
            ;
            //if (_driverPayrollBatch.Customer == null)
            //{
            //    throw new
            //        ApplicationException("Customer not specified");
            //}

            //collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse("[TripID.TripCustomer.No] = '" + _driverPayrollBatch.Customer.No + "' And [Status] = 'Current'");

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void GetOtherDriverDeduction_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (EmpOtherDed item in e.PopupWindow.View.SelectedObjects){
                EmpOtherDed dr = _driverPayrollBatch.Session.GetObjectByKey<EmpOtherDed>(
                    item.Oid);
                Employee emp = _driverPayrollBatch.Session.GetObjectByKey<Employee>(
                    item.Employee.Oid);
                OtherDeduction od = _driverPayrollBatch.Session.GetObjectByKey<OtherDeduction>(
                    item.DedCode.Oid);
                PayrollDeductionOther pdo = ReflectionHelper.CreateObject<PayrollDeductionOther>(_driverPayrollBatch.Session);
                pdo.PayrollBatchID = _driverPayrollBatch;
                pdo.Employee = emp;
                pdo.DeductionDate = _driverPayrollBatch.EntryDate;
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
