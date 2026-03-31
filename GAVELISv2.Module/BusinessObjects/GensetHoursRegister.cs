using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    [FriendlyKeyProperty("Sequence")]
    public class GensetHoursRegister : XPObject
    {
        private Guid _RowID;
        private string _Sequence;
        private DateTime _EntryDate;
        private FAGeneratorSet _Genset;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Sequence
        {
            get { return _Sequence; }
            set { SetPropertyValue("Sequence", ref _Sequence, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set
            {
                SetPropertyValue("EntryDate", ref _EntryDate, value);
                if (!IsLoading)
                {
                    string seq = _EntryDate != DateTime.MinValue ?
                       string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", _EntryDate.Year, _EntryDate.Month,
                       _EntryDate.Day, _EntryDate.Hour, _EntryDate.Minute, _EntryDate.Second, EntryReference != null ? EntryReference.Oid > 0 ? EntryReference.Oid : 0 : 0)
                       : string.Empty;
                     var data = Genset.GensetHoursLogs.Where(o => o.Sequence == seq).LastOrDefault();
                     if (data != null)
                     {
                         Sequence = seq;
                     }
                     else
                     {
                         DateTime dt = _EntryDate.AddSeconds(1);
                         Sequence = dt != DateTime.MinValue ?
                        string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", dt.Year, dt.Month,
                        dt.Day, dt.Hour, dt.Minute, dt.Second, EntryReference != null ? EntryReference.Oid > 0 ? EntryReference.Oid : 0 : 0)
                        : string.Empty;
                     }
                    ProcessRead();
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Association("GensetHoursLogs")]
        public FAGeneratorSet Genset
        {
            get { return _Genset; }
            set { SetPropertyValue("Genset", ref _Genset, value); }
        }
        public void ProcessRead()
        {
            if (!IsLoading)
            {
                #region New Trip Odo Logging
                if (Genset != null)
                {
                    decimal toDecimal = Convert.ToDecimal(Sequence);
                    var data3 = Genset.GensetHoursLogs.OrderBy(o => o.Sequence).Where(o => o.Genset == Genset && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                    decimal lstHours = data3 != null ? data3.LifeHours : 0m;
                    LifeHours = lstHours + Reading;
                    this.Save();
                    //Session.CommitTransaction();

                    //decimal toDecimal = Convert.ToDecimal(delSequenceNo);
                    var data = Genset.GensetHoursLogs.OrderBy(o => o.Sequence).Where(o => o.Genset == Genset && Convert.ToDecimal(o.Sequence) > toDecimal).Select(o => o);
                    if (data != null && data.Count() > 0)
                    {
                        int n = 0;
                        foreach (var item in data)
                        {
                            decimal beforeSeq = Convert.ToDecimal(item.Sequence);
                            GensetHoursRegister before = Genset.GensetHoursLogs.OrderBy(o => o.Sequence).Where(o => o.Genset == Genset
                                && o.Sequence != _Sequence && Convert.ToDecimal(o.Sequence) < beforeSeq).LastOrDefault();
                            if (before != null && n == 0)
                            {
                                item.LifeHours = LifeHours + item.Reading;
                                item.Save();
                            }
                            else if (before != null && n > 0)
                            {
                                item.LifeHours = before.LifeHours + item.Reading;
                                item.Save();
                            }
                            else if (before == null)
                            {
                                item.LifeHours = LifeHours + item.Reading;
                                item.Save();
                            }
                            n++;
                        }
                    }

                }
                #endregion
            }
        }
        private GensetEntry _EntryReference;
        [Custom("AllowEdit", "False")]
        [System.ComponentModel.DisplayName("Ref #")]
        [Association("GensetEntryHoursLogs")]
        public GensetEntry EntryReference
        {
            get { return _EntryReference; }
            set { SetPropertyValue("EntryReference", ref _EntryReference, value);
            
            }
        }
        // Reading
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Reading
        {
            get { return _Reading; }
            set
            {
                SetPropertyValue("Reading", ref _Reading, value);
                if (!IsLoading)
                {
                    ProcessRead();
                }
            }
        }
        // LifeHours
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LifeHours
        {
            get { return _LifeHours; }
            set
            {
                SetPropertyValue("LifeHours", ref _LifeHours, value);
            }
        }
        //[Custom("AllowEdit", "False")]
        //[EditorAlias("MeterTypePropertyEditor")]
        //public decimal LifeHours
        //{
        //    get { return _LifeHours; }
        //}

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }
        public GensetHoursRegister(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            RowID = Guid.NewGuid();
            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(Session.
                GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }
        }
        protected override void OnSaving()
        {
            base.OnSaving();
            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(Session.
                GetKeyValue(SecuritySystem.CurrentUser));
                ModifiedBy = currentUser.UserName;
                ModifiedOn = DateTime.Now;
            }
        }
        #region Get Current User

        private SecurityUser _CurrentUser;
        private decimal _LifeHours;
        private decimal _Reading;
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
        private string delSequenceNo;
        protected override void OnDeleting()
        {
            if (_EntryReference != null)
            {
                throw new UserFriendlyException("Cannot delete a entry register generated hours log");
            }
            delSequenceNo = this.Sequence;
            decimal toDecimal = Convert.ToDecimal(delSequenceNo);
            var data = _Genset.GensetHoursLogs.OrderBy(o => o.Sequence).Where(o => o.Genset == _Genset && Convert.ToDecimal(o.Sequence) > toDecimal).Select(o=>o);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data)
                {
                    // Get last life
                    decimal beforeSeq = Convert.ToDecimal(item.Sequence);
                    GensetHoursRegister before = _Genset.GensetHoursLogs.OrderBy(o => o.Sequence).Where(o => o.Genset == _Genset 
                        && o.Sequence != _Sequence && Convert.ToDecimal(o.Sequence) < beforeSeq).LastOrDefault();
                    if (before != null)
                    {
                        item.LifeHours = before.LifeHours + item.Reading;
                        item.Save();
                    }
                    else
                    {
                        item.LifeHours = item.Reading;
                        item.Save();
                    }
                }
            }
            base.OnDeleting();
        }
    }

}
