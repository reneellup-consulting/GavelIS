using System;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
namespace GAVELISv2.Module.BusinessObjects {
    [DomainComponent]
    [NonPersistent]
    public class ReportGenerate : INotifyPropertyChanged {
        
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) { if (
            PropertyChanged != null) {PropertyChanged(this, new 
                PropertyChangedEventArgs(propertyName));} }
        #endregion
    }
}
