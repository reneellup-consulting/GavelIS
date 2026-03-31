using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TiremanActivityController : ViewController {
        private PopupWindowShowAction tiremanAttachAction;
        private PopupWindowShowAction tiremanDettachAction;
        private PopupWindowShowAction tiremanReplaceAction;
        private PopupWindowShowAction tiremanTransferAction;
        private IObjectSpace _ObjectSpace;
        private TiremanDaily _TiremanDailyObj;
        private TiremanDaily _thisObj;
        public TiremanActivityController() {
            this.TargetObjectType = typeof(TiremanDaily);
            this.TargetViewType = ViewType.DetailView;

            #region Tireman Attach

            this.tiremanAttachAction = new PopupWindowShowAction(this, "TiremanAttachActionId",
            PredefinedCategory.RecordEdit);
            this.tiremanAttachAction.Caption = "Attach Tire";
            this.tiremanAttachAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(tiremanAttachAction_CustomizePopupWindowParams);
            this.tiremanAttachAction.Execute += new PopupWindowShowActionExecuteEventHandler(tiremanAttachAction_Execute);

            #endregion

            #region Tireman Dettach

            this.tiremanDettachAction = new PopupWindowShowAction(this, "TiremanDettachActionId",
            PredefinedCategory.RecordEdit);
            this.tiremanDettachAction.Caption = "Dettach Tire";
            this.tiremanDettachAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(tiremanDettachAction_CustomizePopupWindowParams);
            this.tiremanDettachAction.Execute += new PopupWindowShowActionExecuteEventHandler(tiremanDettachAction_Execute);

            #endregion

            #region Tireman Replace

            //this.tiremanReplaceAction = new PopupWindowShowAction(this, "TiremanReplaceActionId",
            //PredefinedCategory.RecordEdit);
            //this.tiremanReplaceAction.Caption = "Replace Tire";
            //this.tiremanReplaceAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(tiremanReplaceAction_CustomizePopupWindowParams);
            //this.tiremanReplaceAction.Execute += new PopupWindowShowActionExecuteEventHandler(tiremanReplaceAction_Execute);

            #endregion

            #region Tireman Transfer

            //this.tiremanTransferAction = new PopupWindowShowAction(this, "TiremanTransferActionId",
            //PredefinedCategory.RecordEdit);
            //this.tiremanTransferAction.Caption = "Transfer Tire";
            //this.tiremanTransferAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(tiremanTransferAction_CustomizePopupWindowParams);
            //this.tiremanTransferAction.Execute += new PopupWindowShowActionExecuteEventHandler(tiremanTransferAction_Execute);

            #endregion

        }

        #region Tireman Transfer Action

        void tiremanTransferAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            throw new NotImplementedException();
        }

        void tiremanTransferAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            throw new NotImplementedException();
        }

        #endregion

        #region Tireman Replace Action

        void tiremanReplaceAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            throw new NotImplementedException();
        }

        void tiremanReplaceAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            throw new NotImplementedException();
        }

        #endregion

        #region Tireman Dettach Action


        void tiremanDettachAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_TiremanDailyObj);
            ObjectSpace.Refresh();
        }

        void tiremanDettachAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _TiremanDailyObj = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
            as TiremanDaily;
            TiremanDaily thisObj = _ObjectSpace.GetObjectByKey<TiremanDaily>(
            _TiremanDailyObj.Oid);
            TiremanActivity _tda = _ObjectSpace.CreateObject<TiremanActivity>();
            _tda.TmDailyId = thisObj;
            _tda.ActivityDate = thisObj.EntryDate;
            _tda.TiremanActivityType = TiremanActivityTypeEnum.Dettach;
            e.View = Application.CreateDetailView(_ObjectSpace, "TiremanActivity_Dettach", true, _tda);
        }

        #endregion

        #region Tireman Attach Action

        void tiremanAttachAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_TiremanDailyObj);
            ObjectSpace.Refresh();
        }

        void tiremanAttachAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _TiremanDailyObj = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
            as TiremanDaily;
            TiremanDaily thisObj = _ObjectSpace.GetObjectByKey<TiremanDaily>(
            _TiremanDailyObj.Oid);
            TiremanActivity _tda = _ObjectSpace.CreateObject<TiremanActivity>();
            _tda.TmDailyId = thisObj;
            _tda.ActivityDate = thisObj.EntryDate;
            _tda.TiremanActivityType = TiremanActivityTypeEnum.Attach;
            e.View = Application.CreateDetailView(_ObjectSpace, "TiremanActivity_Attach", true, _tda);
        }

        #endregion
    }
}
