using System;
using System.Collections;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class FAGeneratorSet : FixedAsset {
        private string _SeriesNo;
        private bool _NotOwned;
        private Employee _Operator;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        public string SeriesNo {
            get { return _SeriesNo; }
            set { SetPropertyValue("SeriesNo", ref _SeriesNo, value); }
        }

        public bool NotOwned {
            get { return _NotOwned; }
            set { SetPropertyValue("NotOwned", ref _NotOwned, value); }
        }

        public Employee Operator {
            get { return _Operator; }
            set { SetPropertyValue<Employee>("Operator", ref _Operator, value); }
        }
        [Aggregated,
        Association("GensetHoursLogs")]
        public XPCollection<GensetHoursRegister> GensetHoursLogs
        {
            get
            {
                return GetCollection<GensetHoursRegister>("GensetHoursLogs"
                    );
            }
        }
        #region Get Last Fuel Receipt

        public ReceiptFuel GetLastFuelReceipt() {
            ICollection receiptsFuel;
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            DevExpress.Xpo.Metadata.XPClassInfo receiptFuelClass;
            receiptFuelClass = Session.GetClassInfo(typeof(ReceiptFuel));
            criteria = CriteriaOperator.Parse("[GensetNo.No] = '" + this.No +
            "'");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("EntryDate", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            receiptsFuel = Session.GetObjects(receiptFuelClass, criteria,
            sortProps, 0, false, true);
            ReceiptFuel rcpt = null;
            foreach (ReceiptFuel item in receiptsFuel)
            {
                rcpt = item;
            }
            return rcpt;
        }

        #endregion

        public FAGeneratorSet(Session session)
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
            FixedAssetClass = FixedAssetClassEnum.GeneratorSet;
        }
    }
}
