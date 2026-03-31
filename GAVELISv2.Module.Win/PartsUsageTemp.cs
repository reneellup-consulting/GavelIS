using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win
{
    public class PartsUsageTemp
    {
        private List<InventoryControlJournal> _Icjs = new List<InventoryControlJournal>();
        public string SourceType { get; set; }
        public decimal Qty { get; set; }
        public decimal Sales { get; set; }
        public decimal Cost { get; set; }
        //public int Year { get; set; }
        //public MonthsEnum Month { get; set; }

        public List<InventoryControlJournal> Icjs
        {
            get
            {
                return _Icjs;
            }
            set
            {
                if (_Icjs == value)
                    return;
                _Icjs = value;
            }
        }
        //get
        //    {
        //        return _All;
        //    }
        //    set
        //    {
        //        if (_All == value)
        //            return;
        //        _All = value;
        //    }
    }

    public class PartsUsageTempCollection : BaseBindingList<PartsUsageTemp>
    {

    }
}
