using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.Data.Filtering;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class FuelRegisterViewController : ViewController<ListView>
    {
        private ListViewProcessCurrentObjectController processCurrentController;
        private DevExpress.Data.Filtering.CriteriaOperator criteria;
        public FuelRegisterViewController()
        {
            this.TargetObjectType = typeof(FuelRegister);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            processCurrentController = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (processCurrentController != null)
            {
                processCurrentController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentController_CustomProcessSelectedItem);
            }
        }

        void processCurrentController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
            var selectedObject = e.InnerArgs.CurrentObject;

            // Your logic to get the related object
            object relatedObject = GetRelatedObject(selectedObject);

            if (relatedObject != null)
            {
                ListViewProcessCurrentObjectController.ShowObject(
                    relatedObject,
                    e.InnerArgs.ShowViewParameters,
                    Application,
                    Frame,
                    View
                );
            }
        }

        private object GetRelatedObject(object selectedObject)
        {
            IObjectSpace objs = Application.CreateObjectSpace();
            FuelRegister fr = selectedObject as FuelRegister;
            if (fr != null)
            {
                // TODO: If cannot find the criteria then notify the user
                CriteriaOperator criteria = CriteriaOperator.Parse("[Oid] = ?", fr.SourceID);
                ReceiptFuel receipt = objs.FindObject<ReceiptFuel>(criteria);
                if (receipt != null)
                {
                    return receipt;
                }
                else
                {
                    // Show user-friendly message
                    throw new UserFriendlyException(
                        "No ReceiptFuel record was found for this Fuel Register."
                    );
                }
            }
            return null;
        }

        protected override void OnDeactivated()
        {
            if (processCurrentController != null)
            {
                processCurrentController.CustomProcessSelectedItem -= processCurrentController_CustomProcessSelectedItem;
            }
            base.OnDeactivated();
        }
    }
}
