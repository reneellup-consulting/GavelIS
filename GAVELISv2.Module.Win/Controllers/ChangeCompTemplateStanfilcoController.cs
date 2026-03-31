using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ChangeCompTemplateStanfilcoController : ViewController
    {
        private ParametrizedAction ChangeCompTemplateAction;
        public ChangeCompTemplateStanfilcoController()
        {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.ListView;
            string actionID = "ChangeCompTemplateStfActionId";
            this.ChangeCompTemplateAction = new ParametrizedAction(this, actionID, PredefinedCategory.Edit, typeof(string));
            this.ChangeCompTemplateAction.Caption = "Change Computation Template";
            this.ChangeCompTemplateAction.ShortCaption = "Compu";
            this.ChangeCompTemplateAction.Execute += new ParametrizedActionExecuteEventHandler(ChangeCompTemplateAction_Execute);
            this.ChangeCompTemplateAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(ChangeCompTemplateAction_ExecuteCompleted);
        }

        void ChangeCompTemplateAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            string paramValue = e.ParameterCurrentValue as string;
            var sels = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            NetBillingCompSetup findObject = objectSpace.FindObject<NetBillingCompSetup>(CriteriaOperator.Parse(string.Format("Contains([Code], '{0}')", paramValue)));
            foreach (StanfilcoTrip item in sels)
            {
                StanfilcoTrip stf = objectSpace.GetObject<StanfilcoTrip>(item);
                stf.ComputationTemplate = findObject;
                stf.Save();
            }
            objectSpace.CommitChanges();
        }
        private void ChangeCompTemplateAction_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            ObjectSpace.Refresh();
        }
    }
}
