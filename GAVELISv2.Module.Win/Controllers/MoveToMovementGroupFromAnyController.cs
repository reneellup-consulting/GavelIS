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
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class MoveToMovementGroupFromAnyController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        //private BusinessObjectsAlias.ItemMovementFreqAnalDetail _AnalDet;
        private BusinessObjectsAlias.Item _lItem;
        private BusinessObjectsAlias.MoveToMovement _Obj;
        private PopupWindowShowAction moveToMovement;
        public MoveToMovementGroupFromAnyController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.IToMovementCapable);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.MoveToMovement", this.GetType(
            ).Name);
            this.moveToMovement = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.moveToMovement.Caption = "Move To Movement Group";
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
            //_AnalDet = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject
            //as BusinessObjectsAlias.ItemMovementFreqAnalDetail;
            //var selected = ((DevExpress.ExpressApp.ListView)this.View).
            //SelectedObjects;
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

            Session session = ((ObjectSpace)_ObjectSpace2).Session;

            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (object item in selected)
            {
                int jCount = 0;

                BusinessObjectsAlias.ItemsMovementGroupDetail oImgdet = null;

                BusinessObjectsAlias.ItemsMovementGroupDetail itm = null;

                BusinessObjectsAlias.Item ro_itm = null;

                foreach (XPMemberInfo property in session.GetClassInfo(item).PersistentProperties)
                {
                    if (property.MemberType == typeof(BusinessObjectsAlias.Item) && jCount == 0)
                    {
                        jCount++;

                        ro_itm = property.GetValue(item) as BusinessObjectsAlias.Item;
                        //ro_itm = property as BusinessObjectsAlias.Item;
                        
                        itm = imgObj.ItemsMovementGroupDetails.Where(o => o.ItemNo == ro_itm).FirstOrDefault();                       
                    }
                }

                if (ro_itm == null)
                {
                    continue;
                }

                if (itm != null)
                {
                    continue;
                }

                oImgdet = _ObjectSpace2.CreateObject<BusinessObjectsAlias.ItemsMovementGroupDetail>();
                oImgdet.HeaderId = imgObj;
                oImgdet.ItemNo = _ObjectSpace2.GetObject(ro_itm);
                oImgdet.NoOfInstance = 0;
                oImgdet.Rcpt = 0;
                oImgdet.Invc = 0;
                oImgdet.Word = 0;
                oImgdet.Ecs = 0;
                oImgdet.Cm = 0;
                oImgdet.Dm = 0;
                oImgdet.Fpr = 0;
                oImgdet.Save();
            }

            DetailView view = Application.CreateDetailView(_ObjectSpace2, imgObj,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
        }
    }
}
