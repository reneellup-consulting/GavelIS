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
    public partial class AddTripsToDriverPayroll2Controller : ViewController
    {
        private PopupWindowShowAction addTrips;
        private DriverPayroll2 _driverPayroll2;

        public AddTripsToDriverPayroll2Controller()
        {
            this.TargetObjectType = typeof(DriverPayroll2);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayroll2.AddTrips";
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
            _driverPayroll2 = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as DriverPayroll2;
            this.ObjectSpace.CommitChanges();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "DriverRegistry_ListView_AddTrips";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(DriverRegistry), listViewId)
            ;
            string crit = string.Empty;
            if (!string.IsNullOrEmpty((_driverPayroll2.PayrollBatchID as DriverPayrollBatch2).BatchType.RegistryFilter))
            {
                crit = string.Format("Not [Status] In ('Processed', 'Paid') And [Driver.No] = '{0}' And {1}", _driverPayroll2.Employee.No, (_driverPayroll2.PayrollBatchID as DriverPayrollBatch2).BatchType.RegistryFilter);
            }
            else
            {
                crit = string.Format("Not [Status] In ('Processed', 'Paid') And [Driver.No] = '{0}'", _driverPayroll2.Employee.No);
            }
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(crit);

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void AddTrips_Execute(object sender,PopupWindowShowActionExecuteEventArgs e)
        {
            DriverPayrollBatch2 dpb2 = _driverPayroll2.Session.GetObjectByKey<DriverPayrollBatch2>(
                _driverPayroll2.PayrollBatchID.Oid);
            foreach (DriverRegistry item in e.PopupWindow.View.SelectedObjects)
            {
                DriverRegistry dr = _driverPayroll2.Session.GetObjectByKey<DriverRegistry>(
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
                if (_driverPayroll2.BatchInfo.BatchType.TaggedFuelRequired && string.IsNullOrEmpty(dr.TripID.TaggedFuelReceipts))
                {
                    hasError = true;
                    sb.AppendFormat("The trip {0} has no tagged fuel receipts     ", dr.TripNo);
                }
                if (hasError)
                {
                    sb.Remove(sb.Length - 5, 5);
                    sb.Append(".");
                    throw new ApplicationException(sb.ToString());
                }

                // Start: To make the parent trip tariff to be the default
                string tariffCode = string.Empty;
                if (dr.TripID.GetType() == typeof(StanfilcoTrip))
                {
                    tariffCode = ((StanfilcoTrip)dr.TripID).Tariff.Code;
                }
                if (dr.TripID.GetType() == typeof(DolefilTrip))
                {
                    tariffCode = ((DolefilTrip)dr.TripID).Tariff.Code;
                }
                if (dr.TripID.GetType() == typeof(OtherTrip))
                {
                    tariffCode = ((OtherTrip)dr.TripID).Tariff.Code;
                }
                // End: To make the parent trip tariff to be the default

                TariffDriversClassifier trfclass = _driverPayroll2.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse(string.Format("[TariffID.Code] = '{0}' And [DriverClass.Code] = '{1}'", tariffCode, dr.Driver.DriverClassification.Code)));
                if (trfclass == null)
                {
                    throw new ApplicationException(string.Format("Tariff driver classifiers has not been set up for Tariff #{0}. Please check", dr.Tariff.Code));
                }
                dr.CppBasic = trfclass.BaseShare * (trfclass.ShareRate / 100);
                dr.CppAdlMiscExp = trfclass.BaseShare - dr.CppBasic;

                if (dr.TripID.GetType() == typeof(StanfilcoTrip))
                {
                    dr.CppMiscExp = ((StanfilcoTrip)dr.TripID).Allowance;
                }
                if (dr.TripID.GetType() == typeof(DolefilTrip))
                {
                    dr.CppMiscExp = ((DolefilTrip)dr.TripID).Allowance.Value;
                }
                if (dr.TripID.GetType() == typeof(OtherTrip))
                {
                    dr.CppMiscExp = ((OtherTrip)dr.TripID).Allowance;
                }
                if (dr.CppMiscExp == 0)
                {
                    dr.CppMiscExp = trfclass.TariffID.Allowance;
                }
                ICollection kds;
                SortingCollection sorts = new SortingCollection(null);
                DevExpress.Xpo.Metadata.XPClassInfo kdsClassInfo = _driverPayroll2.Session.GetClassInfo(typeof(KDEntry)); ;
                kds = _driverPayroll2.Session.GetObjects(kdsClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + dr.TripID.SourceNo + "'"), sorts, 0, false, true);
                decimal k = 0m;
                foreach (KDEntry kd in kds)
                {
                    k = k + trfclass.KDShare;
                }
                dr.CppKDs = k;
                ICollection shunts;
                DevExpress.Xpo.Metadata.XPClassInfo shuntingClassInfo = _driverPayroll2.Session.GetClassInfo(typeof(ShuntingEntry));
                shunts = _driverPayroll2.Session.GetObjects(shuntingClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + dr.TripID.SourceNo + "'"), sorts, 0, false, true);
                decimal s = 0m;
                foreach (ShuntingEntry sh in shunts)
                {
                    s = s + trfclass.ShuntingShare;
                }
                dr.CppShunting = s;
                dr.Status = DriverRegistryStatusEnum.Processed;
                dr.PayrollBatchID = dpb2;
                dr.Save();

                // Calculate PartialPay End

                #region Trip processing here...

                DriverPayrollTripLine2 dptl2 = _driverPayroll2.DriverPayrollTripLines.Where(o => o.DriverRegistryId == dr).FirstOrDefault();
                if (dptl2 == null)
                {
                    dptl2 = ReflectionHelper.CreateObject<DriverPayrollTripLine2>(_driverPayroll2.Session);
                    dptl2.DriverPayrollID = _driverPayroll2;
                    dptl2.DriverRegistryId = dr;
                }

                dptl2.TripDate = dr.Date;
                dptl2.DocumentNo = dr.ReferenceNo;
                dptl2.Driver = dr.Driver;
                dptl2.Commission = dr.TripCommission;
                dptl2.KDs = dr.Kds ?? 0;
                dptl2.Shunting = dr.Shunting ?? 0;
                dptl2.Include = true;
                dptl2.Save();

                #endregion
            }
        }
    }
}
