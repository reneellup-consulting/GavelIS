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
//using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class CarryOutRequest : WinDetailViewController {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.RequisitionWorksheet _ReqWs;
        private BusinessObjectsAlias.CarryOutRequest _Obj;
        private PopupWindowShowAction carryOutRequest;
        public CarryOutRequest() {
            this.TargetObjectType = typeof(BusinessObjectsAlias.
            RequisitionWorksheet);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.CarryOutRequest", this.GetType(
            ).Name);
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
        CustomizePopupWindowParamsEventArgs e) {
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
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.
            PurchaseOrder;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "CarryOutRequest_Detail", true, _Obj);
        }
        private void CarryOutRequest_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.PurchaseOrder poObj;
            StringBuilder sbr = new StringBuilder();
            StringBuilder sbr1 = new StringBuilder();
            StringBuilder sbr2 = new StringBuilder("REF#s: ");
            List<string> strRefs = new List<string>();
            List<string> strRefs1 = new List<string>();
            if (_Obj.POrders == null) {
                poObj = _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                PurchaseOrder>();
                poObj.EntryDate = DateTime.Now;
                strRefs = new List<string>();
            } else {
                poObj = _ObjectSpace2.GetObject<BusinessObjectsAlias.
                PurchaseOrder>(_Obj.POrders);
                strRefs = poObj.Refs ?? new List<string>();
                sbr.AppendFormat("{0},", poObj.ReferenceNo);
                if (!string.IsNullOrEmpty(poObj.Remarks))
                {
                    sbr1.AppendFormat("{0},", poObj.Remarks).AppendLine();
                }
                foreach (BusinessObjectsAlias.PurchaseOrderDetail podet in poObj.PurchaseOrderDetails)
                {
                    if (podet.RequisitionNo != null && !string.IsNullOrEmpty(podet.RequisitionNo.ReferenceNo))
                    {
                        if (!strRefs1.Contains(podet.RequisitionNo.ReferenceNo))
                        {
                            strRefs1.Add(podet.RequisitionNo.ReferenceNo);
                            sbr2.AppendFormat("{0},", podet.RequisitionNo.ReferenceNo);
                        }
                    }
                }
            }
            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (BusinessObjectsAlias.RequisitionWorksheet item in selected) 
            {
                BusinessObjectsAlias.RequisitionWorksheet rws = _ObjectSpace2.
                GetObject<BusinessObjectsAlias.RequisitionWorksheet>(item);
                BusinessObjectsAlias.PurchaseOrderDetail poDetail = 
                _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                PurchaseOrderDetail>();
                poDetail.GenJournalID = poObj;
                poDetail.ItemNo = rws.ItemNo;
                poDetail.Description = rws.Description;
                poDetail.Quantity = rws.Quantity;
                poDetail.UOM = rws.UOM;
                poDetail.RequestID = rws.RowID;
                poDetail.RequisitionNo = rws.RequisitionInfo != null ? rws.RequisitionInfo : null;
                poDetail.CostCenter = rws.CostCenter != null ? rws.CostCenter : null;
                poDetail.Facility = rws.Facility ?? null;
                poDetail.FacilityHead = rws.FacilityHead ?? null;
                poDetail.Department = rws.Department ?? null;
                poDetail.DepartmentInCharge = rws.DepartmentInCharge ?? null;
                poDetail.RequestedBy = rws.RequisitionInfo.RequestedBy != null ? rws.RequisitionInfo.RequestedBy : null;
                poDetail.Remarks = rws.Reason;
                poDetail.Save();
                if (!strRefs.Contains(rws.RequisitionInfo.SourceNo))
                {
                    strRefs.Add(rws.RequisitionInfo.SourceNo);
                    sbr.AppendFormat("{0},", rws.RequisitionInfo.SourceNo);
                    if (!string.IsNullOrEmpty(rws.RequisitionInfo.Comments))
                    {
                        sbr1.AppendFormat("{0},", rws.RequisitionInfo.Comments).AppendLine();
                    }
                }
                if (poDetail.RequisitionNo != null && !string.IsNullOrEmpty(poDetail.RequisitionNo.ReferenceNo))
                {
                    if (!strRefs1.Contains(poDetail.RequisitionNo.ReferenceNo))
                    {
                        strRefs1.Add(poDetail.RequisitionNo.ReferenceNo);
                        sbr2.AppendFormat("{0},", poDetail.RequisitionNo.ReferenceNo);
                    }
                }
                //rws.LastCarrySource = poObj;
                rws.Action = BusinessObjectsAlias.RequisitionActionsEnum.
                PurchaseOrder;
                rws.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
                rws.Save();
                if (_Obj.POrders == null) {
                    //poObj.ReferenceNo = rws.RequisitionInfo.SourceNo;
                    poObj.ExpectedDate = rws.ExpectedDate;
                }
            }
            if (sbr.Length > 0)
            {
                sbr.Remove(sbr.Length - 1, 1);
            }
            sbr1 = TrimEnd(sbr1);
            if (sbr1.Length > 0)
            {
                sbr1.Remove(sbr1.Length - 1, 1);
            }
            if (sbr2.ToString() == "REF#s: ")
            {
                sbr2.Clear();
            }
            else
            {
                sbr2.Remove(sbr2.Length - 1, 1);
            }
            poObj.Refs = strRefs;
            poObj.ReferenceNo = sbr.ToString();
            poObj.Remarks = sbr2.ToString() + Environment.NewLine + sbr1.ToString();
            DetailView view = Application.CreateDetailView(_ObjectSpace2, poObj, 
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
        }

        private StringBuilder TrimEnd(StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            int i = sb.Length - 1;
            for (; i >= 0; i--)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;

            return sb;
        }
    }
}
