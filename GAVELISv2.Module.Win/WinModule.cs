using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DevExpress.ExpressApp;

namespace GAVELISv2.Module.Win
{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class GAVELISv2WindowsFormsModule : ModuleBase
    {
        public GAVELISv2WindowsFormsModule()
        {
            InitializeComponent();
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule));
        }
    }
}
