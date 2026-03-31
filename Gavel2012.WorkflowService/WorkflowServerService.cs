using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Workflow.CommonServices;
using System.Activities;
using System.Activities.XamlIntegration;
using System.IO;

namespace Gavel2012.WorkflowService
{
    public partial class WorkflowService : System.ServiceProcess.ServiceBase
    {
        private WorkflowServer server;
        protected override void OnStart(string[] args)
        {
            server.Start();
        }

        protected override void OnStop()
        {
            server.Stop();
        }
        public WorkflowService()
        {
            InitializeComponent();

            ServerApplication serverApplication = new ServerApplication();
            serverApplication.ApplicationName = "GAVELISv2";
            // The service can only manage workflows for those business classes that are contained in Modules specified by the serverApplication.Modules collection.
            // So, do not forget to add the required Modules to this collection via the serverApplication.Modules.Add method.
            serverApplication.Modules.Add(new SecurityModule());
            serverApplication.Modules.Add(new WorkflowModule());
            serverApplication.Modules.Add(new GAVELISv2.Module.GAVELISv2Module());

            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
            {
                serverApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            //serverApplication.Security = new SecurityComplex<User, Role>(
            //    new WorkflowServerAuthentication(new BinaryOperator("UserName", "WorkflowService")));
            serverApplication.Setup();
            serverApplication.Logon();

            IObjectSpaceProvider objectSpaceProvider = serverApplication.ObjectSpaceProvider;
            
            server = new WorkflowServer("http://localhost:46233", objectSpaceProvider, objectSpaceProvider);
            server.HostManagerActivityProvider = new CustomHostManagerActivityProvider();
            server.StartWorkflowListenerService.DelayPeriod = TimeSpan.FromSeconds(15);
            server.StartWorkflowByRequestService.RequestsDetectionPeriod = TimeSpan.FromSeconds(15);
            server.RefreshWorkflowDefinitionsService.DelayPeriod = TimeSpan.FromMinutes(15);

            server.CustomizeHost += delegate(object sender, CustomizeHostEventArgs e)
            {
                e.WorkflowInstanceStoreBehavior.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(15);
            };

            server.CustomHandleException += delegate(object sender, CustomHandleServiceExceptionEventArgs e)
            {
                Tracing.Tracer.LogError(e.Exception);
                e.Handled = false;
            };
        }
    }

    public class CustomHostManagerActivityProvider : WorkflowServerService
    {
        private void Manager_HostOpening(object sender, EventArgs e)
        {
            IList<IWorkflowDefinition> definitions = GetService<IWorkflowDefinitionProvider>().GetDefinitions();
            Dictionary<string, Activity> activities = new Dictionary<string, Activity>();
            foreach (IWorkflowDefinition definition in definitions)
            {
                if (definition.CanOpenHost)
                {
                    Activity activity = ActivityXamlServices.Load(new StringReader(definition.Xaml));
                    activity.DisplayName = definition.GetActivityTypeName();
                    activities.Add(definition.GetUniqueId(), activity);
                }
            }

            HostManager.RefreshHosts(activities);
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            HostManager.HostsOpening += new EventHandler(Manager_HostOpening);
        }
    }
}
