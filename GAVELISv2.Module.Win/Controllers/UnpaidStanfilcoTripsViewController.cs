using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class UnpaidStanfilcoTripsViewController : ViewController
    {
        public UnpaidStanfilcoTripsViewController()
        {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "StanfilcoTrip_UnpaidTrips_Selector";
            this.ViewControlsCreated += new EventHandler(ListViewController_ViewControlsCreated);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated += new EventHandler(View_ControlsCreated);
        }
        void View_ControlsCreated(object sender, EventArgs e)
        {
            GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
            if (listEditor != null)
            {
                GridView gridView = listEditor.GridView;
                if (gridView.Columns.Count > 0)
                {
                    GridColumn gridColumn;
                    for (int i = 0; i < gridView.Columns.Count; i++)
                    {
                        gridColumn = gridView.Columns[i];
                        if (gridColumn != null && new[] { 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120 }.Any(o => i.Equals(o)))
                        {
                            gridColumn.Fixed = FixedStyle.Left;
                        }
                        //if (gridColumn != null){
                        //    Console.WriteLine(string.Format("{0} - {1} - {2}", i, gridColumn.Caption, gridColumn.FieldName));
                        //}
                    }
                }

            }
        }
        void ListViewController_ViewControlsCreated(object sender, EventArgs e)
        {
            GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
            if (listEditor != null)
            {
                GridView gridView = listEditor.GridView;
                gridView.OptionsView.ColumnAutoWidth = false;
            }
        }
    }
}
