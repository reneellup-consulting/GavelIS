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
using System.Drawing;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class IncomeExpenseDrillDownController : ViewController
    {
        private GridControl _Grid;
        private DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo _HitInfo;
        private ListViewProcessCurrentObjectController processCurrentObjectController;
        public IncomeExpenseDrillDownController()
        {
            TargetObjectType = typeof(IncomeExpenseBuffer);
            TargetViewId = "IncomeExpenseReporter_IncomeExpenseBufferLines_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
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
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            Company companyInfo = Company.GetInstance(session);
            IncomeExpenseBuffer record = (IncomeExpenseBuffer)e.InnerArgs.CurrentObject;
            GridView gv = _Grid.DefaultView as GridView;
            if (_HitInfo != null && _HitInfo.InRow && !gv.IsGroupRow(_HitInfo.RowHandle))
            {

                ArrayList keysToShow1 = new ArrayList();
                int dCount = 0;
                MonthsEnum mnth = GetMonthValue(_HitInfo.Column.Caption);
                if (mnth== MonthsEnum.None)
                {
                    return;
                }
                //CriteriaOperator criteria = CriteriaOperator.Parse("[Category]=? And [GYear]=? And [GMonth]=?", record.Category, record.Year, mnth);
                //XPCollection<IncomeAndExpense02> filtered = new XPCollection<IncomeAndExpense02>(((ObjectSpace)ObjectSpace).Session, criteria, new SortProperty("GMonthSorter", DevExpress.Xpo.DB.SortingDirection.Ascending), new SortProperty("EntryDate", DevExpress.Xpo.DB.SortingDirection.Ascending));
                //select Oid from vIncomeExpense02 where [Year]=2021 and [Month]=1 and Category='2EFF2179-C543-4937-9012-69D0F0336881'

                string cmd = string.Format("select Oid from vIncomeExpense02 where [Year]={0} and [Month]={1} and Category='{2}'", record.Year, (int)mnth, record.Category.Oid);
                SelectedData data = session.ExecuteQuery(cmd);

                //for (int i = 0; i < filtered.Count; i++)
                //{
                //    object obj = filtered[i];
                //    dCount++;
                //    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                //}

                foreach (var row in data.ResultSet[0].Rows)
                {
                    dCount++;
                    keysToShow1.Add(Guid.Parse(row.Values[0].ToString()));
                }

                IObjectSpace nonPersistentOS = Application.CreateObjectSpace();
                CollectionSource tireCollectionSource = null; // new CollectionSource(nonPersistentOS, typeof(TireExpenseDetailsBuffer));
                if (keysToShow1.Count > 0 || new[] { "0360", "0050", "0350", "0650" }.Any(o => record.Category.Code.Contains(o)))
                {
                    string viewId = "IncomeAndExpense02_ReportSubDetails_Income";
                    if (record.Expense && !new[] { "0360", "0050", "0350", "0650" }.Any(o => record.Category.Code.Contains(o)))
                    {
                        viewId = "IncomeAndExpense02_ReportSubDetails";
                    }
                    else if (record.Expense && record.Category.Code == companyInfo.TireExpenseType.Code)
                    {
                        //viewId = "TireExpenseBufferList_ListView";
                        //tedetails = nonPersistentOS.CreateObject<TireExpenseDetails>();
                        tireCollectionSource = new CollectionSource(nonPersistentOS, typeof(TireExpenseDetailsBuffer));
                        int buffId = 0;
                        if ((tireCollectionSource.Collection as XPBaseCollection) != null)
                        {
                            ((XPBaseCollection)tireCollectionSource.Collection).LoadingEnabled = false;
                        }
                        foreach (var key in keysToShow1)
                        {
                            IncomeAndExpense02 ie2 = nonPersistentOS.GetObjectByKey<IncomeAndExpense02>(key);
                            TireExpenseDetailsBuffer teb = nonPersistentOS.CreateObject<TireExpenseDetailsBuffer>();
                            teb.Oid = buffId;
                            teb.EntryDate = ie2.EntryDate;
                            teb.SourceID = ie2.SourceID;
                            teb.SourceType = ie2.SourceType;
                            teb.SourceNo = ie2.SourceNo;
                            // RequestRef
                            teb.RefID = ie2.RefID;
                            teb.PayeeType = ie2.PayeeType;
                            teb.Payee = ie2.Payee;
                            teb.Description1 = ie2.Description1;
                            teb.Description2 = ie2.Description2;
                            teb.Category = ie2.Category;
                            teb.SubCategory = ie2.SubCategory;
                            teb.CostCenter = ie2.CostCenter;
                            teb.Expense = ie2.Expense;
                            teb.Income = ie2.Income;
                            teb.Fleet = ie2.Fleet;
                            teb.Facility = ie2.Facility;
                            teb.Department = ie2.Department;
                            teb.FacilityHead = ie2.FacilityHead;
                            teb.DepartmentInCharge = ie2.DepartmentInCharge;
                            tireCollectionSource.List.Add(teb);
                            buffId++;
                        }

                        string cmd2 = string.Format("select HeaderId, [Year], [Month], LineCondition, sum(Cost * Qty) as Expense, TireReqDetail, OID from vTireItemsAccomCostDetail " +
                                "where TireItemClass=1 and [Month]={0} and [Year]={1} group by HeaderId, [Year], [Month], LineCondition, TireReqDetail, OID", (int)mnth, record.Year);
                        SelectedData data2 = session.ExecuteQuery(cmd2);

                        foreach (var row in data2.ResultSet[0].Rows)
                        {
                            TireItemsAccomCostDetail tiacd = nonPersistentOS.GetObjectByKey<TireItemsAccomCostDetail>(row.Values[6]);
                            //RwsTireDetail rtd = nonPersistentOS.GetObjectByKey<RwsTireDetail>(row.Values[5]);
                            //InventoryControlJournal icj = null;
                            //ReceiptDetail rd = null;
                            //if (rtd != null)
                            //{
                            //    icj = nonPersistentOS.GetObjectByKey<InventoryControlJournal>(rtd.InvControlId.Oid);
                            //    if (icj.GenJournalID.GetType() == typeof(Receipt))
                            //    {
                            //        rd = nonPersistentOS.FindObject<ReceiptDetail>(CriteriaOperator.Parse("[RowID]=?",Guid.Parse(icj.RowID)));
                            //    }
                            //}
                            TireExpenseDetailsBuffer teb = nonPersistentOS.CreateObject<TireExpenseDetailsBuffer>();
                            teb.Oid = buffId;
                            // ********************
                            teb.EntryDate = tiacd.EntryDate;
                            teb.SourceNo = tiacd.SourceId;
                            teb.RefID = tiacd.TireReqDetail !=null ? tiacd.TireReqDetail.RwsTireDetId : null;
                            teb.Description1 = tiacd.OldTireNo + " TO " + tiacd.NewTireNo;
                            teb.Description2 = tiacd.ItemNo.Description;
                            // ********************
                            teb.Category = nonPersistentOS.GetObject<ExpenseType>(record.Category);
                            if (row.Values[3].ToString() == "Brandnew")
                            {
                                teb.SubCategory = nonPersistentOS.GetObject<SubExpenseType>(companyInfo.BrandnewTireExpenseType);
                            }
                            else if (row.Values[3].ToString() == "Recap")
                            {
                                teb.Description1 = "RECAPPED";
                                teb.SubCategory = nonPersistentOS.GetObject<SubExpenseType>(companyInfo.RecappedTireExpenseType);
                            }
                            else
                            {
                                // 106>Others
                                teb.SubCategory = nonPersistentOS.GetObject<SubExpenseType>(companyInfo.OthersTireExpenseType);
                            }
                            teb.Expense = Convert.ToDecimal(row.Values[4].ToString());
                            tireCollectionSource.List.Add(teb);
                            buffId++;
                        }

                    }
                    else if (record.Expense && record.Category.Code == companyInfo.FlapsExpenseType.Code)
                    {
                        tireCollectionSource = new CollectionSource(nonPersistentOS, typeof(TireExpenseDetailsBuffer));
                        int buffId = 0;
                        if ((tireCollectionSource.Collection as XPBaseCollection) != null)
                        {
                            ((XPBaseCollection)tireCollectionSource.Collection).LoadingEnabled = false;
                        }
                        foreach (var key in keysToShow1)
                        {
                            IncomeAndExpense02 ie2 = nonPersistentOS.GetObjectByKey<IncomeAndExpense02>(key);
                            TireExpenseDetailsBuffer teb = nonPersistentOS.CreateObject<TireExpenseDetailsBuffer>();
                            teb.Oid = buffId;
                            teb.EntryDate = ie2.EntryDate;
                            teb.SourceID = ie2.SourceID;
                            teb.SourceType = ie2.SourceType;
                            teb.SourceNo = ie2.SourceNo;
                            // RequestRef
                            teb.RefID = ie2.RefID;
                            teb.PayeeType = ie2.PayeeType;
                            teb.Payee = ie2.Payee;
                            teb.Description1 = ie2.Description1;
                            teb.Description2 = ie2.Description2;
                            teb.Category = ie2.Category;
                            teb.SubCategory = ie2.SubCategory;
                            teb.Expense = ie2.Expense;
                            teb.Income = ie2.Income;
                            teb.Fleet = ie2.Fleet;
                            teb.Facility = ie2.Facility;
                            teb.Department = ie2.Department;
                            teb.FacilityHead = ie2.FacilityHead;
                            teb.DepartmentInCharge = ie2.DepartmentInCharge;
                            tireCollectionSource.List.Add(teb);
                            buffId++;
                        }
                    }
                    else if (record.Expense && record.Category.Code == companyInfo.TubesExpenseType.Code)
                    {
                        tireCollectionSource = new CollectionSource(nonPersistentOS, typeof(TireExpenseDetailsBuffer));
                        int buffId = 0;
                        if ((tireCollectionSource.Collection as XPBaseCollection) != null)
                        {
                            ((XPBaseCollection)tireCollectionSource.Collection).LoadingEnabled = false;
                        }
                        foreach (var key in keysToShow1)
                        {
                            IncomeAndExpense02 ie2 = nonPersistentOS.GetObjectByKey<IncomeAndExpense02>(key);
                            TireExpenseDetailsBuffer teb = nonPersistentOS.CreateObject<TireExpenseDetailsBuffer>();
                            teb.Oid = buffId;
                            teb.EntryDate = ie2.EntryDate;
                            teb.SourceID = ie2.SourceID;
                            teb.SourceType = ie2.SourceType;
                            teb.SourceNo = ie2.SourceNo;
                            // RequestRef
                            teb.RefID = ie2.RefID;
                            teb.PayeeType = ie2.PayeeType;
                            teb.Payee = ie2.Payee;
                            teb.Description1 = ie2.Description1;
                            teb.Description2 = ie2.Description2;
                            teb.Category = ie2.Category;
                            teb.SubCategory = ie2.SubCategory;
                            teb.Expense = ie2.Expense;
                            teb.Income = ie2.Income;
                            teb.Fleet = ie2.Fleet;
                            teb.Facility = ie2.Facility;
                            teb.Department = ie2.Department;
                            teb.FacilityHead = ie2.FacilityHead;
                            teb.DepartmentInCharge = ie2.DepartmentInCharge;
                            tireCollectionSource.List.Add(teb);
                            buffId++;
                        }

                        //string cmd2 = string.Format("select HeaderId, [Year], [Month], sum(Cost * Qty) as Expense from vTireItemsAccomCostDetail " +
                        //        "where TireItemClass=2 and [Month]={0} and [Year]={1} group by HeaderId, [Year], [Month]", (int)mnth, record.Year);
                        //SelectedData data2 = session.ExecuteQuery(cmd2);

                        //foreach (var row in data2.ResultSet[0].Rows)
                        //{
                        //    TireExpenseDetailsBuffer teb = nonPersistentOS.CreateObject<TireExpenseDetailsBuffer>();
                        //    teb.Oid = buffId;
                        //    teb.Category = nonPersistentOS.GetObject<ExpenseType>(record.Category);
                        //    teb.SubCategory = nonPersistentOS.GetObject<SubExpenseType>(companyInfo.AccuTireFlapsExpenseType);
                        //    teb.Expense = Convert.ToDecimal(row.Values[3].ToString());
                        //    tireCollectionSource.List.Add(teb);
                        //    buffId++;
                        //}
                    }
                    else if (record.Expense && record.Category.Code == companyInfo.RimExpenseType.Code)
                    {
                        tireCollectionSource = new CollectionSource(nonPersistentOS, typeof(TireExpenseDetailsBuffer));
                        int buffId = 0;
                        if ((tireCollectionSource.Collection as XPBaseCollection) != null)
                        {
                            ((XPBaseCollection)tireCollectionSource.Collection).LoadingEnabled = false;
                        }
                        foreach (var key in keysToShow1)
                        {
                            IncomeAndExpense02 ie2 = nonPersistentOS.GetObjectByKey<IncomeAndExpense02>(key);
                            TireExpenseDetailsBuffer teb = nonPersistentOS.CreateObject<TireExpenseDetailsBuffer>();
                            teb.Oid = buffId;
                            teb.EntryDate = ie2.EntryDate;
                            teb.SourceID = ie2.SourceID;
                            teb.SourceType = ie2.SourceType;
                            teb.SourceNo = ie2.SourceNo;
                            // RequestRef
                            teb.RefID = ie2.RefID;
                            teb.PayeeType = ie2.PayeeType;
                            teb.Payee = ie2.Payee;
                            teb.Description1 = ie2.Description1;
                            teb.Description2 = ie2.Description2;
                            teb.Category = ie2.Category;
                            teb.SubCategory = ie2.SubCategory;
                            teb.Expense = ie2.Expense;
                            teb.Income = ie2.Income;
                            teb.Fleet = ie2.Fleet;
                            teb.Facility = ie2.Facility;
                            teb.Department = ie2.Department;
                            teb.FacilityHead = ie2.FacilityHead;
                            teb.DepartmentInCharge = ie2.DepartmentInCharge;
                            tireCollectionSource.List.Add(teb);
                            buffId++;
                        }

                        string cmd2 = string.Format("select HeaderId, [Year], [Month], sum(Cost * Qty) as Expense, TireReqDetail, OID  from vTireItemsAccomCostDetail " +
                                "where TireItemClass=4 and [Month]={0} and [Year]={1} group by HeaderId, [Year], [Month], TireReqDetail, OID", (int)mnth, record.Year);
                        //string cmd2 = string.Format("select HeaderId, [Year], [Month], LineCondition, sum(Cost * Qty) as Expense, TireReqDetail, OID from vTireItemsAccomCostDetail " +
                        //       "where TireItemClass=1 and [Month]={0} and [Year]={1} group by HeaderId, [Year], [Month], LineCondition, TireReqDetail, OID", (int)mnth, record.Year, TireReqDetail, OID);
                        SelectedData data2 = session.ExecuteQuery(cmd2);

                        foreach (var row in data2.ResultSet[0].Rows)
                        {
                            TireItemsAccomCostDetail tiacd = nonPersistentOS.GetObjectByKey<TireItemsAccomCostDetail>(row.Values[5]);
                            TireExpenseDetailsBuffer teb = nonPersistentOS.CreateObject<TireExpenseDetailsBuffer>();
                            teb.Oid = buffId;
                            // ********************
                            teb.EntryDate = tiacd.EntryDate;
                            teb.SourceNo = tiacd.SourceId;
                            teb.RefID = tiacd.TireReqDetail != null ? tiacd.TireReqDetail.RwsTireDetId : null;
                            teb.Description1 = "RIM";
                            teb.Description2 = tiacd.ItemNo.Description;
                            // ********************
                            teb.Category = nonPersistentOS.GetObject<ExpenseType>(record.Category);
                            teb.Expense = Convert.ToDecimal(row.Values[3].ToString());
                            teb.SubCategory = nonPersistentOS.GetObject<SubExpenseType>(companyInfo.RimSubExpenseType);
                            tireCollectionSource.List.Add(teb);
                            buffId++;
                        }
                    }
                    else if (record.Expense!=true && record.Category.Code =="0002")
                    {
                        viewId = "IncomeAndExpense02_ReportSubDetails_Trucking";
                    }
                    else
                    {
                        viewId = "IncomeAndExpense02_ReportSubDetails_Income";
                    }

                    if (new[] { "0360", "0050", "0350", "0650" }.Any(o => record.Category.Code.Contains(o)))
                    {
                        //CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(TireExpenseDetailsBuffer), viewId);
                        //collectionSource1.Add(tedetails.Details);
                        //collectionSource1.List = tedetails
                        //foreach (var item in tedetails)
                        //{
                        //    collectionSource1.List.Add(item);
                        //}
                        ListView listView = Application.CreateListView(Application.GetListViewId(typeof(TireExpenseDetailsBuffer)), tireCollectionSource, true);
                        ShowViewParameters svp = new ShowViewParameters(listView);
                        svp.CreatedView.Caption = string.Format("{0}|{1}", _HitInfo.Column.Caption, svp.CreatedView.Caption);
                        svp.TargetWindow = TargetWindow.NewModalWindow;
                        Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
                    }
                    else
                    {
                        CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(IncomeAndExpense02), viewId);
                        if (dCount > 2100)
                        {
                            collectionSource1.Criteria["GKey"] = new InOperator("GKey", keysToShow1);
                        }
                        else
                        {
                            collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(View.ObjectTypeInfo.Type), keysToShow1);
                        }
                        ListView listView = Application.CreateListView(viewId, collectionSource1, true);
                        ShowViewParameters svp = new ShowViewParameters(listView);
                        svp.CreatedView.Caption = string.Format("{0}|{1}", _HitInfo.Column.Caption, svp.CreatedView.Caption);
                        svp.TargetWindow = TargetWindow.NewModalWindow;
                        Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
                    }
                }
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
