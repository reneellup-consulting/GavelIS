using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using GAVELISv2.Module.BusinessObjects;
using GAVELISv2.Module.Win;
using System.Drawing;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ExpenseAnalyticDetailsDrillDownController : ViewController
    {
        private GridControl _Grid;
        private DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo _HitInfo;
        private ListViewProcessCurrentObjectController processCurrentObjectController;
        public ExpenseAnalyticDetailsDrillDownController()
        {
            TargetObjectType = typeof(ExpensesAnalyticsDetails);
            TargetViewId = "ExpensesAnalyticsHeader_ExpensesAnalyticsDetailLines_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated += new EventHandler(View_ControlsCreated);
            processCurrentObjectController =
            Frame.GetController<ListViewProcessCurrentObjectController>();
            if (processCurrentObjectController != null)
            {
                processCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
            }
        }

        void processCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            Company companyInfo = Company.GetInstance(session);
            ExpensesAnalyticsDetails record = (ExpensesAnalyticsDetails)e.InnerArgs.CurrentObject;
            GridView gv = _Grid.DefaultView as GridView;
            if (_HitInfo != null && _HitInfo.InRow && !gv.IsGroupRow(_HitInfo.RowHandle))
            {
                session.ExecuteNonQuery("delete ExpenseAnalyticPivotBuffer");
                ArrayList keysToShow1 = new ArrayList();
                int dCount = 0;
                if (_HitInfo.Column.Caption == "Category")
                {
                    e.Handled = false;
                    return;
                }
                else if (new[] {"January","February","March","April","May",
                    "June","July","August","September","October","November","December"}.Any(o => _HitInfo.Column.Caption.Contains(o)))
                {
                    ExpenseType value = gv.GetRowCellValue(_HitInfo.RowHandle, "Category") as ExpenseType;
                    int mnth = DrilldownEnumHelper.GetMonthNo(_HitInfo.Column.Caption);
                    // select OID, ExpenseType, [Year], Month(entrydate) AS [Month] from ExpenseAnalyticsBuffer where [Year]=2022 and ExpenseType='2EFF2179-C543-4937-9012-69D0F0336881'
                    string cmd = string.Format("select * from vExpenseAnalyticsKeys where GYear={0} and GMonth={1} and ExpenseType='{2}'", record.Year, mnth, value.Oid);
                    SelectedData data = session.ExecuteQuery(cmd);
                    foreach (var row in data.ResultSet[0].Rows)
                    {
                        dCount++;
                        keysToShow1.Add(row.Values[0]);
                    }
                    IObjectSpace nonPersistentOS = Application.CreateObjectSpace();
                    // ExpenseAnalyticPivotBuffer
                    CollectionSource ExpenseAnalyticPivotBufferSource = null;
                    if (keysToShow1.Count > 0) {
                        ExpenseAnalyticPivotBufferSource = new CollectionSource(nonPersistentOS, typeof(ExpenseAnalyticPivotBuffer));
                        int buffId = 0;
                        if ((ExpenseAnalyticPivotBufferSource.Collection as XPBaseCollection) != null)
                        {
                            ((XPBaseCollection)ExpenseAnalyticPivotBufferSource.Collection).LoadingEnabled = false;
                        }
                        foreach (var key in keysToShow1)
                        {
                            ExpenseAnalyticsBuffer eab = nonPersistentOS.GetObjectByKey<ExpenseAnalyticsBuffer>(key);
                            ExpenseAnalyticPivotBuffer eapv = nonPersistentOS.FindObject<ExpenseAnalyticPivotBuffer>(CriteriaOperator.Parse("[BufferId]=?", eab.BufferId));
                            if (eapv == null)
                            {
                                eapv = nonPersistentOS.CreateObject<ExpenseAnalyticPivotBuffer>();
                            }
                            eapv.Oid = buffId;
                            eapv.Category = eab.ExpenseType;
                            eapv.SubCategory = eab.SubExpenseType;
                            eapv.Year = eab.GYear;
                            eapv.Month = eab.GMonth;
                            eapv.PaymentType = eab.PaymentMode;
                            eapv.Amount = eab.Amount;
                            eapv.BufferId = eab.BufferId;
                            ExpenseAnalyticPivotBufferSource.List.Add(eapv);
                            buffId++;
                        }
                    }
                    if (ExpenseAnalyticPivotBufferSource == null)
                    {
                        e.Handled = false;
                    }
                    ListView listView = Application.CreateListView(Application.GetListViewId(typeof(ExpenseAnalyticPivotBuffer)), ExpenseAnalyticPivotBufferSource, true);
                    ShowViewParameters svp = new ShowViewParameters(listView);
                    svp.CreatedView.Caption = string.Format("{0}|{1}", _HitInfo.Column.Caption, "Month Expense Analysis");
                    svp.TargetWindow = TargetWindow.NewModalWindow;
                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
                }
                else
                {
                    ExpenseType value = gv.GetRowCellValue(_HitInfo.RowHandle, "Category") as ExpenseType;
                    // select * from vExpenseAnalyticsKeys where ExpenseType='619FBDFB-51A8-4DEB-9D79-EE0EF96E0646' and GMonth=1 and PaymentMode=1
                    int mMode = DrilldownEnumHelper.GetMonthMode(_HitInfo.Column.Caption);
                    int pMode = DrilldownEnumHelper.GetPaymentMode(_HitInfo.Column.Caption);
                    string cmd = string.Format("select * from vExpenseAnalyticsKeys where ExpenseType='{0}' and GYear={1} and GMonth={2} and PaymentMode={3}", value.Oid, record.Year, mMode, pMode);
                    SelectedData data = session.ExecuteQuery(cmd);
                    foreach (var row in data.ResultSet[0].Rows)
                    {
                        dCount++;
                        keysToShow1.Add(row.Values[0]);
                    }
                    IObjectSpace nonPersistentOS = Application.CreateObjectSpace();
                    // ExpenseAnalyticPivotBuffer
                    CollectionSource ExpenseAnalyticPivotBufferSource = null;
                    if (keysToShow1.Count > 0)
                    {
                        ExpenseAnalyticPivotBufferSource = new CollectionSource(nonPersistentOS, typeof(ExpenseAnalyticPivotBuffer));
                        int buffId = 0;
                        if ((ExpenseAnalyticPivotBufferSource.Collection as XPBaseCollection) != null)
                        {
                            ((XPBaseCollection)ExpenseAnalyticPivotBufferSource.Collection).LoadingEnabled = false;
                        }
                        foreach (var key in keysToShow1)
                        {
                            ExpenseAnalyticsBuffer eab = nonPersistentOS.GetObjectByKey<ExpenseAnalyticsBuffer>(key);
                            ExpenseAnalyticPivotBuffer eapv = nonPersistentOS.FindObject<ExpenseAnalyticPivotBuffer>(CriteriaOperator.Parse("[BufferId]=?", eab.BufferId));
                            if (eapv == null)
                            {
                                eapv = nonPersistentOS.CreateObject<ExpenseAnalyticPivotBuffer>();
                            }
                            eapv.Oid = buffId;
                            eapv.Category = eab.ExpenseType;
                            eapv.SubCategory = eab.SubExpenseType;
                            eapv.Year = eab.GYear;
                            eapv.Month = eab.GMonth;
                            eapv.PaymentType = eab.PaymentMode;
                            eapv.Amount = eab.Amount;
                            eapv.BufferId = eab.BufferId;
                            ExpenseAnalyticPivotBufferSource.List.Add(eapv);
                            buffId++;
                        }
                    }
                    if (ExpenseAnalyticPivotBufferSource == null)
                    {
                        e.Handled = false;
                        return;
                    }
                    ListView listView = Application.CreateListView(Application.GetListViewId(typeof(ExpenseAnalyticPivotBuffer)), ExpenseAnalyticPivotBufferSource, true);
                    ShowViewParameters svp = new ShowViewParameters(listView);
                    svp.CreatedView.Caption = string.Format("{0}|{1}", _HitInfo.Column.Caption, "Month Expense Analysis");
                    svp.TargetWindow = TargetWindow.NewModalWindow;
                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
                }
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
    }
}
