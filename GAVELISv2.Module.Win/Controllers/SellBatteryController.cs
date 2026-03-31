using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class SellBatteryController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        //private BusinessObjectsAlias.Battery _Batt;
        private BusinessObjectsAlias.BatterySell _Obj;
        private PopupWindowShowAction sellBattery;
        public SellBatteryController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.Battery);
            this.TargetViewType = ViewType.Any;
            string actionID = string.Format("{0}.SellBattery", this.GetType().
            Name);
            this.sellBattery = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.sellBattery.Caption = "Sell Battery";
            this.sellBattery.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            SellBattery_CustomizePopupWindowParams);
            this.sellBattery.Execute += new
            PopupWindowShowActionExecuteEventHandler(SellBattery_Execute);
        }
        private void SellBattery_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView))
            {
                var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
                foreach (BusinessObjectsAlias.Battery item in selwo)
                {
                    if (item.SoldRef != null)
                    {
                        throw new UserFriendlyException("Cannot continue because one or more selected battery has already been sold.");
                    }
                    if (item.LastDetail == null)
                    {
                        throw new UserFriendlyException("Cannot continue because one or more selected battery has never been in service.");
                    }
                    else if (item.LastDetail != null && item.LastDetail.Reason == null)
                    {
                        throw new UserFriendlyException("Cannot continue because one or more selected battery was not marked as for disposal.");
                    }
                    else if (item.LastDetail != null && item.LastDetail.Reason != null && item.LastDetail.Reason.Code != "DISPOSAL")
                    {
                        throw new UserFriendlyException("Cannot continue because one or more selected battery was not marked as for disposal.");
                    }
                }
            }
            else
            {
                BusinessObjectsAlias.Battery battery = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as BusinessObjectsAlias.Battery;
                if (battery.SoldRef != null)
                {
                    throw new UserFriendlyException("Cannot continue because one or more selected battery has already been sold.");
                }
                if (battery.LastDetail == null)
                {
                    throw new UserFriendlyException("Cannot continue because this battery has never been in service.");
                }
                else if (battery.LastDetail != null && battery.LastDetail.Reason == null)
                {
                    throw new UserFriendlyException("Cannot continue because this battery was not marked as for disposal.");
                }
                else if (battery.LastDetail != null && battery.LastDetail.Reason != null && battery.LastDetail.Reason.Code != "DISPOSAL")
                {
                    throw new UserFriendlyException("Cannot continue because this battery was not marked as for disposal.");
                }
            }
            _Obj = new BusinessObjectsAlias.BatterySell();
            _Obj.Declaration = DateTime.Now;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "BatterySell_DetailView", true, _Obj);
        }
        private void SellBattery_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.Invoice invoice;
            if (_Obj.InvoiceDoc == null)
            {
                invoice = _ObjectSpace2.CreateObject<BusinessObjectsAlias.Invoice>();
                invoice.EntryDate = DateTime.Now;
            }
            else
            {
                invoice = _ObjectSpace2.GetObject<BusinessObjectsAlias.Invoice>(
                _Obj.InvoiceDoc);
            }
            BusinessObjectsAlias.InventoryItem scrapBatt = _ObjectSpace2.FindObject<BusinessObjectsAlias.InventoryItem>(BinaryOperator.Parse("[Description]=?", "Scrapped Battery"));
            if (this.View.GetType() == typeof(ListView))
            {
                var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
                foreach (BusinessObjectsAlias.Battery item in selwo)
                {
                    BusinessObjectsAlias.Battery objectVar = _ObjectSpace2.GetObject<BusinessObjectsAlias.Battery>(item);
                    BusinessObjectsAlias.InvoiceDetail createObject = _ObjectSpace2.CreateObject<BusinessObjectsAlias.InvoiceDetail>();
                    invoice.InvoiceDetails.Add(createObject);
                    createObject.ItemNo = scrapBatt ?? null;
                    createObject.Description = string.Format("Battery #{0}", objectVar.BatteryName);
                    createObject.BatterySoldId = objectVar??null;
                }
            }
            else
            {
                BusinessObjectsAlias.Battery battery = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as BusinessObjectsAlias.Battery;
                BusinessObjectsAlias.InvoiceDetail createObject = _ObjectSpace2.CreateObject<BusinessObjectsAlias.InvoiceDetail>();
                invoice.InvoiceDetails.Add(createObject);
                createObject.ItemNo = scrapBatt ?? null;
                createObject.Description = string.Format("Battery #{0}", battery.BatteryName);
                createObject.BatterySoldId = _ObjectSpace2.GetObject<BusinessObjectsAlias.Battery>(battery) ?? null;
            }
            DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
            invoice, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;
        }
    }
}
