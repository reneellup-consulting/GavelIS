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
    public class FuelEfficiencyMon : XPObject {
        private Guid _RowID;
        private ReceiptFuel _RecFuelID;
        private DateTime _EntryDate;
        private FATruck _TruckNo;
        private Employee _Driver;
        private FEMEntryTypeEnum _EntryType;
        private OdometerRegister _OdoReadID;
        private decimal _Adjustment;
        private decimal _OdoRead;
        private decimal _Kms;
        private decimal _Liters;
        private decimal _Cost;
        private decimal _KmsPerLiter;
        private decimal _Efficiency;
        private decimal _CostPerKm;
        private decimal _Life;
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        public ReceiptFuel RecFuelID {
            get { return _RecFuelID; }
            set { SetPropertyValue("RecFuelID", ref _RecFuelID, value); }
        }
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        public FATruck TruckNo {
            get { return _TruckNo; }
            set { SetPropertyValue("TruckNo", ref _TruckNo, value); }
        }
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        public FEMEntryTypeEnum EntryType {
            get { return _EntryType; }
            set { SetPropertyValue("EntryType", ref _EntryType, value); }
        }
        public OdometerRegister OdoReadID {
            get { return _OdoReadID; }
            set { SetPropertyValue("OdoReadID", ref _OdoReadID, value); }
        }
        public decimal Adjustment {
            get { return _Adjustment; }
            set { SetPropertyValue("Adjustment", ref _Adjustment, value); }
        }
        public decimal OdoRead {
            get { return _OdoRead; }
            set { SetPropertyValue("OdoRead", ref _OdoRead, value); }
        }
        public decimal Kms {
            get { return _Kms; }
            set { SetPropertyValue("Kms", ref _Kms, value); }
        }
        public decimal Liters {
            get { return _Liters; }
            set { SetPropertyValue("Liters", ref _Liters, value); }
        }
        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        public decimal KmsPerLiter {
            get { return _KmsPerLiter; }
            set { SetPropertyValue("KmsPerLiter", ref _KmsPerLiter, value); }
        }
        public decimal Efficiency {
            get { return _Efficiency; }
            set { SetPropertyValue("Efficiency", ref _Efficiency, value); }
        }
        public decimal CostPerKm {
            get { return _CostPerKm; }
            set { SetPropertyValue("CostPerKm", ref _CostPerKm, value); }
        }
        public decimal Life {
            get { return _Life; }
            set { SetPropertyValue("Life", ref _Life, value); }
        }
        public FuelEfficiencyMon(Session session): base(session) {
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
            RowID = Guid.NewGuid();
        }
    }
}
