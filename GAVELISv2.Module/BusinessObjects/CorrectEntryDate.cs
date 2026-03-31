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
    public class CorrectEntryDate
    {
        [Custom("AllowEdit", "False")]
        public DateTime OldEntryDate { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
