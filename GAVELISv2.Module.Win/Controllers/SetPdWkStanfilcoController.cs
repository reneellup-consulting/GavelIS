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
    public partial class SetPdWkStanfilcoController : ViewController
    {
        private ParametrizedAction SetPdWkStanfilcoAction;
        public SetPdWkStanfilcoController()
        {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.ListView;
            string actionID = "SetPdWkStanfilcoActionId";
            this.SetPdWkStanfilcoAction = new ParametrizedAction(this, actionID, PredefinedCategory.Edit, typeof(string));
            this.SetPdWkStanfilcoAction.Caption = "Set Pd/Wk";
            this.SetPdWkStanfilcoAction.ShortCaption = "Pd/Wk";
            this.SetPdWkStanfilcoAction.Execute += new ParametrizedActionExecuteEventHandler(ChangeCompTemplateAction_Execute);
            this.SetPdWkStanfilcoAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(ChangeCompTemplateAction_ExecuteCompleted);
        }
        private void ChangeCompTemplateAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            string paramValue = e.ParameterCurrentValue as string;
            var sels = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            string[] split = paramValue.Split(new char[] {','});
            if (split.Count() != 3)
            {
                throw new ApplicationException("Invalid value. e.g. 2016,11,4 for Year 2016 period 11 week 4");
            }
            int yr = Int32.Parse(split[0]);
            if (yr.GetType() != typeof(Int32))
            {
                throw new ApplicationException("Period is not a type of integer");
            }
            int pd = Int32.Parse(split[1]);
            if (pd.GetType()!=typeof(Int32))
            {
                throw new ApplicationException("Period is not a type of integer");
            }
            int wk = Int32.Parse(split[2]);
            if (wk.GetType() != typeof(Int32))
            {
                throw new ApplicationException("Week is not a type of integer");
            }
            foreach (StanfilcoTrip item in sels)
            {
                StanfilcoTrip stf = objectSpace.GetObject<StanfilcoTrip>(item);
                stf.BYear = yr;
                stf.Period = pd;
                stf.Week = wk;
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
