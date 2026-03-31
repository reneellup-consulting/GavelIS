using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win
{
    public class TempIncExp
    {
        private int _Key;
        private decimal _Income;
        private decimal _Expense;
        public int Key
        {
            get { return _Key; }
            set { this._Key = value; }
        }
        public decimal Income
        {
            get { return _Income; }
            set { this._Income = value; }
        }
        public decimal Expense
        {
            get { return _Expense; }
            set { this._Expense = value; }
        }
    }

    public class TempIncExpCollection : BaseBindingList<TempIncExp>
    {
    }
}
