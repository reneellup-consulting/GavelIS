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
namespace GAVELISv2.Module.BusinessObjects
{
    [DomainComponent]
    [NonPersistent]
    public class AdjustItemCostPrices : INotifyPropertyChanged
    {
        private string _ItemsFilter;
        private decimal _MarkupRate;
        private string _TemplateFilePath;
        private AdjustItemCostPricesEnum _AdjustMode;
        public AdjustItemCostPricesEnum AdjustMode
        {
            get { return _AdjustMode; }
            set
            {
                if (_AdjustMode == value) { return; }
                _AdjustMode = value;
                NotifyPropertyChanged("AdjustMode");
            }
        }
        public string TemplateFilePath
        {
            get { return _TemplateFilePath; }
            set
            {
                if (_TemplateFilePath == value) { return; }
                _TemplateFilePath = value;
                NotifyPropertyChanged("TemplateFilePath");
            }
        }
        private Type ObjectType { get { return typeof(Item); } }
        [CriteriaObjectTypeMember("ObjectType"), Size(-1), ImmediatePostData]
        public string ItemsFilter
        {
            get { return _ItemsFilter; }
            set
            {
                if (_ItemsFilter == value) return;
                _ItemsFilter = value;
                NotifyPropertyChanged("ItemsFilter");
            }
        }
        public decimal MarkupRate
        {
            get { return _MarkupRate; }
            set
            {
                if (_MarkupRate == value) { return; }
                _MarkupRate = value;
                NotifyPropertyChanged("MarkupRate");
            }
        }
        public string CriterionString
        {
            get { return ItemsFilter; }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (
                PropertyChanged != null)
            {
                PropertyChanged(this, new
                    PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
