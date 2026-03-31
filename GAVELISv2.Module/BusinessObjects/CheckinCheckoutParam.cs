using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Editors;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NonPersistent]
    public class CheckinCheckoutParam : XPObject {
        private string _SourceFile;
        public string SourceFile {
            get { return _SourceFile; }
            set { SetPropertyValue<string>("SourceFile", ref _SourceFile, value); }
        }

        public CheckinCheckoutParam(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }
    }

}
