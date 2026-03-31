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
    public class WorkOrderDetailParam : ReportParametersObjectBase {
        public WorkOrderDetailParam(Session session)
            : base(session) {
        }

        public override CriteriaOperator GetCriteria() {
            if (_Start == DateTime.MinValue || _End == DateTime.MinValue)
            {
                throw new UserFriendlyException("Start and End Date cannot be empty");
            }
            if (_Start > _End)
            {
                throw new UserFriendlyException("Start Date cannot be greater than the End Date");
            }
            //if (_ItemClass==ItemClassEnum.None && _Fleet == null)
            //{
            //    throw new UserFriendlyException("Fleet must be specified");
            //}
            //[ExactEntryDate] >= #2014-10-01# And [ExactEntryDate] <= #2014-10-30# And [Status] <> 'Current' And [Customer.Code] = 'c003080'
            // [Item No.Item Class] = 'Mechanical' And [Work Order Info.Fleet] = '18-01->Genset #18-01'

            StringBuilder sbCrit = new StringBuilder();
            // Date Range
            if (_ItemClass != ItemClassEnum.None)
            {
                sbCrit.AppendFormat("[ItemNo.ItemClass] = '{0}' And ", _ItemClass);
            }
            if (_CostCenter != null)
            {
                sbCrit.AppendFormat("[CostCenter.Code] = '{0}' And ", _CostCenter.Code);
            }
            if (_Fleet != null)
            {
                sbCrit.AppendFormat("[WorkOrderInfo.Fleet.No] = '{0}' And ", Fleet.No);
            }

            if (_FacilityCode != null)
            {
                sbCrit.AppendFormat("[WorkOrderInfo.Facility.Code] = '{0}' And ", FacilityCode.Code);
            }

            sbCrit.AppendFormat("[WorkOrderInfo.ExactEntryDate] >= #{0}# And [WorkOrderInfo.ExactEntryDateEnd] <= #{1}#", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));
            // if only invoiced Status
            //if (_InvoicedOnly)
            //{
            //    sbCrit.Append("[Status] <> 'Current' And ");
            //}
            // if is not all customer
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }

        public override SortingCollection GetSorting() {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private FixedAsset _Fleet;
        private ItemClassEnum _ItemClass;
        private Facility _FacilityCode;
        private CostCenter _CostCenter;
        private DateTime _Start;
        private DateTime _End;
        public FixedAsset Fleet {
            get { return _Fleet; }
            set
            {
                if (_Fleet == value)
                    return;
                _Fleet = value;
            }
        }

        public ItemClassEnum ItemClass {
            get { return _ItemClass; }
            set
            {
                if (_ItemClass == value)
                    return;
                _ItemClass = value;
            }
        }

        public Facility FacilityCode {
            get { return _FacilityCode; }
            set
            {
                if (_FacilityCode == value)
                    return;
                _FacilityCode = value;
            }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value); }
        }

        public DateTime Start {
            get { return _Start; }
            set
            {
                if (_Start == value)
                    return;
                _Start = value;
            }
        }

        public DateTime End {
            get { return _End; }
            set
            {
                if (_End == value)
                    return;
                _End = value;
            }
        }
    }

}
