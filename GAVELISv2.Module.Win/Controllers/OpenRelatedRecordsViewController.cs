using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Controls;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;

namespace GAVELISv2.Module.Win.Controllers
{
    public class OpenRelatedRecordsViewController : ViewController<DetailView>
    {
        Dictionary<LookupPropertyEditor, EventHandler> lookupPropertyEditorEventHandlers;
        Dictionary<LookupEdit, ButtonPressedEventHandler> lookupEditEventHandlers;

        protected override void OnActivated()
        {
            base.OnActivated();

            lookupPropertyEditorEventHandlers = new Dictionary<LookupPropertyEditor, EventHandler>();
            lookupEditEventHandlers = new Dictionary<LookupEdit, ButtonPressedEventHandler>();



            foreach (var item in View.Items.OfType<LookupPropertyEditor>())
            {

                var theItem = item;
                if (item.Control == null)
                {
                    EventHandler<EventArgs> controlCreated = null;

                    controlCreated = (s, e) =>
                    {
                        theItem.ControlCreated -= controlCreated;
                        AddOpenObjectButton(theItem);
                    };

                    item.ControlCreated += controlCreated;
                }
                else
                {
                    AddOpenObjectButton(item);
                }

            }
        }

        private void AddOpenObjectButton(LookupPropertyEditor item)
        {
            var openObjectButton = new EditorButton(ButtonPredefines.Ellipsis, CaptionHelper.GetLocalizedText("Controls/LookupPropertyEditorEx", "OpenButtonToolTip"),
            new SuperToolTip());

            if (item.PropertyValue == null)
            {
                openObjectButton.Enabled = false;
            }

            openObjectButton.Enabled = openObjectButton.Enabled && Frame.GetController<OpenObjectController>().Active.ResultValue;

            item.Control.Properties.Buttons.Insert(0, openObjectButton);
            item.Control.Properties.ActionButtonIndex = item.Control.Properties.Buttons.Count - 1;
            item.Control.Properties.NullText = CaptionHelper.DefaultNullValueText;

            EventHandler controlValueChanged = (s, e) =>
            {
                var openObjectController = this.Frame.GetController<OpenObjectController>();
                openObjectButton.Enabled = openObjectController.OpenObjectAction.Active.ResultValue && item.ControlValue != null;
            };

            item.ControlValueChanged += controlValueChanged;
            lookupPropertyEditorEventHandlers[item] = controlValueChanged;

            ButtonPressedEventHandler buttonClick = (s, e) =>
            {
                if (e.Button == openObjectButton)
                {
                    var openObjectController = this.Frame.GetController<OpenObjectController>();
                    openObjectController.OpenObjectAction.DoExecute();
                }
            };

            item.Control.ButtonClick += buttonClick;
            lookupEditEventHandlers[item.Control] = buttonClick;
        }

        protected override void OnDeactivated()
        {
            foreach (var pair in lookupPropertyEditorEventHandlers)
                pair.Key.ControlValueChanged -= pair.Value;

            lookupPropertyEditorEventHandlers.Clear();


            foreach (var pair in lookupEditEventHandlers)
                pair.Key.ButtonClick -= pair.Value;

            lookupEditEventHandlers.Clear();

            base.OnDeactivated();
        }
    }
}
