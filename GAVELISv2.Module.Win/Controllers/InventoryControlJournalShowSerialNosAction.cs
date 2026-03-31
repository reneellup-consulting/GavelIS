using System;
using System.ComponentModel;
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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class InventoryControlJournalShowSerialNosAction : 
    ViewController {
        private PopupWindowShowAction inventoryControlJournalShowSerialNosAction
        ;
        private InventoryControlJournal _InventoryControlJournal;
        public InventoryControlJournalShowSerialNosAction() {
            this.TargetObjectType = typeof(InventoryControlJournal);
            this.TargetViewType = ViewType.Any;
            string actionID = "InventoryControlJournal.ShowSerialNos";
            this.inventoryControlJournalShowSerialNosAction = new 
            PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit)
            ;
            this.inventoryControlJournalShowSerialNosAction.
            CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            InventoryControlJournalShowSerialNosAction_CustomizePopupWindowParams
            );
        }
        private void 
        InventoryControlJournalShowSerialNosAction_CustomizePopupWindowParams(
        object sender, CustomizePopupWindowParamsEventArgs e) {
            _InventoryControlJournal = this.View.CurrentObject as 
            InventoryControlJournal;
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(
            ItemTrackingEntry));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(ItemTrackingEntry), 
            listViewId);
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(
            "[IcjID.Oid]= '" + _InventoryControlJournal.Oid + "'");
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
    }
}
