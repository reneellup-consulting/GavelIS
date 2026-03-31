using System;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Reports;

namespace GAVELISv2.Module.BusinessObjects
{
    public class InventoryReportActualParam : ReportParametersObjectBase
    {
        public InventoryReportActualParam(Session session)
            : base(session)
        {
        }
        public override CriteriaOperator GetCriteria()
        {
            if (_WarehouseLocation == null)
            {
                throw new UserFriendlyException("Warehouse location not yet provided");
            }

            StringBuilder sbCrit = new StringBuilder();
            sbCrit.AppendFormat("[Warehouse.Code] = '{0}'", _WarehouseLocation.Code);
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");

        }

        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private Warehouse _WarehouseLocation;
        public Warehouse WarehouseLocation
        {
            get { return _WarehouseLocation; }
            set { SetPropertyValue<Warehouse>("WarehouseLocation", ref _WarehouseLocation, value); }
        }
    }

}
