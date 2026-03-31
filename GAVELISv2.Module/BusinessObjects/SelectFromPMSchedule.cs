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

namespace GAVELISv2.Module.BusinessObjects {
    public partial class SelectFromPMSchedule : ViewController {
        private PopupWindowShowAction selectFromPMSchedule;
        private WorkOrder _WorkOrder;
        public SelectFromPMSchedule() {
            this.TargetObjectType = typeof(WorkOrder);
            this.TargetViewType = ViewType.DetailView;
            var actionID = "SelectFromPMSchedule";
            this.selectFromPMSchedule = new PopupWindowShowAction(this, actionID
            , PredefinedCategory.RecordEdit);
            this.selectFromPMSchedule.Caption = "Select From PM";
            this.selectFromPMSchedule.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            SelectFromPMSchedule_CustomizePopupWindowParams);
            this.selectFromPMSchedule.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            SelectFromPMSchedule_Execute);
        }

        private void SelectFromPMSchedule_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _WorkOrder = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as WorkOrder;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            var objectSpace = Application.CreateObjectSpace();
            var listViewId = Application.FindListViewId(typeof(
            PreventiveMaintenance));
            var collectionSource = Application.CreateCollectionSource(
            objectSpace, typeof(PreventiveMaintenance), listViewId);
            //if (_WorkOrder.Customer == null) {throw new 
            //    ApplicationException("Customer not specified");}
            //if (_WorkOrder.FromDate != DateTime.MinValue || 
            //_WorkOrder.ToDate != DateTime.MinValue) {
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '" 
            //    + _WorkOrder.Customer.No + 
            //    "' And [EntryDate] Between(#" + _WorkOrder.
            //    FromDate.ToShortDateString() + "#, #" + _WorkOrder.
            //    ToDate.ToShortDateString() + "#)");
            //}
            //else if (_WorkOrder.Period>0 && _WorkOrder.Week>0)
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _WorkOrder.Customer.No +
            //    "' And [Period] = " + _WorkOrder.Period + " And [Week] = " + _WorkOrder.Week);
            //}
            //else
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _WorkOrder.Customer.No + "'");
            //}
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }

        private void SelectFromPMSchedule_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            if (e.PopupWindow.View.SelectedObjects.Count == 0) {throw new 
                UserFriendlyException("No selected schedule");}
            var sel = e.PopupWindow.View.SelectedObjects[0] as 
            PreventiveMaintenance;
            var pm = _WorkOrder.Session.GetObjectByKey<PreventiveMaintenance>(
            sel.Oid);
            _WorkOrder.PrevMaintenanceID = pm;
            _WorkOrder.Problem = pm.Name;
            _WorkOrder.Fleet = pm.Fleet;
            LastReadings rdngsLast = new LastReadings();
            LastReadings rdngsService = new LastReadings();
            LastReadings rdngsServiceId = new LastReadings();
            rdngsLast = pm.Fleet.GetLastReadingBeforeDate(_WorkOrder.EntryDate);
            rdngsService = pm.Fleet.GetServiceLastReadingBeforeDate(_WorkOrder.EntryDate);
            if (_WorkOrder.PrevMaintenanceID!=null)
            {
                rdngsServiceId = pm.Fleet.GetServiceIdLastReadingBeforeDate(_WorkOrder.EntryDate, _WorkOrder.PrevMaintenanceID);
            }
            else
            {
                rdngsServiceId = null;
            }
            if (rdngsLast !=null)
            {
                _WorkOrder.PrevOdo = rdngsLast.LastServiceRead;
                _WorkOrder.CurrOdo = rdngsLast.LastOdoRead;
            }
            else if (rdngsService!=null)
            {
                _WorkOrder.PrevOdo = rdngsService.LastServiceRead;
                _WorkOrder.CurrOdo = rdngsService.LastOdoRead;
            }
            else if (rdngsServiceId!=null)
            {
                _WorkOrder.PrevOdo = rdngsServiceId.LastServiceRead;
                _WorkOrder.CurrOdo = rdngsServiceId.LastOdoRead;
            }
            else
            {
                _WorkOrder.PrevOdo = 0m;
                _WorkOrder.CurrOdo = 0m;
            }
            //_WorkOrder.PrevOdo = pm.LastRunMeter;
            //_WorkOrder.CurrOdo = pm.Fleet.LastReading;
            if (pm.Fleet.GetType()==typeof(FATruck) && ((FATruck)pm.Fleet).Operator!=null)
            {
                _WorkOrder.Driver = ((FATruck)pm.Fleet).Operator;
            }
            _WorkOrder.Memo = "Scheduled preventive maintenance";
            _WorkOrder.PrevOdo = pm.LastRunMeter;
        }
    }
}
