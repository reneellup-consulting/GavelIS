using System;
using System.ComponentModel;
using System.Collections;
//using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TransactionDependencyController : ViewController
    {
        private PopupWindowShowAction ShowDependencyAction;
        public TransactionDependencyController()
        {
            this.TargetObjectType = typeof(XPCustomObject);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.ShowDependency", this.GetType().
            Name);
            this.ShowDependencyAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.ShowDependencyAction.Caption = "Show Dependency";
            this.ShowDependencyAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(ShowDependencyAction_CustomizePopupWindowParams);
            //this.ShowDependencyAction.Execute += new SimpleActionExecuteEventHandler(ShowDependencyAction_Execute);
        }

        void ShowDependencyAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            XPCustomObject gjh = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as XPCustomObject;
            Session session = gjh.Session;
            StringBuilder sb = new StringBuilder("The dependecies of the selected transaction:");
            sb.AppendLine();
            foreach (object obj in session.CollectReferencingObjects(gjh))
            {
                foreach (XPMemberInfo property in session.GetClassInfo(obj).PersistentProperties)
                {
                    if (property.MemberType.IsAssignableFrom(gjh.GetType()))
                    {
                        //property.SetValue(obj, null);
                        sb.AppendFormat("Property Name: {0} -->> Obj. Name: {1} >> {2}", property.Name, obj.GetType().Name,
                            session.GetKeyValue(obj));
                        sb.AppendLine();
                    }
                }
            }
            IObjectSpace objs = Application.CreateObjectSpace();
            TransactionDependency tdep = new TransactionDependency() { Dependencies = sb.ToString()
            };
            e.View = Application.CreateDetailView(
            objs, tdep, true);
        }

        //void ShowDependencyAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    XPCustomObject gjh = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as XPCustomObject;
        //    Session session = gjh.Session;
        //    StringBuilder sb = new StringBuilder("The dependecies of the selected transaction:");
        //    sb.AppendLine();
        //    foreach (object obj in session.CollectReferencingObjects(gjh))
        //    {
        //        foreach (XPMemberInfo property in session.GetClassInfo(obj).PersistentProperties)
        //        {
        //            if (property.MemberType.IsAssignableFrom(gjh.GetType()))
        //            {
        //                //property.SetValue(obj, null);
        //                sb.AppendFormat("Property Name: {0} -->> Obj. Name: {1} >> {2}", property.Name, obj.GetType().Name, 
        //                    session.GetKeyValue(obj));
        //                sb.AppendLine();
        //            }
        //        }
        //    }
        //    IObjectSpace objs = Application.CreateObjectSpace();
        //    TransactionDependency tdep = objs.CreateObject<TransactionDependency>();
        //    tdep.Dependencies = sb.ToString();
        //    DetailView viewTD = Application.CreateDetailView(
        //objs, tdep, true);
        //    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        //    e.ShowViewParameters.CreatedView = viewTD;
        //    // XtraMessageBox.Show(sb.ToString());
        //}
    }
}
