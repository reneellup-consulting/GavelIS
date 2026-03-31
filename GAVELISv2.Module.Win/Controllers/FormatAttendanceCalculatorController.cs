using System;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class FormatAttendanceCalculatorController : ViewController<ListView> 
    {
        GridListEditor gridListEditor = null;
        public FormatAttendanceCalculatorController()
        {
            TargetViewId = "AttendanceCalculator_ShiftCalculations2_ListView";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            gridListEditor = View.Editor as GridListEditor;
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (gridListEditor != null)
            {
                GridView gridView = gridListEditor.GridView;
                foreach (GridColumn item in gridView.Columns)
                {
                    if (new[] { "Zero(Hrs)", "1st(Hrs)", "2nd(Hrs)", "3rd(Hrs)" }.Any(o => item.Caption.Contains(o)))
                    {
                        item.AppearanceCell.BackColor = Color.DimGray;
                    }
                }
            }
        }
    }
}
