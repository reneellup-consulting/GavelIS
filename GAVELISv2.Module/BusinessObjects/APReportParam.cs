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
    //[NonPersistent]
    public class APReportParam : ReportParametersObjectBase
    {
        public APReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            //if (_Start == DateTime.MinValue || _End == DateTime.MinValue)
            //{
            //    throw new UserFriendlyException("Start and End Date cannot be empty");
            //}
            //if (_Start > _End)
            //{
            //    throw new UserFriendlyException("Start Date cannot be greater than the End Date");
            //}

            if (!_All && _Vendor == null)
            {
                throw new UserFriendlyException("Vendor not yet provided");
            }
            //[AmtRmn] <> 0.0m And [Status] <> 'Current'
            //[ExactEntryDate] >= #2014-10-01# And [ExactEntryDate] <= #2014-10-30# And [Status] <> 'Current' And [Customer.Code] = 'c003080'

            StringBuilder sbCrit = new StringBuilder();
            // Date Range
            //sbCrit.AppendFormat("[AmtRmn] <> 0.0m And [Status] <> 'Current' And [ExactEntryDate] >= #{0}# And [ExactEntryDate] <= #{1}# And ", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));
            if (_End==DateTime.MinValue)
            {
                sbCrit.AppendFormat("Not [Status] In ('Current', 'Paid') And ");
            } else
                sbCrit.AppendFormat("Not [Status] In ('Current', 'Paid') And [ExactEntryDate] >= #{0}# And [ExactEntryDateEnd] <= #{1}# And ", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));

            // if only invoiced Status
            //if (_InvoicedOnly)
            //{
            //    sbCrit.Append("[Status] <> 'Current' And ");
            //}
            // if is not all customer
            if (!_All)
            {
                sbCrit.AppendFormat("[Vendor.Code] = '{0}'", _Vendor.No);
            }
            else
            {
                sbCrit.Remove(sbCrit.Length - 5, 5);
            }
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");

        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private bool _All = true;
        private bool _ReceivedOnly = false;
        private Vendor _Vendor;
        private DateTime _Start;
        private DateTime _End;

        public bool All
        {
            get
            {
                return _All;
            }
            set
            {
                if (_All == value)
                    return;
                _All = value;
            }
        }
        public bool ReceivedOnly
        {
            get
            {
                return _ReceivedOnly;
            }
            set
            {
                if (_ReceivedOnly == value)
                    return;
                _ReceivedOnly = value;
            }
        }

        public Vendor Vendor
        {
            get
            {
                return _Vendor;
            }
            set
            {
                if (_Vendor == value)
                    return;
                _Vendor = value;
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
