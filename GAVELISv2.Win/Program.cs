using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using GAVELISv2.Module;
using GAVELISv2.Module.BusinessObjects;
using GAVELISv2.Module.Win;
using DevExpress.ExpressApp.Workflow.Win;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Utils;

namespace GAVELISv2.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.
            IsAttached;
            GAVELISv2WindowsFormsApplication winApplication = new 
            GAVELISv2WindowsFormsApplication();
            winApplication.Modules.FindModule<WorkflowWindowsFormsModule>().QueryAvailableActivities +=
            delegate(object sender, ActivitiesInformationEventArgs e)
            {
                e.ActivitiesInformation.Add(new ActivityInformation(typeof(CreatePOFromOFRSBuffer),"Code Activities", "Create PO from OFRS"));
                //    ,
                //"Code Activities", "Generate Applied Taxes",
                //ImageLoader.Instance.GetImageInfo("Images_Mail_Outbo").Image));
            };
            string name = winApplication.ApplicationName;
            winApplication.CreateCustomTemplate += new EventHandler<
            CreateCustomTemplateEventArgs>(winApplication_CreateCustomTemplate);
            DevExpress.Data.Filtering.CriteriaOperator.RegisterCustomFunction(new TimeAddFunction());
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != 
            null) {winApplication.ConnectionString = ConfigurationManager.
                ConnectionStrings["ConnectionString"].ConnectionString;}
            try {
                // Uncomment this line when using the Middle Tier application server:
                //new DevExpress.ExpressApp.MiddleTier.MiddleTierClientApplicationConfigurator(winApplication);
                winApplication.SplashScreen = new SplashScreen();
                winApplication.Setup();
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
        private static void winApplication_CreateCustomTemplate(object sender, 
        CreateCustomTemplateEventArgs e) {
            if (e.Context.Name == TemplateContext.ApplicationWindow) {e.Template 
                = new MainForm();} 
            //else {
            //    if (e.Context.Name == TemplateContext.View) {e.Template = new 
            //        DetailViewForm();}
            //}
        }
    }
}
