using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using GAVELISv2.Module.BusinessObjects;
using System.Drawing;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ExpensePerFacilitityDetailDrillDownController : ViewController
    {
        private GridControl _Grid;
        private DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo _HitInfo;
        private ListViewProcessCurrentObjectController processCurrentObjectController;
        public ExpensePerFacilitityDetailDrillDownController()
        {
            TargetObjectType = typeof(ExpensePerFacilityDetail);
            TargetViewId = "ExpensePerFacility_ExpensePerFacilityDetails_ListView";
        }
        protected override void OnActivated()
        {
            View.ControlsCreated += new EventHandler(View_ControlsCreated);
            processCurrentObjectController =
            Frame.GetController<ListViewProcessCurrentObjectController>();
            if (processCurrentObjectController != null)
            {
                processCurrentObjectController.CustomProcessSelectedItem +=
                    processCurrentObjectController_CustomProcessSelectedItem;
            }
        }
        void View_ControlsCreated(object sender, EventArgs e)
        {
            _Grid = processCurrentObjectController.View.Control as GridControl;
            _Grid.MouseMove += new System.Windows.Forms.MouseEventHandler(_Grid_MouseMove);
        }

        void _Grid_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GridView gv = _Grid.DefaultView as GridView;
            GetHitInfo(gv.CalcHitInfo(new Point(e.X, e.Y)));
        }

        private void GetHitInfo(DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi)
        {
            _HitInfo = hi;
        }
        private void processCurrentObjectController_CustomProcessSelectedItem(
        object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            ExpensePerFacilityDetail record = (ExpensePerFacilityDetail)e.InnerArgs.CurrentObject;
            GridView gv = _Grid.DefaultView as GridView;
            if (_HitInfo != null && _HitInfo.InRow && !gv.IsGroupRow(_HitInfo.RowHandle))
            {

                ArrayList keysToShow1 = new ArrayList();
                //int dCount = 0;
                MonthsEnum mnth = GetMonthValue(_HitInfo.Column.Caption);
                if (mnth == MonthsEnum.None)
                {
                    return;
                }
                string viewId = "IncomeAndExpense02_ExpensesPerFacility";
                string selKeys = string.Empty;
                switch (mnth)
                {
                    case MonthsEnum.None:
                        break;
                    case MonthsEnum.January:
                        selKeys = record.JanKeys;
                        break;
                    case MonthsEnum.February:
                        selKeys = record.FebKeys;
                        break;
                    case MonthsEnum.March:
                        selKeys = record.MarKeys;
                        break;
                    case MonthsEnum.April:
                        selKeys = record.AprKeys;
                        break;
                    case MonthsEnum.May:
                        selKeys = record.MayKeys;
                        break;
                    case MonthsEnum.June:
                        selKeys = record.JunKeys;
                        break;
                    case MonthsEnum.July:
                        selKeys = record.JulKeys;
                        break;
                    case MonthsEnum.August:
                        selKeys = record.AugKeys;
                        break;
                    case MonthsEnum.September:
                        selKeys = record.SepKeys;
                        break;
                    case MonthsEnum.October:
                        selKeys = record.OctKeys;
                        break;
                    case MonthsEnum.November:
                        selKeys = record.NovKeys;
                        break;
                    case MonthsEnum.December:
                        selKeys = record.DecKeys;
                        break;
                    default:
                        break;
                }
                string[] split = selKeys.Split(',');
                foreach (var item in split)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        keysToShow1.Add(new Guid(item));
                    }
                }
                CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(IncomeAndExpense02), viewId);
                if (split.Length > 2100)
                {
                    collectionSource1.Criteria["GKey"] = new InOperator("GKey", keysToShow1);
                }
                else
                {
                    collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(View.ObjectTypeInfo.Type), keysToShow1);
                }
                ListView listView = Application.CreateListView(viewId, collectionSource1, true);
                ShowViewParameters svp = new ShowViewParameters(listView);
                svp.CreatedView.Caption = string.Format("{0} {1}|{2} - Expenses", _HitInfo.Column.Caption, record.MainId.Year, record.Department.Code);
                svp.TargetWindow = TargetWindow.NewModalWindow;
                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
            }
        }
        private MonthsEnum GetMonthValue(string month)
        {
            if (month == "January")
            {
                return MonthsEnum.January;
            }
            else if (month == "February")
            {
                return MonthsEnum.February;
            }
            else if (month == "March")
            {
                return MonthsEnum.March;
            }
            else if (month == "April")
            {
                return MonthsEnum.April;
            }
            else if (month == "May")
            {
                return MonthsEnum.May;
            }
            else if (month == "June")
            {
                return MonthsEnum.June;
            }
            else if (month == "July")
            {
                return MonthsEnum.July;
            }
            else if (month == "August")
            {
                return MonthsEnum.August;
            }
            else if (month == "September")
            {
                return MonthsEnum.September;
            }
            else if (month == "October")
            {
                return MonthsEnum.October;
            }
            else if (month == "November")
            {
                return MonthsEnum.November;
            }
            else if (month == "December")
            {
                return MonthsEnum.December;
            }
            else
            {
                return MonthsEnum.None;
            }
        }
        protected override void OnDeactivated()
        {
            if (processCurrentObjectController != null)
            {
                processCurrentObjectController.CustomProcessSelectedItem -=
                    processCurrentObjectController_CustomProcessSelectedItem;
            }
            base.OnDeactivated();
        }
    }
}
