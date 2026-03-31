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
    public partial class CalculatePartialPayController : ViewController
    {
        private DriverRegistry _DriverRegis;
        public CalculatePartialPayController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void CalculatePartialPayAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (this.View.GetType() == typeof(ListView))
            {
                var selected = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
                foreach (DriverRegistry dr in selected)
                {
                    if (dr.TripID.GetType() == typeof(DolefilTrip))
                    {
                        continue;
                    }
                    DriverRegistry odr = dr.Session.GetObjectByKey<DriverRegistry>(dr.Oid);
                    StringBuilder sb = new StringBuilder();
                    bool hasError = false;
                    sb.AppendFormat("Problems found in Driver Registry ID#{0}. ", odr.Oid);
                    if (odr.Tariff == null)
                    {
                        hasError = true;
                        sb.Append("Tariff is not specified and ");
                    }
                    if (odr.Driver.DriverClassification == null)
                    {
                        hasError = true;
                        sb.AppendFormat("Driver {0} Driver Classification is not specified     ", odr.Driver.Name);
                    }
                    if (hasError)
                    {
                        sb.Remove(sb.Length - 5, 5);
                        sb.Append(".");
                        throw new ApplicationException(sb.ToString());
                    }

                    // Start: To make the parent trip tariff to be the default
                    string tariffCode = string.Empty;
                    if (odr.TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        tariffCode = ((StanfilcoTrip)odr.TripID).Tariff.Code;
                    }
                    if (odr.TripID.GetType() == typeof(DolefilTrip))
                    {
                        tariffCode = ((DolefilTrip)odr.TripID).Tariff.Code;
                    }
                    if (odr.TripID.GetType() == typeof(OtherTrip))
                    {
                        tariffCode = ((OtherTrip)odr.TripID).Tariff.Code;
                    }
                    // End: To make the parent trip tariff to be the default

                    //odr.DriverClass = dr.Driver.DriverClassification;
                    TariffDriversClassifier trfclass = dr.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse(string.Format("[TariffID.Code] = '{0}' And [DriverClass.Code] = '{1}'", tariffCode, odr.Driver.DriverClassification.Code)));
                    //
                    if (trfclass == null)
                    {
                        throw new ApplicationException(string.Format("Tariff driver classifiers {0} for {1} has not been set up for Tariff #{2}. Please check", odr.Driver.DriverClassification.Code, odr.Driver.Name, tariffCode));
                    }
                    odr.CppBasic = trfclass.BaseShare * (trfclass.ShareRate / 100);
                    odr.CppAdlMiscExp = trfclass.BaseShare - odr.CppBasic;

                    if (odr.TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        odr.CppMiscExp = ((StanfilcoTrip)odr.TripID).Allowance;
                    }
                    if (odr.TripID.GetType() == typeof(DolefilTrip))
                    {
                        odr.CppMiscExp = ((DolefilTrip)odr.TripID).Allowance.Value;
                    }
                    if (odr.TripID.GetType() == typeof(OtherTrip))
                    {
                        odr.CppMiscExp = ((OtherTrip)odr.TripID).Allowance;
                    }
                    if (odr.CppMiscExp == 0)
                    {
                        odr.CppMiscExp = trfclass.TariffID.Allowance;
                    }
                    ICollection kds;
                    SortingCollection sorts = new SortingCollection(null);
                    DevExpress.Xpo.Metadata.XPClassInfo kdsClassInfo = dr.Session.GetClassInfo(typeof(KDEntry)); ;
                    kds = dr.Session.GetObjects(kdsClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + odr.TripID.SourceNo + "'"), sorts, 0, false, true);
                    decimal k = 0m;
                    foreach (KDEntry kd in kds)
                    {
                        k = k + trfclass.KDShare;
                    }
                    odr.CppKDs = k;
                    ICollection shunts;
                    DevExpress.Xpo.Metadata.XPClassInfo shuntingClassInfo = dr.Session.GetClassInfo(typeof(ShuntingEntry));
                    shunts = dr.Session.GetObjects(shuntingClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + odr.TripID.SourceNo + "'"), sorts, 0, false, true);
                    decimal s = 0m;
                    foreach (ShuntingEntry sh in shunts)
                    {
                        s = s + trfclass.ShuntingShare;
                    }
                    odr.CppShunting = s;
                    odr.Save();
                    dr.Session.CommitTransaction();
                }
            }
            else if (this.View.GetType() == typeof(DetailView))
            {
                _DriverRegis = ((DevExpress.ExpressApp.DetailView)this.View
                ).CurrentObject as DriverRegistry;
                if (_DriverRegis.TripID.GetType()==typeof(DolefilTrip))
                {
                    throw new UserFriendlyException("Dolefil Trips cannot be process!");
                }
                DriverRegistry odr = _DriverRegis.Session.GetObjectByKey<DriverRegistry>(_DriverRegis.Oid);
                StringBuilder sb = new StringBuilder();
                bool hasError = false;
                sb.AppendFormat("Problems found in Driver Registry ID#{0}. ", odr.Oid);
                if (odr.Tariff == null)
                {
                    hasError = true;
                    sb.Append("Tariff is not specified and ");
                }
                if (odr.Driver.DriverClassification == null)
                {
                    hasError = true;
                    sb.AppendFormat("Driver {0} Driver Classification is not specified     ", odr.Driver.Name);
                }
                if (hasError)
                {
                    sb.Remove(sb.Length - 5, 5);
                    sb.Append(".");
                    throw new ApplicationException(sb.ToString());
                }

                // Start: To make the parent trip tariff to be the default
                string tariffCode = string.Empty;
                if (odr.TripID.GetType() == typeof(StanfilcoTrip))
                {
                    tariffCode = ((StanfilcoTrip)odr.TripID).Tariff.Code;
                }
                if (odr.TripID.GetType() == typeof(DolefilTrip))
                {
                    tariffCode = ((DolefilTrip)odr.TripID).Tariff.Code;
                }
                if (odr.TripID.GetType() == typeof(OtherTrip))
                {
                    tariffCode = ((OtherTrip)odr.TripID).Tariff.Code;
                }
                // End: To make the parent trip tariff to be the default

                //odr.DriverClass = odr.Driver.DriverClassification;
                TariffDriversClassifier trfclass = _DriverRegis.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse(string.Format("[TariffID.Code] = '{0}' And [DriverClass.Code] = '{1}'", tariffCode, odr.Driver.DriverClassification.Code)));
                //
                if (trfclass == null)
                {
                    throw new ApplicationException(string.Format("Tariff driver classifiers has not been set up for Tariff #{0}. Please check", tariffCode));
                }
                odr.CppBasic = trfclass.BaseShare * (trfclass.ShareRate / 100);
                odr.CppAdlMiscExp = trfclass.BaseShare - odr.CppBasic;

                if (odr.TripID.GetType() == typeof(StanfilcoTrip))
                {
                    odr.CppMiscExp = ((StanfilcoTrip)odr.TripID).Allowance;
                }
                if (odr.TripID.GetType() == typeof(DolefilTrip))
                {
                    odr.CppMiscExp = ((DolefilTrip)odr.TripID).Allowance.Value;
                }
                if (odr.TripID.GetType() == typeof(OtherTrip))
                {
                    odr.CppMiscExp = ((OtherTrip)odr.TripID).Allowance;
                }
                if (odr.CppMiscExp == 0)
                {
                    odr.CppMiscExp = trfclass.TariffID.Allowance;
                }
                ICollection kds;
                SortingCollection sorts = new SortingCollection(null);
                DevExpress.Xpo.Metadata.XPClassInfo kdsClassInfo = _DriverRegis.Session.GetClassInfo(typeof(KDEntry)); ;
                kds = _DriverRegis.Session.GetObjects(kdsClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + odr.TripID.SourceNo + "'"), sorts, 0, false, true);
                decimal k = 0m;
                foreach (KDEntry kd in kds)
                {
                    k = k + trfclass.KDShare;
                }
                odr.CppKDs = k;
                ICollection shunts;
                DevExpress.Xpo.Metadata.XPClassInfo shuntingClassInfo = _DriverRegis.Session.GetClassInfo(typeof(ShuntingEntry));
                shunts = _DriverRegis.Session.GetObjects(shuntingClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + odr.TripID.SourceNo + "'"), sorts, 0, false, true);
                decimal s = 0m;
                foreach (ShuntingEntry sh in shunts)
                {
                    s = s + trfclass.ShuntingShare;
                }
                odr.CppShunting = s;
                odr.Save();
                _DriverRegis.Session.CommitTransaction();
            }
        }
    }
}
