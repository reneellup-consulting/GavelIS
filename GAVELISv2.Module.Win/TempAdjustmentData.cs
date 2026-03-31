using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win
{
    public class TempAdjustmentData
    {
        private BusinessObjects.Item _Item;
        private PhysicalAdjustment _Document;
        private DateTime _DocumentDate;
        private BusinessObjects.Warehouse _Warehouse;
        private decimal _ActualCount=0;
        private UnitOfMeasure _Unit;
        private UnitOfMeasure _StockUnit;
        private decimal _Factor=0;
        // Item
        public Item Item
        {
            get { return _Item; }
            set { this._Item = value; }
        }
        // Document
        public PhysicalAdjustment Document
        {
            get { return _Document; }
            set { this._Document = value; }
        }
        // DocumentDate
        public DateTime DocumentDate
        {
            get { return _DocumentDate; }
            set { this._DocumentDate = value; }
        }
        // Warehouse
        public Warehouse Warehouse
        {
            get { return _Warehouse; }
            set { this._Warehouse = value; }
        }
        // ActualCount
        public decimal ActualCount
        {
            get { return _ActualCount; }
            set { this._ActualCount = value; }
        }
        // Unit
        public UnitOfMeasure Unit
        {
            get { return _Unit; }
            set { this._Unit = value; }
        }
        // StockUnit
        public UnitOfMeasure StockUnit
        {
            get { return _StockUnit; }
            set { this._StockUnit = value; }
        }
        // Factor
        public decimal Factor
        {
            get { return _Factor; }
            set { this._Factor = value; }
        }
    }

    public class TempAdjustmentDataCollection : BaseBindingList<TempAdjustmentData>
    {
    }
}
