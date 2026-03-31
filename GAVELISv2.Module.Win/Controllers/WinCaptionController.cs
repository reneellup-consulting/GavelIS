using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Utils.Frames;
namespace GAVELISv2.Module.Win
{
    public class WinCaptionController : ViewController {
        protected override void OnActivated() {
            base.OnActivated();
            if (Frame.Template is ICaptionPanelHolder) {
                ApplicationCaption8_1 captionPanel = ((ICaptionPanelHolder)Frame
                .Template).CaptionPanel;
                captionPanel.Text = View.Caption;
                captionPanel.Visible = !string.IsNullOrEmpty(captionPanel.Text);
            }
        }
        public WinCaptionController() { this.TargetViewNesting = Nesting.Root; }
    }
}
