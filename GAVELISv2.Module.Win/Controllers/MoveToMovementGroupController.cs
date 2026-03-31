using System;
using System.Linq;
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
using DevExpress.Data.Filtering;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class MoveToMovementGroup : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.ItemMovementFreqAnalDetail _AnalDet;
        private BusinessObjectsAlias.MoveToMovement _Obj;
        private PopupWindowShowAction moveToMovement;
        public MoveToMovementGroup()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.
            ItemMovementFreqAnalDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.MoveToMovement", this.GetType(
            ).Name);
            this.moveToMovement = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.moveToMovement.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            moveToMovement_CustomizePopupWindowParams);
            this.moveToMovement.Execute += new
            PopupWindowShowActionExecuteEventHandler(moveToMovement_Execute);
        }
        private void moveToMovement_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _AnalDet = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject
            as BusinessObjectsAlias.ItemMovementFreqAnalDetail;
            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            _Obj = new BusinessObjectsAlias.MoveToMovement();
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "MoveToMovement_Detail", true, _Obj);
        }
        private void moveToMovement_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.ItemsMovementGroup imgObj;
            
            if (_Obj.ItemMoveGroup == null)
            {
                imgObj = _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                ItemsMovementGroup>();
            }
            else
            {
                imgObj = _ObjectSpace2.GetObject<BusinessObjectsAlias.
                ItemsMovementGroup>(_Obj.ItemMoveGroup);

                //if (imgObj.ItemsMovementGroupDetails.Count > 0)
                //{
                //    throw new UserFriendlyException("Details already exist. Please delete all its contents first.");
                //}
            }

            imgObj.SortBy = _Obj.SortBy;

            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (BusinessObjectsAlias.ItemMovementFreqAnalDetail item in selected)
            {
                BusinessObjectsAlias.ItemMovementFreqAnalDetail oItmfad = _ObjectSpace2.
                GetObject<BusinessObjectsAlias.ItemMovementFreqAnalDetail>(item);

                BusinessObjectsAlias.ItemsMovementGroupDetail oImgdet = null;

                BusinessObjectsAlias.ItemsMovementGroupDetail itm = imgObj.ItemsMovementGroupDetails.Where(o => o.ItemNo == oItmfad.ItemNo).FirstOrDefault();

                if (itm != null)
                {
                    continue;
                }

                oImgdet = _ObjectSpace2.CreateObject<BusinessObjectsAlias.ItemsMovementGroupDetail>();
                oImgdet.HeaderId = imgObj;
                oImgdet.ItemNo = oItmfad.ItemNo;
                oImgdet.NoOfInstance = oItmfad.NoOfInstance;
                oImgdet.Rcpt = oItmfad.Rcpt;
                oImgdet.Invc = oItmfad.Invc;
                oImgdet.Word = oItmfad.Word;
                oImgdet.Ecs = oItmfad.Ecs;
                oImgdet.Cm = oItmfad.Cm;
                oImgdet.Dm = oItmfad.Dm;
                oImgdet.Fpr = oItmfad.Fpr;
                oImgdet.Save(); 
            }
            
            DetailView view = Application.CreateDetailView(_ObjectSpace2, imgObj,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
        }
    }
}
