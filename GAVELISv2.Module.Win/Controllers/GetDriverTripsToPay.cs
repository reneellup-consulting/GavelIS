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
    public partial class GetDriverTripsToPay : ViewController
    {
        private PopupWindowShowAction getDriverTripsToPay;
        private DriverPayrollBatch _driverPayrollBatch;

        public GetDriverTripsToPay()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch.GetDriverTripsToPay";
            this.getDriverTripsToPay = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.getDriverTripsToPay.Caption = "Get Driver Trips";
            this.getDriverTripsToPay.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            GetDriverTripsToPay_CustomizePopupWindowParams);
            this.getDriverTripsToPay.Execute += new
            PopupWindowShowActionExecuteEventHandler(GetDriverTripsToPay_Execute
            );
            this.getDriverTripsToPay.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(getDriverTripsToPay_ExecuteCompleted);
        }

        void getDriverTripsToPay_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            this.ObjectSpace.CommitChanges();
        }
        private void GetDriverTripsToPay_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _driverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as DriverPayrollBatch;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            this.ObjectSpace.CommitChanges();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(DriverRegistry));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(DriverRegistry), listViewId)
            ;
            //if (_driverPayrollBatch.Customer == null)
            //{
            //    throw new
            //        ApplicationException("Customer not specified");
            //}

            //collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse("[TripID.TripCustomer.No] = '" + _driverPayrollBatch.Customer.No + "' And [Status] = 'Current'");

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void GetDriverTripsToPay_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (DriverRegistry item in e.PopupWindow.View.SelectedObjects){
                DriverRegistry dr = _driverPayrollBatch.Session.GetObjectByKey<DriverRegistry>(
                    item.Oid);
                DriverPayrollTrip driverTrip = ReflectionHelper.CreateObject<DriverPayrollTrip>(_driverPayrollBatch.Session);
                driverTrip.PayrollBatchID = _driverPayrollBatch;
                driverTrip.TripDate = dr.Date;
                driverTrip.TripNo = dr.TripID;
                driverTrip.DocumentNo = dr.ReferenceNo;
                driverTrip.RegID=dr;
                driverTrip.Driver = dr.Driver;
                // Tariff Class
                StringBuilder sb = new StringBuilder();
                bool hasError = false;
                sb.AppendFormat("Problems found in Driver Registry ID#{0}. ", item.Oid);
                if (item.Tariff == null)
                {
                    hasError = true;
                    sb.Append("Tariff is not specified and ");
                }
                if (item.Driver.DriverClassification == null)
                {
                    hasError = true;
                    sb.AppendFormat("Driver {0} Driver Classification is not specified     ", item.Driver.Name);
                }
                if (hasError)
                {
                    sb.Remove(sb.Length - 5, 5);
                    sb.Append(".");
                    throw new ApplicationException(sb.ToString());
                }
                //dr.DriverClass = dr.Driver.DriverClassification;
                TariffDriversClassifier trfclass = _driverPayrollBatch.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse(string.Format("[TariffID.Code] = '{0}' And [DriverClass.Code] = '{1}'", item.Tariff.Code, item.Driver.DriverClassification.Code)));
                //
                if (trfclass == null)
                {
                    throw new ApplicationException(string.Format("Tariff driver classifiers has not been set up for Tariff #{0}. Please check", item.Tariff.Code));
                }
                driverTrip.Basic = trfclass.BaseShare * (trfclass.ShareRate / 100);
                driverTrip.AdlMiscExp = trfclass.BaseShare - driverTrip.Basic;

                if (dr.TripID.GetType() == typeof(StanfilcoTrip))
                {
                    driverTrip.MiscExp = ((StanfilcoTrip)dr.TripID).Allowance;
                }
                if (dr.TripID.GetType() == typeof(DolefilTrip))
                {
                    driverTrip.MiscExp = ((DolefilTrip)dr.TripID).Allowance.Value;
                }
                if (dr.TripID.GetType() == typeof(OtherTrip))
                {
                    driverTrip.MiscExp = ((OtherTrip)dr.TripID).Allowance;
                }
                if (driverTrip.MiscExp == 0)
                {
                    driverTrip.MiscExp = trfclass.TariffID.Allowance;
                }
                ICollection kds;
                SortingCollection sorts = new SortingCollection(null);
                DevExpress.Xpo.Metadata.XPClassInfo kdsClassInfo = _driverPayrollBatch.Session.GetClassInfo(typeof(KDEntry)); ;
                kds = _driverPayrollBatch.Session.GetObjects(kdsClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + item.TripID.SourceNo + "'"), sorts, 0, false, true);
                foreach (KDEntry kd in kds)
                {
                    driverTrip.KDs = driverTrip.KDs + trfclass.KDShare;
                }

                ICollection shunts;
                DevExpress.Xpo.Metadata.XPClassInfo shuntingClassInfo = _driverPayrollBatch.Session.GetClassInfo(typeof(ShuntingEntry));
                shunts = _driverPayrollBatch.Session.GetObjects(shuntingClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + item.TripID.SourceNo + "'"), sorts, 0, false, true);
                foreach (ShuntingEntry sh in shunts)
                {
                    driverTrip.Shunting = driverTrip.Shunting + trfclass.ShuntingShare;
                }
                if (item.TripID.GetType() == typeof(StanfilcoTrip))
                {
                    driverTrip.ShuntingTo = (item.TripID as StanfilcoTrip).Origin.Code;
                }
                if (item.TripID.GetType() == typeof(DolefilTrip))
                {
                    driverTrip.ShuntingTo = (item.TripID as DolefilTrip).Origin.Code;
                }
                if (item.TripID.GetType() == typeof(OtherTrip))
                {
                    driverTrip.ShuntingTo = (item.TripID as OtherTrip).Origin.Code;
                }
                dr.Status=DriverRegistryStatusEnum.Processed;
                dr.Save();
                driverTrip.Save();

            }
        }
    }
}
