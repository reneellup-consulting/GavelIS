using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class TOFuel : XPObject {
        private TruckingOperation _TOID;
        private FATruck _Fleet;
        private Contact _Account;
        private GenJournalHeader _ReferenceNo;
        private DateTime _Date;
        private decimal _Amount;
        [Custom("AllowEdit", "False")]
        [Association("TruckingOperation-Fuels")]
        public TruckingOperation TOID {
            get { return _TOID; }
            set { SetPropertyValue("TOID", ref _TOID, value); }
        }
        [Custom("AllowEdit", "False")]
        public FATruck Fleet {
            get { return _Fleet; }
            set { SetPropertyValue("Fleet", ref _Fleet, value); }
        }

        [Custom("AllowEdit", "False")]
        public Contact Account {
            get { return _Account; }
            set { SetPropertyValue("Account", ref _Account, value); }
        }

        [Custom("AllowEdit", "False")]
        public GenJournalHeader ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Amount {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value);
            if (!IsLoading)
            {
                _TOID.UpdateExpensesFromFuel(true);
            }
            }
        }

        public TOFuel(Session session): base(session) {
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

        #region Get Current User

        private SecurityUser _CurrentUser;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser
        {
            get
            {
                if (SecuritySystem.CurrentUser != null)
                {
                    _CurrentUser = Session.GetObjectByKey<SecurityUser>(
                    Session.GetKeyValue(SecuritySystem.CurrentUser));
                }
                return _CurrentUser;
            }
        }

        #endregion

    }

}
