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
    public partial class ShowDriverPayroll3TripDetailsIncController : ViewController
    {
        private PopupWindowShowAction showDriverPayroll3TripDetailsInc;
        private DriverPayroll3 _driverPayroll3;

        public ShowDriverPayroll3TripDetailsIncController()
        {
            this.TargetObjectType = typeof(DriverPayroll3);
            this.TargetViewType = ViewType.Any;
            string actionID = "DriverPayroll3.ShowDriverPayroll3TripDetailsInc";
            this.showDriverPayroll3TripDetailsInc = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.showDriverPayroll3TripDetailsInc.Caption = "Show Details Included";
            this.showDriverPayroll3TripDetailsInc.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            showDriverPayroll3TripDetailsInc_CustomizePopupWindowParams);
        }

        private void showDriverPayroll3TripDetailsInc_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _driverPayroll3 = this.View.CurrentObject as DriverPayroll3;
            this.ObjectSpace.CommitChanges();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "DolefilTripDetail_ShowDriverPayroll3TripDetailsInc";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(DolefilTripDetail), listViewId)
            ;
            string crit = string.Format("[Dprl3Id]={0}", _driverPayroll3.Oid);
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(crit);

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}
