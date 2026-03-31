using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TireCarryOutController : ViewController {
        private IObjectSpace _Objs;
        private CarryOutDettachedTire _Codt;
        private TireToRetreader _Ttr;
        private PopupWindowShowAction markAsUsableAction;
        private PopupWindowShowAction toRecapAction;
        private PopupWindowShowAction toRegrooveAction;
        private PopupWindowShowAction toRepairAction;
        private PopupWindowShowAction toScapAction;
        public TireCarryOutController() {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "Tire_ListView_Dettached_All";

            #region Mark as Usable

            this.markAsUsableAction = new PopupWindowShowAction(this, "Tire.markAsUsableAction",
            PredefinedCategory.RecordEdit);
            this.markAsUsableAction.Caption = "Mark as Usable";
            this.markAsUsableAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(markAsUsableAction_CustomizePopupWindowParams);
            this.markAsUsableAction.Execute += new PopupWindowShowActionExecuteEventHandler(markAsUsableAction_Execute);

            #endregion

            #region To Recapping

            this.toRecapAction = new PopupWindowShowAction(this, "Tire.toRecapAction",
            PredefinedCategory.RecordEdit);
            this.toRecapAction.Caption = "To Recapping";
            this.toRecapAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(toRecapAction_CustomizePopupWindowParams);
            this.toRecapAction.Execute += new PopupWindowShowActionExecuteEventHandler(toRecapAction_Execute);

            #endregion

            #region To Regrooving

            //this.toRegrooveAction = new PopupWindowShowAction(this, "Tire.toRegrooveAction",
            //PredefinedCategory.RecordEdit);
            //this.toRegrooveAction.Caption = "To Regrooving";
            //this.toRegrooveAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(toRegrooveAction_CustomizePopupWindowParams);
            //this.toRegrooveAction.Execute += new PopupWindowShowActionExecuteEventHandler(toRegrooveAction_Execute);

            #endregion

            #region To Repair

            //this.toRepairAction = new PopupWindowShowAction(this, "Tire.toRepairAction",
            //PredefinedCategory.RecordEdit);
            //this.toRepairAction.Caption = "To Repair";
            //this.toRepairAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(toRepairAction_CustomizePopupWindowParams);
            //this.toRepairAction.Execute += new PopupWindowShowActionExecuteEventHandler(toRepairAction_Execute);

            #endregion

            #region To Scrap

            this.toScapAction = new PopupWindowShowAction(this, "Tire.toScapAction",
            PredefinedCategory.RecordEdit);
            this.toScapAction.Caption = "To Scrap";
            this.toScapAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(toScapAction_CustomizePopupWindowParams);
            this.toScapAction.Execute += new PopupWindowShowActionExecuteEventHandler(toScapAction_Execute);

            #endregion

        }

        void toScapAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            IObjectSpace _Objs2 = Application.CreateObjectSpace();
            Tire tr = _Objs2.GetObject<Tire>(((DevExpress.ExpressApp.ListView)this.View).CurrentObject as Tire);
            if (tr.LastDetail.ActivityType != TireActivityTypeEnum.Dettached)
            {
                throw new ApplicationException("Last detail found is not tire activity type Dettached");
            }
            tr.CarriedOut = TireCarryOutTypeEnum.Scrapped;
            tr.LastCarriedOut = TireCarryOutTypeEnum.Scrapped;
            tr.DateDeclared = _Codt.DateDeclared;
            tr.DateScrapped = _Codt.DateDeclared;
            tr.LastDetail.Reason = _Objs2.FindObject<TireDettachReason>(new BinaryOperator("Code", "SCRAP"));
            tr.LastDetail.Remarks = _Codt.Remarks;
            tr.Save();
            _Objs2.CommitChanges();
            DetailView view = Application.CreateDetailView(_Objs2, tr,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
            view.Closed += new EventHandler(view_Closed);
        }

        void toScapAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            _Objs = Application.CreateObjectSpace();
            _Codt = new CarryOutDettachedTire() { Action = TireCarryOutTypeEnum.Scrapped,
                DateDeclared = DateTime.Now,
            };
            e.View = Application.CreateDetailView(_Objs, "CarryOutDettachedTire_NonRet", true, _Codt);
        }

        void toRepairAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            IObjectSpace _Objs2 = Application.CreateObjectSpace();
            if (_Codt.ToRetreaderDoc == null)
            {
                _Ttr = _Objs2.CreateObject<TireToRetreader>();
                _Ttr.CarryOutAction = TireCarryOutTypeEnum.OnRepair;
            }
            else
            {
                _Ttr = _Objs2.GetObject<TireToRetreader>(_Codt.ToRetreaderDoc);
            }
            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (Tire item in selected)
            {
                Tire tr = _Objs2.GetObject<Tire>(item);
                TireToRetDetail ttrd = _Objs2.CreateObject<TireToRetDetail>();
                ttrd.DocNo = _Ttr;
                ttrd.TireNo = tr;
                ttrd.Description = tr.TireItem.Description;
                ttrd.SerialBrandingNo = !string.IsNullOrEmpty(tr.LastBrandingNo) ? tr.LastBrandingNo : tr.SerialNo;
                ttrd.Remarks = tr.FindingsAndOperations;
                if (tr.LastDetail.ActivityType != TireActivityTypeEnum.Dettached)
                {
                    throw new ApplicationException("Last detail found is not tire activity type Dettached");
                }
                tr.LastDetail.TorId = _Ttr;
                tr.CarriedOut = TireCarryOutTypeEnum.OnRepair;
                tr.LastCarriedOut = TireCarryOutTypeEnum.OnRepair;
                tr.Save();
                ttrd.Save();
            }
            DetailView view = Application.CreateDetailView(_Objs2, _Ttr,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
            view.Closed += new EventHandler(view_Closed);
        }

        void toRepairAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            _Objs = Application.CreateObjectSpace();
            _Codt = new CarryOutDettachedTire()
            {
                Action = TireCarryOutTypeEnum.OnRepair
            };
            e.View = Application.CreateDetailView(_Objs, "CarryOutDettachedTire_DetailView", true, _Codt);
        }

        void toRegrooveAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            IObjectSpace _Objs2 = Application.CreateObjectSpace();
            if (_Codt.ToRetreaderDoc == null)
            {
                _Ttr = _Objs2.CreateObject<TireToRetreader>();
                _Ttr.CarryOutAction = TireCarryOutTypeEnum.OnRegroove;
            }
            else
            {
                _Ttr = _Objs2.GetObject<TireToRetreader>(_Codt.ToRetreaderDoc);
            }
            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (Tire item in selected)
            {
                Tire tr = _Objs2.GetObject<Tire>(item);
                TireToRetDetail ttrd = _Objs2.CreateObject<TireToRetDetail>();
                ttrd.DocNo = _Ttr;
                ttrd.TireNo = tr;
                ttrd.Description = tr.TireItem.Description;
                ttrd.SerialBrandingNo = !string.IsNullOrEmpty(tr.LastBrandingNo) ? tr.LastBrandingNo : tr.SerialNo;
                ttrd.Remarks = tr.FindingsAndOperations;
                if (tr.LastDetail.ActivityType != TireActivityTypeEnum.Dettached)
                {
                    throw new ApplicationException("Last detail found is not tire activity type Dettached");
                }
                tr.LastDetail.TorId = _Ttr;
                tr.CarriedOut = TireCarryOutTypeEnum.OnRegroove;
                tr.LastCarriedOut = TireCarryOutTypeEnum.OnRegroove;
                tr.Save();
                ttrd.Save();
            }
            DetailView view = Application.CreateDetailView(_Objs2, _Ttr,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
            view.Closed += new EventHandler(view_Closed);
        }

        void toRegrooveAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            _Objs = Application.CreateObjectSpace();
            _Codt = new CarryOutDettachedTire()
            {
                Action = TireCarryOutTypeEnum.OnRegroove
            };
            e.View = Application.CreateDetailView(_Objs, "CarryOutDettachedTire_DetailView", true, _Codt);
        }

        void toRecapAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            IObjectSpace _Objs2 = Application.CreateObjectSpace();
            if (_Codt.ToRetreaderDoc == null)
            {
                _Ttr = _Objs2.CreateObject<TireToRetreader>();
                _Ttr.CarryOutAction = TireCarryOutTypeEnum.OnRecap;
            }
            else
            {
                _Ttr = _Objs2.GetObject<TireToRetreader>(_Codt.ToRetreaderDoc);
            }
            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (Tire item in selected)
            {
                Tire tr = _Objs2.GetObject<Tire>(item);
                TireToRetDetail ttrd = _Objs2.CreateObject<TireToRetDetail>();
                ttrd.DocNo = _Ttr;
                ttrd.TireNo = tr;
                ttrd.Description = tr.TireItem.Description;
                ttrd.SerialBrandingNo = !string.IsNullOrEmpty(tr.LastBrandingNo) ? tr.LastBrandingNo : tr.SerialNo;
                ttrd.Remarks = tr.FindingsAndOperations;
                if (tr.LastDetail.ActivityType != TireActivityTypeEnum.Dettached)
                {
                    throw new ApplicationException("Last detail found is not tire activity type Dettached");
                }
                tr.LastDetail.TorId = _Ttr;
                tr.CarriedOut = TireCarryOutTypeEnum.OnRecap;
                tr.LastCarriedOut = TireCarryOutTypeEnum.OnRecap;
                tr.Save();
                ttrd.Save();
            }
            DetailView view = Application.CreateDetailView(_Objs2, _Ttr,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
            view.Closed += new EventHandler(view_Closed);
        }

        void view_Closed(object sender, EventArgs e)
        {
            this.ObjectSpace.Refresh();
        }

        void toRecapAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            _Objs = Application.CreateObjectSpace();
            _Codt = new CarryOutDettachedTire() { Action = TireCarryOutTypeEnum.OnRecap
            };
            e.View = Application.CreateDetailView(_Objs, "CarryOutDettachedTire_DetailView", true, _Codt);
        }

        void markAsUsableAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            IObjectSpace _Objs2 = Application.CreateObjectSpace();
            Tire tr = _Objs2.GetObject<Tire>(((DevExpress.ExpressApp.ListView)this.View).CurrentObject as Tire);
            if (tr.LastDetail.ActivityType != TireActivityTypeEnum.Dettached)
            {
                throw new ApplicationException("Last detail found is not tire activity type Dettached");
            }
            tr.CarriedOut = TireCarryOutTypeEnum.Usable;
            tr.LastCarriedOut = TireCarryOutTypeEnum.Usable;
            tr.DateDeclared = _Codt.DateDeclared;
            //tr.DateScrapped = _Codt.DateDeclared;
            tr.LastDetail.Reason = _Objs2.FindObject<TireDettachReason>(new BinaryOperator("Code", "USABLE"));
            tr.LastDetail.Remarks = _Codt.Remarks;
            tr.Save();
            _Objs2.CommitChanges();
            DetailView view = Application.CreateDetailView(_Objs2, tr,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
            view.Closed += new EventHandler(view_Closed);
        }

        void markAsUsableAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            _Objs = Application.CreateObjectSpace();
            _Codt = new CarryOutDettachedTire() { Action = TireCarryOutTypeEnum.Usable,
                DateDeclared = DateTime.Now
            };
            e.View = Application.CreateDetailView(_Objs, "CarryOutDettachedTire_NonRet", true, _Codt);
        }
    }
}
