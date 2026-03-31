using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CarryOutSO : WinDetailViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.RequisitionWorksheet _ReqWs;
        private BusinessObjectsAlias.CarryOutRequest _Obj;
        private PopupWindowShowAction carryOutRequest;

        public CarryOutSO()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.RequisitionWorksheet);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.CarryOutSO", this.GetType().
            Name);
            this.carryOutRequest = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.SuppressConfirmation = true;
            this.carryOutRequest.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            CarryOutRequest_CustomizePopupWindowParams);
            this.carryOutRequest.Execute += new
            PopupWindowShowActionExecuteEventHandler(CarryOutRequest_Execute);
        }


        private void CarryOutRequest_CustomizePopupWindowParams(object sender,
CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _ReqWs = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject
            as BusinessObjectsAlias.RequisitionWorksheet;
            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (BusinessObjectsAlias.RequisitionWorksheet item in selected)
            {
                if (item.Cancelled)
                {
                    throw new UserFriendlyException("One of the request selected was marked as cancelled");
                }
                if (item.RequisitionInfo.Status != BusinessObjectsAlias.RequisitionStatusEnum.Approved)
                {
                    throw new UserFriendlyException("One of the request selected was not approved");
                }
            }
            _Obj = new BusinessObjectsAlias.CarryOutRequest();
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.SalesOrder;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "CarryOutSO_Detail", true, _Obj);
        }
        private void CarryOutRequest_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.SalesOrder soObj = _ObjectSpace2.CreateObject<BusinessObjectsAlias.SalesOrder>();
            soObj.EntryDate = DateTime.Now;
            var selwo = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            foreach (BusinessObjectsAlias.RequisitionWorksheet item in selwo)
            {
                BusinessObjectsAlias.RequisitionWorksheet rws = _ObjectSpace2.GetObject<BusinessObjectsAlias.RequisitionWorksheet>(item);
                BusinessObjectsAlias.SalesOrderDetail soDetail = _ObjectSpace2.CreateObject<BusinessObjectsAlias.SalesOrderDetail>();
                soDetail.GenJournalID = soObj;
                soDetail.ItemNo = rws.ItemNo;
                soDetail.Description = rws.Description;
                soDetail.Quantity = rws.Quantity;
                soDetail.UOM = rws.UOM;
                soDetail.RequestID = rws.RowID;
                soDetail.RequisitionNo = rws.RequisitionInfo != null ? rws.RequisitionInfo : null;
                soDetail.CostCenter = rws.CostCenter != null ? rws.CostCenter : null;
                soDetail.RequestedBy = rws.RequisitionInfo.RequestedBy != null ? rws.RequisitionInfo.RequestedBy : null;
                soDetail.Facility = rws.Facility ?? null;
                soDetail.FacilityHead = rws.FacilityHead ?? null;
                soDetail.Department = rws.Department ?? null;
                soDetail.DepartmentInCharge = rws.DepartmentInCharge ?? null;
                soDetail.Save();
                //rws.LastCarrySource = soObj;
                rws.Action = BusinessObjectsAlias.RequisitionActionsEnum.SalesOrder;
                rws.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
                rws.Save();
                BusinessObjectsAlias.FATruck flt = _ObjectSpace2.FindObject<BusinessObjectsAlias.FATruck>(DevExpress.Data.Filtering.CriteriaOperator.Parse("[No] = '" + rws.CostCenter.Code + "'"));
                //soObj.Fleet = flt != null ? flt : null;
                soObj.ReferenceNo = rws.RequisitionInfo.SourceNo;
                //soObj.Problem = rws.Reason;
            }

            DetailView viewWO = Application.CreateDetailView(
        _ObjectSpace2, soObj, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;

        }
    }
}
