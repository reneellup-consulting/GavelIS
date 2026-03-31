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
    public class ReceiptDetailParam : ReportParametersObjectBase
    {
        private DateTime _Start;
        private DateTime _End;
        private PartsOrigin _Origin;
        private bool _AllOrigin = false;
        public DateTime Start
        {
            get { return _Start; }
            set
            {
                if (_Start == value)
                    return;
                _Start = value;
            }
        }

        public DateTime End
        {
            get { return _End; }
            set
            {
                if (_End == value)
                    return;
                _End = value;
            }
        }

        public PartsOrigin Origin
        {
            get { return _Origin; }
            set
            {
                if (_Origin == value)
                    return;
                _Origin = value;
            }
        }
        public bool AllOrigin
        {
            get { return _AllOrigin; }
            set
            {
                if (_AllOrigin == value)
                    return;
                _AllOrigin = value;
            }
        }
        public string Title
        {
            get
            {
                string t = "ITEMS RECEIVED FROM SUPPLIERS";
                if (!_AllOrigin && _Origin != null)
                {
                    t = string.Format("ITEMS RECEIVED FROM {0} SUPPLIERS", _Origin.Name.ToUpper());
                }
                else if (!_AllOrigin && _Origin == null)
                {
                    t = string.Format("ITEMS RECEIVED FROM LOCAL SUPPLIERS");
                }
                return t;
            }
        }
        public ReceiptDetailParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            if (_Start == DateTime.MinValue || _End == DateTime.MinValue)
            {
                throw new UserFriendlyException("Start and End Date cannot be empty");
            }
            if (_Start > _End)
            {
                throw new UserFriendlyException("Start Date cannot be greater than the End Date");
            }
            StringBuilder sbCrit = new StringBuilder();
            // [Date Issued] >= #2018-03-13# And [Date Issued] < #2018-03-14#
            if (!_AllOrigin && _Origin != null)
            {
                sbCrit.AppendFormat("[GenJournalID.ExactEntryDate] >= #{0}# And [GenJournalID.ExactEntryDateEnd] <= #{1}# And [ReceiptInfo.Vendor.Origin.Name] = '{2}'", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"), Origin.Name);
            }
            else if (!_AllOrigin && _Origin == null)
            {
                sbCrit.AppendFormat("[GenJournalID.ExactEntryDate] >= #{0}# And [GenJournalID.ExactEntryDateEnd] <= #{1}# And [ReceiptInfo.Vendor.Origin] Is Null", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));
            }
            else if (_AllOrigin)
            {
                sbCrit.AppendFormat("[GenJournalID.ExactEntryDate] >= #{0}# And [GenJournalID.ExactEntryDateEnd] <= #{1}#", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));
            }
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
    }

}
