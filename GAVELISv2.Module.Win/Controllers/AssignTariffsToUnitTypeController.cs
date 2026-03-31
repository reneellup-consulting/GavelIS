using System;
using System.Linq;
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
    public partial class AssignTariffsToUnitTypeController : ViewController
    {
        private PopupWindowShowAction assignTariffsToUnitType;
        public AssignTariffsToUnitTypeController()
        {
            this.TargetObjectType = typeof(TruckUnitType);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "AssignTariffsToUnitType";
            this.assignTariffsToUnitType = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.assignTariffsToUnitType.Caption = "Assign Tariffs";
            this.assignTariffsToUnitType.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(AssignTariffsToUnitType_CustomizePopupWindowParams);
            this.assignTariffsToUnitType.Execute += new PopupWindowShowActionExecuteEventHandler(AssignTariffsToUnitType_Execute);
        }

        void AssignTariffsToUnitType_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            TruckUnitType tutype = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
            as TruckUnitType;
            TruckUnitType otutype = ObjectSpace.GetObject(tutype);
            foreach (var item in e.PopupWindow.View.SelectedObjects)
            {
                Tariff trf = ObjectSpace.GetObject(item) as Tariff;
                var data = otutype.UnitTypeFuelAllocations.Where(o => o.TariffID == trf).FirstOrDefault();
                if (data == null)
                {
                    var tfa = ObjectSpace.CreateObject<TariffFuelAllocation>();
                    tfa.TariffID = trf;
                    tfa.DistOneWay = trf.Distance;
                    tfa.DistRoundTrip = trf.Distance * 2;
                    tfa.LtrsPerKm = otutype.LtrsPerKm;
                    if (tfa.LtrsPerKm > 0)
                    {
                        tfa.FuelAllocation = tfa.DistRoundTrip / tfa.LtrsPerKm;
                    }
                    else
                    {
                        tfa.FuelAllocation = 0m;
                    }
                    tutype.UnitTypeFuelAllocations.Add(tfa);
                }
                else
                {
                    data.DistOneWay = trf.Distance;
                    data.DistRoundTrip = trf.Distance * 2;
                    data.LtrsPerKm = otutype.LtrsPerKm;
                    if (data.LtrsPerKm > 0)
                    {
                        data.FuelAllocation = data.DistRoundTrip / data.LtrsPerKm;
                    }
                    else
                    {
                        data.FuelAllocation = 0m;
                    }
                }
                otutype.Save();
            }
            ObjectSpace.CommitChanges();
        }

        void AssignTariffsToUnitType_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(Tariff));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(Tariff), listViewId)
            ;
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}
