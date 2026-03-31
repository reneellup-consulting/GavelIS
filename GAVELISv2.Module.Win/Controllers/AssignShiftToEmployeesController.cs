using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class AssignShiftToEmployeesController : ViewController
    {
        private PopupWindowShowAction assignShiftToEmployees;
        public AssignShiftToEmployeesController()
        {
            this.TargetObjectType = typeof(Employee);
            this.TargetViewType = ViewType.ListView;
            string actionID = "Employee.AssignShiftToEmployees";
            this.assignShiftToEmployees = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.assignShiftToEmployees.Caption = "Assign Shift";
            this.assignShiftToEmployees.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(assignShiftToEmployees_CustomizePopupWindowParams);
            this.assignShiftToEmployees.Execute += new PopupWindowShowActionExecuteEventHandler(assignShiftToEmployees_Execute);
        }

        void assignShiftToEmployees_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (var item in View.SelectedObjects)
            {
                Employee emp = ObjectSpace.GetObject(item) as Employee;
                foreach (TimeTable ttbl in e.PopupWindow.View.SelectedObjects)
                {
                    TimeTable ottbl = ObjectSpace.GetObject<TimeTable>(ttbl);
                    ShiftEmployee ose;
                    ose = emp.ShiftEmployees.Where(o => o.Shift == ottbl).FirstOrDefault();
                    if (ose != null)
                    {
                        continue;
                    }
                    ose = ObjectSpace.CreateObject<ShiftEmployee>();
                    ose.Shift = ottbl;
                    ose.Save();
                    emp.ShiftEmployees.Add(ose);
                }
                emp.Save();
                ObjectSpace.CommitChanges();
            }
        }

        void assignShiftToEmployees_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(TimeTable));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(TimeTable), listViewId)
            ;
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}
