using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win
{
    public class UomBuffer
    {
        private UnitOfMeasure _UnitOfMeasure;
        private int _Count;
        public UnitOfMeasure UnitOfMeasure
        {
            get { return _UnitOfMeasure; }
            set { this._UnitOfMeasure = value; }
        }
        public int Count
        {
            get { return _Count; }
            set { this._Count = value; }
        }
    }
    // For UnitOfMeasure field in Purchase Movement Summary
    public class UomBufferCollection : BaseBindingList<UomBuffer>
    {
    }
}
