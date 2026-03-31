using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class UpdateDoleFromTariffValues : ViewController {
        private DolefilTrip dolefilTrip;
        private SimpleAction updateStanFromTariffValues;
        public UpdateDoleFromTariffValues() {
            this.TargetObjectType = typeof(DolefilTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.UpdateDoleFromTariffValues", 
            this.GetType().Name);
            this.updateStanFromTariffValues = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.updateStanFromTariffValues.Caption = "Update Tariff Values";
            this.updateStanFromTariffValues.Execute += new 
            SimpleActionExecuteEventHandler(UpdateDoleFromTariffValues_Execute);
            this.updateStanFromTariffValues.Executed += new EventHandler<
            ActionBaseEventArgs>(UpdateStanFromTariffValues_Executed);
            this.updateStanFromTariffValues.ConfirmationMessage = 
            "Do you really want to update from Tariff values?";
        }
        private void UpdateDoleFromTariffValues_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            dolefilTrip = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as DolefilTrip;
            Tariff tariff = ObjectSpace.GetObjectByKey<Tariff>(dolefilTrip.
            Tariff.Oid);
            if (tariff != null) {
                // Origin
                dolefilTrip.Origin = tariff.Origin;
                // Destination
                dolefilTrip.Destination = tariff.Destination;
                // Distance
                dolefilTrip.Distance = tariff.Distance;
                // VATRate
                dolefilTrip.VATRate = tariff.TaxCode != null ? tariff.TaxCode.
                Rate : 0;
                // WHTRate
                dolefilTrip.WHTInclusive = tariff.WHTInclusive;
                dolefilTrip.WHTRate = tariff.WHTInclusive ? tariff.WHTGroupCode 
                != null ? tariff.WHTGroupCode.WHTRate : 0 : 0;
                dolefilTrip.Save();
                ObjectSpace.CommitChanges();
                //dolefilTrip.Session.CommitTransaction();
            } else {
                throw new ApplicationException("Tariff not found");
            }
        }
        private void UpdateStanFromTariffValues_Executed(object sender, 
        ActionBaseEventArgs e) {
            ObjectSpace.ReloadObject(dolefilTrip);
            ObjectSpace.Refresh();
        }
    }
}
