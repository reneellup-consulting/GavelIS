using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;

namespace GAVELISv2.Module.Win {
    public class WorkerArgs {
        public WorkerArgs(IObjectSpace objectSpace, Microsoft.Office.Interop.Excel.Range rows, Microsoft.Office.Interop.Excel.Worksheet sheet, int param1)
        {
            this.ObjectSpace = objectSpace;
            Rows = rows;
            Sheet = sheet;
            Param1 = param1;
        }

        public WorkerArgs(IObjectSpace ObjectSpace, Microsoft.Office.Interop.Excel.Sheets sheets)
        {
            // Complete member initialization
            this.ObjectSpace = ObjectSpace;
            this.Sheets = sheets;
        }

        public IObjectSpace ObjectSpace { get; set; }
        public Microsoft.Office.Interop.Excel.Range Rows { get; set; }
        public Microsoft.Office.Interop.Excel.Worksheet Sheet { get; set; }
        public Microsoft.Office.Interop.Excel.Sheets Sheets { get; set; }
        public int Param1 { get; set; }

    }
}
