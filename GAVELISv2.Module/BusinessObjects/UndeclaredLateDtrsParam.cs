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
    public class UndeclaredLateDtrsParam : ReportParametersObjectBase
    {
        public UndeclaredLateDtrsParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            if (!_AllCustomer && _TripCustomer == null)
            {
                throw new UserFriendlyException("Customer not yet provided");
            }
            if (_Start == DateTime.MinValue || _End == DateTime.MinValue)
            {
                throw new UserFriendlyException("Start and End Date cannot be empty");
            }
            if (_Start > _End)
            {
                throw new UserFriendlyException("Start Date cannot be greater than the End Date");
            }
            StringBuilder sbCrit = new StringBuilder();
            if (!_AllCustomer)
            {
                sbCrit.AppendFormat("[TripID.TripCustomer.No] = '{0}' And ", TripCustomer.No);
            }
            sbCrit.AppendFormat("[ExactEntryDate] >= #{0}# And [ExactEntryDateEnd] <= #{1}# And ", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));

            if (_Driver != null)
            {
                sbCrit.AppendFormat("[Driver.Code] = '{0}'", _Driver.No);
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

        private Employee _Driver;
        private DateTime _Start;
        private DateTime _End;
        private Customer _TripCustomer;
        private bool _AllCustomer;
        public bool AllCustomer
        {
            get
            {
                return _AllCustomer;
            }
            set
            {
                if (_AllCustomer == value)
                    return;
                _AllCustomer = value;
            }
        }
        public Customer TripCustomer
        {
            get
            {
                return _TripCustomer;
            }
            set
            {
                if (_TripCustomer == value)
                    return;
                _TripCustomer = value;
            }
        }
        public Employee Driver
        {
            get
            {
                return _Driver;
            }
            set
            {
                if (_Driver == value)
                    return;
                _Driver = value;
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
