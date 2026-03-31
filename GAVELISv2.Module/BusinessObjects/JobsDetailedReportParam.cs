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
    public class JobsDetailedReportParam : ReportParametersObjectBase
    {
        public JobsDetailedReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            if (!_All && _Vendor == null)
            {
                throw new UserFriendlyException("Vendor not yet provided");
            }
            //[Status] In ('Current', 'Confirmed', 'Partially Paid', 'Paid') And [Exact Entry Date] >= #2014-12-01# And [Exact Entry Date] <= #2014-12-31# And [Vendor.Code] = 'rwer'
            StringBuilder sbCrit = new StringBuilder();
            if (_End == DateTime.MinValue)
            {
                sbCrit.AppendFormat("");
            }
            else
                sbCrit.AppendFormat("[ExactEntryDate] >= #{0}# And [ExactEntryDateEnd] <= #{1}# And ", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));

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
