using System;
using GAVELISv2.Module.Controllers;
using System.Windows.Forms;
using GAVELISv2.Module.Editors;

namespace GAVELISv2.Module.Win.Controllers {
    public class WinShowCustomFormWindowController : ShowCustomFormWindowController {
        protected override void ShowCustomForm() {
            Form form = DevExpress.Persistent.Base.ReflectionHelper.CreateObject(
                    "GAVELISv2.Win.Controls.BiometricDeviceForm") as Form;
            //Initializing a form when it is invoked from a controller.
            new XpoSessionAwareControlInitializer(form as IXpoSessionAwareControl, Application);
            form.ShowDialog();
        }
    }
}