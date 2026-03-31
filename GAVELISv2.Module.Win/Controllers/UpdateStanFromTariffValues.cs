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
    public partial class UpdateStanFromTariffValues : ViewController {
        private StanfilcoTrip stanfilcoTrip;
        private SimpleAction updateStanFromTariffValues;
        public UpdateStanFromTariffValues() {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.UpdateStanFromTariffValues", 
            this.GetType().Name);
            this.updateStanFromTariffValues = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.updateStanFromTariffValues.Caption = "Update Tariff Values";
            this.updateStanFromTariffValues.Execute += new 
            SimpleActionExecuteEventHandler(UpdateStanFromTariffValues_Execute);
            this.updateStanFromTariffValues.Executed += new EventHandler<
            ActionBaseEventArgs>(UpdateStanFromTariffValues_Executed);
            this.updateStanFromTariffValues.ConfirmationMessage = 
            "Do you really want to update from Tariff values?";
        }
        private void UpdateStanFromTariffValues_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            stanfilcoTrip = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as StanfilcoTrip;
            Tariff tariff = ObjectSpace.GetObjectByKey<Tariff>(
            stanfilcoTrip.Tariff.Oid);
            if (tariff != null) {
                // Origin
                stanfilcoTrip.Origin = tariff.Origin;
                // Destination
                stanfilcoTrip.Destination = tariff.Destination;
                // Distance
                stanfilcoTrip.Distance = tariff.Distance;
                // Tariff Distance
                stanfilcoTrip.TariffDistance = tariff.TariffDistance;
                // TurnAround
                stanfilcoTrip.TurnAround = tariff.TurnAround;
                // Allowance
                stanfilcoTrip.Allowance = tariff.Allowance;
                // Fuel Allocation
                stanfilcoTrip.FuelAllocation = tariff.Fuel;
                // TruckerPay
                stanfilcoTrip.TruckerPay = tariff.TruckerPay;
                // RateAdjmt
                stanfilcoTrip.RateAdjmt = tariff.RateAdjmt;
                // TariffTruckerPay
                stanfilcoTrip.TariffTruckerPay = tariff.TariffTruckerPay;
                // TariffFuelSubsidy
                stanfilcoTrip.TariffFuelSubsidy = tariff.TariffFuelSubsidy;
                // RentTrailer
                stanfilcoTrip.RentTrailer = tariff.TrailerRent;
                // TrailerRental
                stanfilcoTrip.TrailerRental = tariff.TrailerRent ? tariff.
                TrailerRental : 0;
                // Insurance
                stanfilcoTrip.Insurance = tariff.Insurance;
                // VATRate
                stanfilcoTrip.VATRate = tariff.TaxCode != null ? tariff.TaxCode.
                Rate : 0;
                // WHTRate
                stanfilcoTrip.WHTRate = tariff.WHTGroupCode != null ? tariff.
                WHTGroupCode.WHTRate : 0;
                stanfilcoTrip.Save();
                ObjectSpace.CommitChanges();
            } else {
                throw new ApplicationException("Tariff not found");
            }
        }
        private void UpdateStanFromTariffValues_Executed(object sender, 
        ActionBaseEventArgs e) {
            ObjectSpace.ReloadObject(stanfilcoTrip);
            ObjectSpace.Refresh();
        }
    }
}
