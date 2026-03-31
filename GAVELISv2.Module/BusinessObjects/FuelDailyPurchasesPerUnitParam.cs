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
    public class FuelDailyPurchasesPerUnitParam : ReportParametersObjectBase
    {
        public FuelDailyPurchasesPerUnitParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            // [Status] <> 'Disapproved' And [Entry Date] >= #2017-02-01# And [Entry Date] <= #2017-02-28#
            if (_Start == DateTime.MinValue || _End == DateTime.MinValue)
            {
                throw new UserFriendlyException("Start and End Date cannot be empty");
            }
            if (_Start > _End)
            {
                throw new UserFriendlyException("Start Date cannot be greater than the End Date");
            }

            //[ExactEntryDate] >= #2014-10-01# And [ExactEntryDate] <= #2014-10-30# And [Status] <> 'Current' And [Customer.Code] = 'c003080'

            StringBuilder sbCrit = new StringBuilder();
            // Date Range
            // [Purchase Info.Exact Entry Date] Between(#2017-08-01#, #2017-08-02#) And [Item No.Item Type] = 'Fuel Item'
            sbCrit.AppendFormat("[PurchaseInfo.ExactEntryDate] >= #{0}# And [PurchaseInfo.ExactEntryDateEnd] <= #{1}# And [ItemNo.ItemType] = 'FuelItem'", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));
            if (_Unit != null)
            {
                sbCrit.AppendFormat(" And [CostCenter.Code] = '{0}'", _Unit.Code);
            }
            // if only invoiced Status
            //if (_InvoicedOnly)
            //{
            //    sbCrit.Append("[Status] <> 'Current' And ");
            //}
            // if is not all customer
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
        private DateTime _Start = DateTime.Today;
        private DateTime _End = DateTime.Today;
        private CostCenter _Unit;

        public CostCenter Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                if (_Unit == value)
                    return;
                _Unit = value;
            }
        }
        public DateTime Start
        {
            get
            {
                return _Start;
            }
            set
            {
                if (_Start == value)
                    return;
                _Start = value;
                _End = value;
            }
        }
        public DateTime End
        {
            get
            {
                return _End;
            }
            set
            {
                if (_End == value)
                    return;
                _End = value;
            }
        }
    }

}
