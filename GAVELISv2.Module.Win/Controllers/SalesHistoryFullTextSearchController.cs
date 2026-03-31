using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class SalesHistoryFullTextSearchController : ViewController
    {
        public SalesHistoryFullTextSearchController()
        {
            InitializeComponent();
            RegisterActions(components);
            this.TargetObjectType = typeof(InvoiceDetail);
            //this.TargetViewId = "InvoiceDetail_ListView_SalesHistory";
        }

        private void SalesHistoryFullTextSearchController_Activated(object sender, EventArgs e)
        {
            FilterController standardFilterController = Frame.GetController<FilterController>();
            if (standardFilterController != null)
            {
                standardFilterController.CustomGetFullTextSearchProperties += standardFilterController_CustomGetFullTextSearchProperties;
            }
        }

        void standardFilterController_CustomGetFullTextSearchProperties(object sender, CustomGetFullTextSearchPropertiesEventArgs e)
        {
            foreach (string property in GetFullTextSearchProperties())
            {
                e.Properties.Add(property);
            }
            e.Handled = true;
        }

        private List<string> GetFullTextSearchProperties()
        {
            List<string> searchProperties = new List<string>();
            searchProperties.Add("InvoiceInfo.InvoiceType");
            searchProperties.Add("InvoiceInfo.Customer");
            return searchProperties;
        }
    }
}
