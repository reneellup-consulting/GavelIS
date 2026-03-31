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

namespace GAVELISv2.Module.BusinessObjects
{
    public partial class AddTripsToDriverPayroll3Controller : ViewController
    {
        private PopupWindowShowAction addTrips;
        private DriverPayroll3 _driverPayroll3;

        public AddTripsToDriverPayroll3Controller()
        {
            this.TargetObjectType = typeof(DriverPayroll3);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayroll3.AddTrips";
            this.addTrips = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.addTrips.Caption = "Add Trips";
            this.addTrips.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            AddTrips_CustomizePopupWindowParams);
            this.addTrips.Execute += new
            PopupWindowShowActionExecuteEventHandler(AddTrips_Execute
            );
            this.addTrips.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(AddTrips_ExecuteCompleted);
        }

        void AddTrips_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            this.ObjectSpace.CommitChanges();
        }

        private void AddTrips_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _driverPayroll3 = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as DriverPayroll3;
            this.ObjectSpace.CommitChanges();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "DriverRegistry_ListView_AddTrips3";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(DriverRegistry), listViewId)
            ;
            string crit = string.Empty;
            if (!string.IsNullOrEmpty((_driverPayroll3.PayrollBatchID as DriverPayrollBatch3).BatchType.RegistryFilter))
            {
                crit = string.Format("Not [Status] In ('Processed', 'Paid') And [Driver.No] = '{0}' And {1}", _driverPayroll3.Employee.No, (_driverPayroll3.PayrollBatchID as DriverPayrollBatch3).BatchType.RegistryFilter);
            }
            else
            {
                crit = string.Format("Not [Status] In ('Processed', 'Paid') And [Driver.No] = '{0}'", _driverPayroll3.Employee.No);
            }
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(crit);

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }

        private void AddTrips_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DriverPayrollBatch3 dpb2 = _driverPayroll3.Session.GetObjectByKey<DriverPayrollBatch3>(
                _driverPayroll3.PayrollBatchID.Oid);
            foreach (DriverRegistry item in e.PopupWindow.View.SelectedObjects)
            {
                DriverRegistry dr = _driverPayroll3.Session.GetObjectByKey<DriverRegistry>(
                    item.Oid);
                StringBuilder sb = new StringBuilder();
                bool hasError = false;
                sb.AppendFormat("Problems found in Driver Registry ID#{0}. ", dr.Oid);
                if (dr.Tariff == null)
                {
                    hasError = true;
                    sb.Append("Tariff is not specified and ");
                }
                if (dr.Driver.DriverClassification == null)
                {
                    hasError = true;
                    sb.AppendFormat("Driver {0} Driver Classification is not specified     ", dr.Driver.Name);
                }
                if (hasError)
                {
                    sb.Remove(sb.Length - 5, 5);
                    sb.Append(".");
                    throw new ApplicationException(sb.ToString());
                }
                dr.DolePayroll = true;
                dr.Status = DriverRegistryStatusEnum.Processed;
                dr.PayrollBatchID3 = dpb2;
                dr.Save();

                DriverPayroll3 dpr = dpb2.DriverPayrolls3.Where(o => o.Employee == dr.Driver).FirstOrDefault();
                if (dpr == null)
                {
                    dpr = ReflectionHelper.CreateObject<DriverPayroll3>(dpr.Session);
                    dpr.PayrollBatchID = dpb2;
                }

                dpr.Employee = dr.Driver;

                #region Trip processing here...
                foreach (DolefilTripDetail dftd in dr.TripID.DolefilTripDetails)
                {
                    DriverPayrollTripLine3 dptl3 = dpr.DriverPayrollTripLines.Where(o => o.Driver == dpr.Employee && o.OriginDestination == dftd.CommRoute).FirstOrDefault();
                    if (dptl3 != null)
                    {
                        dptl3.Reprocessing = true;
                    }
                    if (dptl3 == null)
                    {
                        dptl3 = ReflectionHelper.CreateObject<DriverPayrollTripLine3>(dpr.Session);
                        dptl3.DriverPayrollID = dpr;
                        dptl3.DriverRegistryId = dr;
                        dptl3.TripId = dr.TripID as DolefilTrip;
                        dptl3.Driver = dr.Driver;
                        dptl3.OriginDestination = dftd.CommRoute;
                        dptl3.Category = dftd.Category;
                        dptl3.Manual = false;
                    }
                    if (!dptl3.Altered)
                    {
                        dptl3.NoOfTrips += dftd.CommCount;
                        dptl3.Commission += dftd.Commission;
                    }
                    dptl3.Reprocessing = false;
                    dptl3.Save();
                    dftd.Dptl3Id = dptl3;
                    dftd.Dprl3Id = dpr;
                    dftd.DriverRegistryId = dr;
                    dftd.Save();
                }

                foreach (DriverPayrollTripLine3 dptl in dpr.DriverPayrollTripLines)
                {
                    dptl.Reprocessing = true;
                    //if ((dptl.NoOfTrips - Math.Truncate(dptl.NoOfTrips)) == 0.50m)
                    //{
                    //    var gcomm = dptl.Commission / dptl.NoOfTrips;
                    //    // According to LVG, she allow .5 for a reason
                    //    //dptl.NoOfTrips = Math.Truncate(dptl.NoOfTrips) + 0.50m;
                    //    dptl.NoOfTrips = Math.Truncate(dptl.NoOfTrips);
                    //    dptl.Commission = gcomm * dptl.NoOfTrips;
                    //    dptl.Save();
                    //}
                    dptl.Reprocessing = false;
                    dptl.Save();
                }
                #endregion

                dpb2.Processed = true;
                dpb2.Save();
            }
        }
    }
}
