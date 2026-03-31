using System;
using System.Linq;
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
    public partial class RevolvingPartDisposalController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private RevolvingPart _RevolvingPart;
        private RevolvingPartDetail _Obj;
        private PopupWindowShowAction revolvingPartToScrapAction;
        public RevolvingPartDisposalController()
        {
            this.TargetObjectType = typeof(RevolvingPart);
            this.TargetViewType = ViewType.Any;
            string actionID = "RevolvingPart.ToScrap";
            this.revolvingPartToScrapAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.revolvingPartToScrapAction.Caption = "For Disposal";
            this.revolvingPartToScrapAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(revolvingPartToScrapAction_CustomizePopupWindowParams);
            this.revolvingPartToScrapAction.Execute += new PopupWindowShowActionExecuteEventHandler(revolvingPartToScrapAction_Execute);
        }
        void revolvingPartToScrapAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            //Item battItem = _ObjectSpace2.FindObject<Item>(BinaryOperator.Parse("[Description] = ?", "Scrapped Revolving Part"));
            //if (battItem == null)
            //{
            //    throw new UserFriendlyException("RevolvingPart item does not exist.");
            //}
            PhysicalAdjustment phys;
            if (_Obj.AdjustmentDoc == null)
            {
                phys = _ObjectSpace2.CreateObject<PhysicalAdjustment>();
                phys.EntryDate = DateTime.Now;
                phys.WarehouseLocation = _ObjectSpace2.GetObject(_Obj.Location);
            }
            else
            {
                phys = _ObjectSpace2.GetObject<PhysicalAdjustment>(
                _Obj.AdjustmentDoc);
            }
            PhysicalAdjustmentDetail createObject = _ObjectSpace2.CreateObject<PhysicalAdjustmentDetail>();
            phys.PhysicalAdjustmentDetails.Add(createObject);
            if (_Obj.ScrappedItemNo == null)
            {
                throw new ApplicationException("Please specify scrapped item no");
            }
            createObject.ItemNo = _ObjectSpace2.GetObject(_Obj.ScrappedItemNo);
            createObject.ActualQtyStock = 1m;
            createObject.BatteryRef = _Obj.RevolvingPartId.PartNo;
            createObject.Warehouse = _ObjectSpace2.GetObject(_Obj.Location);
            createObject.Save();
            StringBuilder sb = new StringBuilder();
            if (phys.PhysicalAdjustmentDetails.Count > 0)
            {
                sb.Append("Scrapping revolvingPart #s ");
            }
            foreach (var item in phys.PhysicalAdjustmentDetails)
            {
                sb.AppendFormat("{0}, ", item.BatteryRef);
            }
            sb.Remove(sb.Length - 2, 2);
            phys.Memo = sb.ToString();
            phys.Save();
            _ObjectSpace2.CommitChanges();
            _Obj.AdjustmentDoc = _ObjectSpace.GetObject(phys);
            _Obj.Save();
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
            if (_Obj.OpenAdjustmentDoc)
            {
                DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
            phys, true);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = viewWO;
            }
        }

        void revolvingPartToScrapAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView))
            {
                _RevolvingPart = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as RevolvingPart;
            }
            else
            {
                _RevolvingPart = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as RevolvingPart;
            }

            ObjectSpace.CommitChanges();
            RevolvingPart thisBatt = _ObjectSpace.GetObjectByKey<RevolvingPart>(
            _RevolvingPart.Oid);
            if (thisBatt.LastDetail != null && thisBatt.LastDetail.Reason != null && thisBatt.LastDetail.Reason.Code == "DISPOSAL")
            {
                throw new UserFriendlyException("Cannot continue because this revolvingPart has already been disposed.");
            }
            if (thisBatt.LastUnitAttachedTo != null)
            {
                _Obj = _ObjectSpace.CreateObject<RevolvingPartDetail>();
                _Obj.RevolvingPartId = thisBatt;
                _Obj.EntryDate = DateTime.Now;
                _Obj.ActivityDate = thisBatt.LastActivityDate;
                _Obj.ActivityType = RevolvingPartActivityTypeEnum.Dettach;
                _Obj.Unit = thisBatt.LastUnitAttachedTo;
                _Obj.Status = RevolvingPartsStatusEnum.Scrap;
                _Obj.Reason = _ObjectSpace.FindObject<RevolvingPartsActivityReason>(BinaryOperator.Parse("[Code]=?", "DISPOSAL"));
                _Obj.Save();
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this revolvingPart has never been in service.");
            }
            e.View = Application.CreateDetailView(_ObjectSpace,
            "RevolvingPartDetail_Disposal", true, _Obj);
        }
    }
}
