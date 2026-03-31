using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.DC;

namespace GAVELISv2.Module.BusinessObjects
{
    [DomainComponent]
    public class BatteryRequest
    {
        public Requisition RequestDoc { get; set; }
        public Item BatteryItem { get; set; }
        public Battery BatteryToReplace { get; set; }
        public Employee RequestedBy { get; set; }
        public CostCenter ChargeTo { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime DateDeclared { get; set; }
        [Size(500)]
        public string Remarks { get; set; }
    }
}
