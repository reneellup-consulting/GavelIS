using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions, Persistent("vDayToDayItemReports")]
    public class DayToDayItemReports : XPLiteObject
    {
        public DayToDayItemReports(Session session) : base(session) { }
        [Key, Persistent, Browsable(false)]
        public DayToDayItemReportsKey Key;
        public int OID { get { return Key.OID; } }
        public DateTime EntryDate { get { return Key.EntryDate; } }
        public string SourceNo { get { return Key.SourceNo; } }
        public string SourceType { get { return Key.SourceType; } }
        public decimal QTY { get { return Key.QTY; } }
        public string ItemNo { get { return Key.ItemNo; } }
        public decimal Cost { get { return Key.Cost; } }
        public string UOM { get { return Key.UOM; } }
        public string WHSE { get { return Key.WHSE; } }
        public string RequisitionNo { get { return Key.RequisitionNo; } }
        public decimal Price { get { return Key.Price; } }
        public string DateIssued { get { return Key.DateIssued; } }
    }

    public struct DayToDayItemReportsKey
    {
        [Persistent("OID"), Custom("DisplayFormat", "d"), Browsable(false)]
        public int OID;
        [Persistent("EntryDate"), Browsable(false)]
        public DateTime EntryDate;
        [Persistent("SourceNo"), Browsable(false)]
        public string SourceNo;
        [Persistent("SourceType"), Browsable(false)]
        public string SourceType;
        [Persistent("QTY"), Browsable(false)]
        public decimal QTY;
        [Persistent("ItemNo"), Browsable(false)]
        public string ItemNo;
        [Persistent("Cost"),Custom("DisplayFormat", "n"), Browsable(false)]
        public decimal Cost;
        [Persistent("UOM"), Browsable(false)]
        public string UOM;
        [Persistent("WHSE"), Browsable(false)]
        public string WHSE;
        [Persistent("ReqNo"), Browsable(false)]
        public string RequisitionNo;
        [Persistent("Price"), Browsable(false)]
        public decimal Price;
        [Persistent("DateIssued"), Browsable(false)]
        public string DateIssued;
    }
}