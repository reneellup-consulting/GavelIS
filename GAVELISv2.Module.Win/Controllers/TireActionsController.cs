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
    public partial class TireActionsController : ViewController
    {
        private IObjectSpace _Objs;
        private CarryOutSellTire _CoSt;
        private TireForSale _Tfs;
        private PopupWindowShowAction sellTireAction;
        public TireActionsController()
        {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            #region Sell Tire
            this.sellTireAction = new PopupWindowShowAction(this, "Tire.SellTire",
            PredefinedCategory.RecordEdit);
            this.sellTireAction.Caption = "Sell Tire";
            this.sellTireAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(sellTireAction_CustomizePopupWindowParams);
            this.sellTireAction.Execute += new PopupWindowShowActionExecuteEventHandler(sellTireAction_Execute);
            #endregion
        }

        void sellTireAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            IObjectSpace _Objs2 = Application.CreateObjectSpace();
            if (_CoSt.TireForSaleDoc == null)
            {
                _Tfs = _Objs2.CreateObject<TireForSale>();
                _Tfs.CarryOutAction = TireCarryOutTypeEnum.ForSale;
            }
            else
            {
                _Tfs = _Objs2.GetObject<TireForSale>(_CoSt.TireForSaleDoc);
            }
            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (Tire item in selected)
            {
                Tire tr = _Objs2.GetObject<Tire>(item);
                TireForSaleDet ttrd = _Objs2.CreateObject<TireForSaleDet>();
                ttrd.DocNo = _Tfs;
                ttrd.TireNo = tr;
                ttrd.Description = tr.TireItem.Description;
                ttrd.SerialBrandingNo = !string.IsNullOrEmpty(tr.LastBrandingNo) ? tr.LastBrandingNo : tr.SerialNo;
                ttrd.Remarks = tr.LastDetail.Remarks;
                if (tr.LastDetail.ActivityType != TireActivityTypeEnum.Dettached)
                {
                    throw new ApplicationException("Last detail found is not tire activity type Dettached");
                }
                tr.LastDetail.TfsId = _Tfs;
                tr.CarriedOut = TireCarryOutTypeEnum.ForSale;
                tr.DateDeclared = _Tfs.EntryDate;
                tr.Save();
                ttrd.Save();
            }
            DetailView view = Application.CreateDetailView(_Objs2, _Tfs,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
            view.Closed += new EventHandler(view_Closed);
        }

        void sellTireAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _Objs = Application.CreateObjectSpace();
            _CoSt = new CarryOutSellTire()
            {
                Action = TireCarryOutTypeEnum.ForSale,
                DateDeclared = DateTime.Now,
            };
            e.View = Application.CreateDetailView(_Objs, "CarryOutSellTire_DetailView", true, _CoSt);
        }

        void view_Closed(object sender, EventArgs e)
        {
            this.ObjectSpace.Refresh();
        }

    }
}
