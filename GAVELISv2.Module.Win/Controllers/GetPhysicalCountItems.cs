using System;
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
    public partial class GetPhysicalCountItems : ViewController
    {
        private PopupWindowShowAction getPhysicalCountItems;
        private PhysicalAdjustment _physicalAdjustment;

        public GetPhysicalCountItems()
        {
            this.TargetObjectType = typeof(PhysicalAdjustment);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "PhysicalAdjustment.GetPhysicalCountItems";
            this.getPhysicalCountItems = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.getPhysicalCountItems.Caption = "Get Physical Count Items";
            this.getPhysicalCountItems.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            GetPhysicalCountItems_CustomizePopupWindowParams);
            this.getPhysicalCountItems.Execute += new
            PopupWindowShowActionExecuteEventHandler(GetPhysicalCountItems_Execute
            );
        }
        private void GetPhysicalCountItems_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e) {
            _physicalAdjustment = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as PhysicalAdjustment;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "Item_ListView_ForCount"; //Application.FindListViewId(typeof(Item));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(Item), listViewId)
            ;
            if (_physicalAdjustment.WarehouseLocation == null)
            {
                throw new
                ApplicationException("Wharehouse location not specified");
            }

            //collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse("[TripID.TripCustomer.No] = '" + _physicalAdjustment.Customer.No + "' And [Status] = 'Current'");

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void GetPhysicalCountItems_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (Item item in e.PopupWindow.View.SelectedObjects){
                Item _itm = _physicalAdjustment.Session.GetObjectByKey<Item>(
                    item.Oid);
                PhysicalAdjustmentDetail pad = ReflectionHelper.CreateObject<PhysicalAdjustmentDetail>(_physicalAdjustment.Session);
                pad.GenJournalID = _physicalAdjustment;
                pad.ItemNo = _itm;
                pad.Warehouse = _physicalAdjustment.WarehouseLocation;
                pad.Save();
            }
            _physicalAdjustment.Save();
        }
    }
}
