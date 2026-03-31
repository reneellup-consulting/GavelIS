using System;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class TransferOrder : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private TransferOrderStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private DateTime _DateReceived;
        private Warehouse _FromWarehouse;
        private Warehouse _ToWarehouse;
        private FixedAsset _TransportVehicle;
        private Employee _Driver;
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }

        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }

        [Size(500)]
        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }

        public TransferOrderStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != TransferOrderStatusEnum.Current)
                    {
                        Approved =
                        true;
                    } else
                    {
                        Approved = false;
                    }
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }

        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        public DateTime DateReceived {
            get { return _DateReceived; }
            set { SetPropertyValue("DateReceived", ref _DateReceived, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Warehouse FromWarehouse {
            get { return _FromWarehouse; }
            set { SetPropertyValue("FromWarehouse", ref _FromWarehouse, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Warehouse ToWarehouse {
            get { return _ToWarehouse; }
            set { SetPropertyValue("ToWarehouse", ref _ToWarehouse, value); }
        }
        [DataSourceCriteria("[FixedAssetClass] In ('Truck', 'OtherVehicle')")]
        public FixedAsset TransportVehicle {
            get { return _TransportVehicle; }
            set
            {
                SetPropertyValue<FixedAsset>("TransportVehicle", ref _TransportVehicle, value);
                if (!IsLoading && _TransportVehicle != null)
                {
                    Driver = _TransportVehicle.DriverOperator ?? null;
                }
            }
        }

        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue<Employee>("Driver", ref _Driver, value); }
        }

        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }

        public TransferOrder(Session session)
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "TO"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "TO"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "TO"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
        }

        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Transfer Order transactions."
                );
            }
        }
    }
}
