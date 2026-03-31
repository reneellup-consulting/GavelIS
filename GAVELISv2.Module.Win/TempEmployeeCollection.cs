using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win {
    public class TempEmployee {
        private Employee _Employee;
        public Employee Employee {
            get { return _Employee; }
            set { this._Employee = value; }
        }
    }
    public class TempEmployeeCollection : BaseBindingList<TempEmployee> {
    }
}
