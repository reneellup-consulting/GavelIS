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
    public class PdcSalesReportParam : ReportParametersObjectBase
    {
        private PdcStatusSelectionEnum _PdcStatusSelection;
        public PdcSalesReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            StringBuilder sbCrit = new StringBuilder();
            switch (_PdcStatusSelection)
            {
                case PdcStatusSelectionEnum.Due:
                    sbCrit.AppendFormat("[SourceType.Code] In ('CR', 'FT') And [Voided] = False And [OperationType.Code] = 'PR' And [PostDated] = True And [PdcCleared] = False And [PdcDue] = True");
                    break;
                case PdcStatusSelectionEnum.Uncleared:
                    sbCrit.AppendFormat("[SourceType.Code] In ('CR', 'FT') And [Voided] = False And [OperationType.Code] = 'PR' And [PostDated] = True And [PdcCleared] = False");
                    break;
                default:
                    break;
            }
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
        public PdcStatusSelectionEnum PdcStatusSelection
        {
            get
            {
                return _PdcStatusSelection;
            }
            set
            {
                if (_PdcStatusSelection == value)
                    return;
                _PdcStatusSelection = value;
            }
        }

        public string Title
        {
            get
            {
                switch (_PdcStatusSelection)
                {
                    case PdcStatusSelectionEnum.Due:
                        return "ALL DUE PDC (SALES)";
                    case PdcStatusSelectionEnum.Uncleared:
                        return "ALL UNCLEARED PDC (SALES)";
                    default:
                        return string.Empty;
                }
            }
        }
    }

}
