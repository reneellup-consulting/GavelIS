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
    public partial class SellRevolvingPartsController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        //private BusinessObjectsAlias.RevolvingPart _Batt;
        private BusinessObjectsAlias.RevolvingPartSell _Obj;
        private PopupWindowShowAction sellRevolvingPart;
        public SellRevolvingPartsController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.RevolvingPart);
            this.TargetViewType = ViewType.Any;
            string actionID = string.Format("{0}.SellRevolvingPart", this.GetType().
            Name);
            this.sellRevolvingPart = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.sellRevolvingPart.Caption = "Sell Part";
            this.sellRevolvingPart.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            SellRevolvingPart_CustomizePopupWindowParams);
            this.sellRevolvingPart.Execute += new
            PopupWindowShowActionExecuteEventHandler(SellRevolvingPart_Execute);
        }
        private void SellRevolvingPart_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView))
            {
                var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
                foreach (BusinessObjectsAlias.RevolvingPart item in selwo)
                {
                    if (item.SoldDocId != null)
                    {
                        throw new UserFriendlyException("Cannot continue because one or more selected revolvingPart has already been sold.");
                    }
                    if (item.LastDetail == null)
                    {
                        throw new UserFriendlyException("Cannot continue because one or more selected revolvingPart has never been in service.");
                    }
                    else if (item.LastDetail != null && item.LastDetail.Reason == null)
                    {
                        throw new UserFriendlyException("Cannot continue because one or more selected revolvingPart was not marked as for disposal.");
                    }
                    else if (item.LastDetail != null && item.LastDetail.Reason != null && item.LastDetail.Reason.Code != "DISPOSAL")
                    {
                        throw new UserFriendlyException("Cannot continue because one or more selected revolvingPart was not marked as for disposal.");
                    }
                }
            }
            else
            {
                BusinessObjectsAlias.RevolvingPart revolvingPart = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as BusinessObjectsAlias.RevolvingPart;
                if (revolvingPart.SoldDocId != null)
                {
                    throw new UserFriendlyException("Cannot continue because one or more selected revolvingPart has already been sold.");
                }
                if (revolvingPart.LastDetail == null)
                {
                    throw new UserFriendlyException("Cannot continue because this revolvingPart has never been in service.");
                }
                else if (revolvingPart.LastDetail != null && revolvingPart.LastDetail.Reason == null)
                {
                    throw new UserFriendlyException("Cannot continue because this revolvingPart was not marked as for disposal.");
                }
                else if (revolvingPart.LastDetail != null && revolvingPart.LastDetail.Reason != null && revolvingPart.LastDetail.Reason.Code != "DISPOSAL")
                {
                    throw new UserFriendlyException("Cannot continue because this revolvingPart was not marked as for disposal.");
                }
            }
            _Obj = new BusinessObjectsAlias.RevolvingPartSell();
            _Obj.Declaration = DateTime.Now;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "RevolvingPartSell_DetailView", true, _Obj);
        }
        private void SellRevolvingPart_Execute(object sender,
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
            BusinessObjectsAlias.InventoryItem scrapBatt = _ObjectSpace2.FindObject<BusinessObjectsAlias.InventoryItem>(BinaryOperator.Parse("[Description]=?", "Scrapped RevolvingPart"));
            if (this.View.GetType() == typeof(ListView))
            {
                var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
                foreach (BusinessObjectsAlias.RevolvingPart item in selwo)
                {
                    BusinessObjectsAlias.RevolvingPart objectVar = _ObjectSpace2.GetObject<BusinessObjectsAlias.RevolvingPart>(item);
                    BusinessObjectsAlias.InvoiceDetail createObject = _ObjectSpace2.CreateObject<BusinessObjectsAlias.InvoiceDetail>();
                    invoice.InvoiceDetails.Add(createObject);
                    createObject.ItemNo = scrapBatt ?? null;
                    createObject.Description = string.Format("RevolvingPart #{0}", objectVar.PartNo);
                    createObject.RevPartSoldId = objectVar ?? null;
                }
            }
            else
            {
                BusinessObjectsAlias.RevolvingPart revolvingPart = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as BusinessObjectsAlias.RevolvingPart;
                BusinessObjectsAlias.InvoiceDetail createObject = _ObjectSpace2.CreateObject<BusinessObjectsAlias.InvoiceDetail>();
                invoice.InvoiceDetails.Add(createObject);
                createObject.ItemNo = scrapBatt ?? null;
                createObject.Description = string.Format("RevolvingPart #{0}", revolvingPart.PartNo);
                createObject.RevPartSoldId = _ObjectSpace2.GetObject<BusinessObjectsAlias.RevolvingPart>(revolvingPart) ?? null;
            }
            DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
            invoice, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;
        }
    }
}
