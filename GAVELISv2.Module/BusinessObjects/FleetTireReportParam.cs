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
    public class FleetTireReportParam : ReportParametersObjectBase {
        public FleetTireReportParam(Session session)
            : base(session) {
        }

        public override CriteriaOperator GetCriteria() {
            StringBuilder sbCrit = new StringBuilder();
            //if (_FleetType != null)
            //{
            //}
            // [Fixed Asset Class] = 'Truck'
            sbCrit.AppendFormat("[FixedAssetClass] = '{0}'", _FleetType.ToString());
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }

        public override SortingCollection GetSorting() {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private FixedAssetClassEnum _FleetType = FixedAssetClassEnum.Truck;
        [Custom("AllowEdit", "False")]
        public FixedAssetClassEnum FleetType {
            get { return _FleetType; }
            set { SetPropertyValue<FixedAssetClassEnum>("FleetType", ref _FleetType, value); }
        }

        private FleetTypeEnum _FleetType2;
        [DisplayName("Type of Fleet")]
        [ImmediatePostData]
        public FleetTypeEnum FleetType2 {
            get { return _FleetType2; }
            set { SetPropertyValue<FleetTypeEnum>("FleetType2", ref _FleetType2, value);
            if (!IsLoading)
            {
                switch (_FleetType2)
                {
                    case FleetTypeEnum.Truck:
                            FleetType = FixedAssetClassEnum.Truck;
                        break;
                    case FleetTypeEnum.Trailer:
                            FleetType = FixedAssetClassEnum.Trailer;
                        break;
                    default:
                        break;
                }
            }
            }
        }
    }

}
