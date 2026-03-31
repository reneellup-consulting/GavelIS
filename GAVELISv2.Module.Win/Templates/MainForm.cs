using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp;
namespace GAVELISv2.Module.Win
{
    public partial class MainForm : MainFormTemplateBase, IDockManagerHolder, 
    ISupportMdiContainer, ISupportClassicToRibbonTransform, ICaptionPanelHolder, 
    IInfoPanelTemplate {
        public override void SetSettings(IModelTemplate modelTemplate) {
            base.SetSettings(modelTemplate);
            navigation.Model = TemplatesHelper.GetNavBarCustomizationNode();
            formStateModelSynchronizerComponent.Model = GetFormStateNode();
            modelSynchronizationManager.ModelSynchronizableComponents.Add(new 
            NavigationModelSynchronizer(dockPanelNavigation, (IModelTemplateWin)
            modelTemplate));
            modelSynchronizationManager.ModelSynchronizableComponents.Add(
            navigation);
        }
        protected virtual void InitializeImages() {
            barMdiChildrenListItem.Glyph = ImageLoader.Instance.GetImageInfo(
            "Action_WindowList").Image;
            barMdiChildrenListItem.LargeGlyph = ImageLoader.Instance.
            GetLargeImageInfo("Action_WindowList").Image;
            barSubItemPanels.Glyph = ImageLoader.Instance.GetImageInfo(
            "Action_Navigation").Image;
            barSubItemPanels.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo
            ("Action_Navigation").Image;
        }
        [Obsolete("Use the MainForm() constructor"),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)]
        public MainForm(XafApplication application): this() { }
        public MainForm() {
            InitializeComponent();
            InitializeImages();
            viewSiteManager.ViewSiteControl = viewSitePanel;
            UpdateMdiModeDependentProperties();
            //if (TemplateCreated != null)
            //{
            //    TemplateCreated(this, EventArgs.Empty);
            //}
        }
        public Bar ClassicStatusBar { get { return _statusBar; } }
        public DockPanel DockPanelNavigation { get { return dockPanelNavigation; 
            } }
        public DockManager DockManager { get { return mainDockManager; } }
        protected override void UpdateMdiModeDependentProperties() {
            base.UpdateMdiModeDependentProperties();
            bool isMdi = UIMode == UIMode.Mdi;
            viewSiteControlPanel.Visible = !isMdi;
            //viewSitePanel.Visible = !isMdi;
            //Do not replace with ? operator (problems with convertion to VB)
            if (isMdi) {
                barSubItemWindow.Visibility = BarItemVisibility.Always;
                barMdiChildrenListItem.Visibility = BarItemVisibility.Always;
            } else {
                barSubItemWindow.Visibility = BarItemVisibility.Never;
                barMdiChildrenListItem.Visibility = BarItemVisibility.Never;
            }
        }
        //public static event EventHandler<EventArgs> TemplateCreated;
        #region ICaptionPanelHolder Members
        public DevExpress.Utils.Frames.ApplicationCaption8_1 CaptionPanel { get 
            { return captionPanel; } }
        #endregion
        #region IInfoPanelTemplate Members
        public DevExpress.XtraEditors.SplitContainerControl SplitContainer { get 
            { return splitContainerControl; } }
        #endregion
    }
    public interface IInfoPanelTemplate : IFrameTemplate {
        DevExpress.XtraEditors.SplitContainerControl SplitContainer { get; }
    }
    public interface ICaptionPanelHolder {
        DevExpress.Utils.Frames.ApplicationCaption8_1 CaptionPanel { get; }
    }
    [System.ComponentModel.DisplayName("GAVEL I.S. MainForm Template")]
    public class GAVELISMainFormTemplateLocalizer : FrameTemplateLocalizer<
    MainForm> {
    }
}
