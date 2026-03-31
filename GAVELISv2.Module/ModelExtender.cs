using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.Summary;
using System.ComponentModel;

namespace GAVELISv2.Module
{
    public interface IModelListViewExtender
    {
        bool IsGroupFooterVisible { get; set; }
        bool IsDisableColumnAutoWidth { get; set; }
    }
    public interface IModelColumnExtender
    {
        [DefaultValue(SummaryItemType.None)]
        [System.ComponentModel.Category("Group Footer")]
        SummaryItemType GroupFooterSummaryType { get; set; }
        [System.ComponentModel.Category("Group Footer")]
        string GroupFooterDisplayFormat { get; set; }
        [DefaultValue(SummaryItemType.None)]
        [System.ComponentModel.Category("Footer")]
        SummaryItemType FooterSummaryType { get; set; }
        [System.ComponentModel.Category("Footer")]
        string FooterDisplayFormat { get; set; }
    }
}
