using System;

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
    //[NonPersistent]
    public class VendorBalanceToDate : ReportParametersObjectBase
    {
        public VendorBalanceToDate(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            if (_AllVendors)
            {
                return CriteriaOperator.Parse("[DaysOt] > 0");

            }
            else
            {
                return CriteriaOperator.Parse("[DaysOt] > 0 And [Vendor] = ?", _Vendor); ;

            }
            //return CriteriaOperator.Parse("Vendor.No = ?", "V0001"); 
            //return null;
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            sorting.Add(new SortProperty("Vendor.No", SortingDirection.Ascending));
            return sorting;
        }
        private Vendor _Vendor;
        private bool _AllVendors;
        public bool AllVendors
        {
            get { return _AllVendors; }
            set { _AllVendors = value; }
        }
        public Vendor Vendor
        {
            get { return _Vendor; }
            set { _Vendor = value; }
        }

    }

}
