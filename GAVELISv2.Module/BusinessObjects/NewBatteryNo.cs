using System;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace GAVELISv2.Module.BusinessObjects
{
    [DomainComponent]
    public class NewBatteryNo
    {
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string NewNo { get; set; }
    }
}
