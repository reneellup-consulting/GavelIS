using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Editors;
namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class AdjustItemCostPrices2 : BaseObject {
        private string _ItemsFilter;
        private decimal _MarkupRate;
        private string _TemplateFilePath;
        private AdjustItemCostPricesEnum _AdjustMode;
        private decimal _TestProgress;
        private decimal _Progress = 0;
        private Type ObjectType { get { return typeof(Item); } }
        [CriteriaObjectTypeMember("ObjectType"),
        Size(-1),
        ImmediatePostData]
        public string ItemsFilter {
            get { return _ItemsFilter; }
            set { SetPropertyValue("ItemsFilter", ref _ItemsFilter, value); }
        }

        public decimal MarkupRate {
            get { return _MarkupRate; }
            set { SetPropertyValue("MarkupRate", ref _MarkupRate, value); }
        }

        public string TemplateFilePath {
            get { return _TemplateFilePath; }
            set { SetPropertyValue("TemplateFilePath", ref _TemplateFilePath, 
                value); }
        }
        
        public decimal TestProgress {
            get { return _TestProgress; }
            set { SetPropertyValue("TestProgress", ref _TestProgress, value);
            if (!IsLoading)
            {
                    Progress = _TestProgress;
            }
            }
        }

        public AdjustItemCostPricesEnum AdjustMode {
            get { return _AdjustMode; }
            set { SetPropertyValue("AdjustMode", ref _AdjustMode, value); }
        }

        public decimal Progress {
            get { return _Progress; }
            set { SetPropertyValue("Progress", ref _Progress, value); }
        }

        public string CriterionString { get { return ItemsFilter; } }
        //[Action(Caption = "Start Adjustment")]
        public void StartAdjustment() {
            switch (_AdjustMode) {
                case AdjustItemCostPricesEnum.FromTemplate:
                    Microsoft.Office.Interop.Excel.Application app = new 
                    Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.
                    Open(_TemplateFilePath, Type.Missing, Type.Missing, Type.
                    Missing, Type.Missing, Type.Missing, Type.Missing, Type.
                    Missing, Type.Missing, Type.Missing, Type.Missing, Type.
                    Missing, Type.Missing, Type.Missing, Type.Missing);
                    Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.
                    Office.Interop.Excel.Worksheet)wb.Sheets["Sheet1"];
                    try {
                        Microsoft.Office.Interop.Excel.Range excelRange = sheet.
                        UsedRange;
                        foreach (Microsoft.Office.Interop.Excel.Range row in 
                        excelRange.Rows) {
                            int rowNumber = row.Row;
                            if (rowNumber != 1) {
                                //int i = 0;
                                string[] A4D4 = GetRange("A" + rowNumber + ":E" 
                                + rowNumber + "", sheet);
                                if (!string.IsNullOrEmpty(A4D4[0])) {
                                    Item _itm = Session.FindObject<Item>(
                                    BinaryOperator.Parse("[No] = '" + A4D4[0] + 
                                    "'"));
                                    _itm.SalesPrice = Convert.ToDecimal(A4D4[4])
                                    ;
                                    _itm.Save();
                                    Progress = 40;
                                }
                            }
                        }
                    } catch(Exception ex) {
                        if (wb != null) {wb.Close();}
                        throw new ApplicationException(ex.Message);
                    } finally {
                        Session.CommitTransaction();
                        if (wb != null) {wb.Close();}
                    }
                    break;
                case AdjustItemCostPricesEnum.InputRate:
                    break;
                default:
                    break;
            }
        }

        public string[] GetRange(string range, Microsoft.Office.Interop.Excel.
        Worksheet excelWorksheet) {
            Microsoft.Office.Interop.Excel.Range workingRangeCells = 
            excelWorksheet.get_Range(range, Type.Missing);
            //workingRangeCells.Select();
            System.Array array = (System.Array)workingRangeCells.Cells.Value2;
            string[] arrayS = this.ConvertToStringArray(array);
            return arrayS;
        }

        internal string[] ConvertToStringArray(System.Array values) {
            string[] theArray = new string[values.Length];
            for (int i = 1; i <= values.Length; i++) {
                if (values.GetValue(1, i) == null) theArray[i - 1] = ""; else 
                theArray[i - 1] = (string)values.GetValue(1, i).ToString();
            }
            return theArray;
        }

        public AdjustItemCostPrices2(Session session): base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public static AdjustItemCostPrices2 GetInstance(Session session) {
            //Get the Singleton's instance if it exists 
            AdjustItemCostPrices2 result = session.FindObject<
            AdjustItemCostPrices2>(null);
            //Create the Singleton's instance 
            if (result == null) {
                result = new AdjustItemCostPrices2(session);
                result.Save();
            }
            return result;
        }

        //Prevent the Singleton from being deleted 
        protected override void OnDeleting() { throw new UserFriendlyException(
            "The system prohibits the deletion of Adjust Item Cost Prices Object."
            ); }
    }

}
