using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Reports;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using GAVELISv2.Module.BusinessObjects;
using System.IO;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class PrintAccumCostBrandnewTubeTypeController : ViewController
    {
        private SimpleAction printAccumCostBrandnewTubeTypeAction;
        private TireItemsAccomCostDetail accDetail;
        public PrintAccumCostBrandnewTubeTypeController()
        {
            this.TargetObjectType = typeof(TireItemsAccomCostDetail);
            this.TargetViewId = "TireItemsAccomulatedCost_Details_ListView_TubeTypeBrandner";
            #region Print Calculated Attendance Sheet
            this.printAccumCostBrandnewTubeTypeAction = new SimpleAction(this, "PrintAccumCostBrandnewTubeTypeActionId",
            PredefinedCategory.RecordEdit);
            this.printAccumCostBrandnewTubeTypeAction.Caption = "Print BRANDNEW 10.00-20";
            this.printAccumCostBrandnewTubeTypeAction.Execute += new SimpleActionExecuteEventHandler(printAccumCostBrandnewTubeTypeAction_Execute);
            #endregion
        }
        void printAccumCostBrandnewTubeTypeAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            accDetail = this.View.CurrentObject as TireItemsAccomCostDetail;
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\Brandnew 10.00-20.repx";
            IObjectSpace objs = View.ObjectSpace;
            rep.LoadLayout(path);
            rep.ObjectSpace = objs;
            CollectionSourceBase cols = (this.View as ListView).CollectionSource;
            //var oCols = cols.Collection;
            //XafGridView gView = gControl.Views[0] as XafGridView;
            //ListPropertyEditor listPropertyEditor = ((ListView)View).Items as ListPropertyEditor;
            //XPCollection<TireItemsAccomCostDetail> xpc = new XPCollection<TireItemsAccomCostDetail>(((ObjectSpace)objs).Session);
            //xpc.Criteria = BinaryOperator.Parse("[TireItemClass] = 'Tire' And [TireType] = 'Tubeless' And [LineCondition] = 'Brandnew' And [Year] = ?", accDetail.Year);
            DevExpress.ExpressApp.ProxyCollection prox = cols.Collection as ProxyCollection;
            XPCollection<TireItemsAccomCostDetail> xcols = new XPCollection<TireItemsAccomCostDetail>(((ObjectSpace)objs).Session);
            if (prox.Filter != null)
            {
                xcols.Filter = prox.Filter;
            }
            else
            {
                xcols.Filter = (prox.OriginalCollection as XPCollection).Criteria;
            }
            rep.DataSource = xcols;
            rep.ShowPreview();
        }
    }
}
