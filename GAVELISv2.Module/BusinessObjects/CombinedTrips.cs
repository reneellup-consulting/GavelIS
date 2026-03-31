using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using System.ComponentModel;

namespace GAVELISv2.Module.BusinessObjects
{
    public struct vCombinedTrips
    {
        // OID
        [Persistent("Oid"), Browsable(false)]
        public int Oid_p { get; set; }
        // Customer
        [Persistent("Customer"), Browsable(false)]
        public Guid Customer_p { get; set; }
        // Tariff
        [Persistent("Tariff"), Browsable(false)]
        public Guid Tariff_p { get; set; }
        // Origin
        [Persistent("Origin"), Browsable(false)]
        public Guid Origin_p { get; set; }
        // Destination
        [Persistent("Destination"), Browsable(false)]
        public Guid Destination_p { get; set; }
        // DTRNo
        [Persistent("DTRNo"), Browsable(false)]
        public string DTRNo_p { get; set; }
        // ReferenceNo
        [Persistent("ReferenceNo"), Browsable(false)]
        public string ReferenceNo_p { get; set; }
        // SourceNo
        [Persistent("SourceNo"), Browsable(false)]
        public string SourceNo_p { get; set; }
        // SourceTable
        [Persistent("SourceTable"), Browsable(false)]
        public string SourceTable_p { get; set; }
    }

    [DefaultClassOptions]
    [Persistent("vCombinedTrips"), OptimisticLocking(false)]
    public class CombinedTrips : XPBaseObject
    {
        [Key, Persistent, Browsable(false)]
        public vCombinedTrips Key { get; set; }

        public int Oid { get { return Key.Oid_p; } }

        public GenJournalHeader GenJournalID
        {
            get
            {
                GenJournalHeader ret = null;
                if (Key.Oid_p != null)
                {
                    ret = Session.GetObjectByKey<GenJournalHeader>(Key.Oid_p);
                }
                return ret ?? null;
            }
        }

        // Customer
        public Customer Customer
        {
            get
            {
                Customer ret = null;
                if (Key.Customer_p != null)
                {
                    ret = Session.GetObjectByKey<Customer>(Key.Customer_p);
                }
                return ret ?? null;
            }
        }

        // Tariff
        public Tariff PayTariffee
        {
            get
            {
                Tariff ret = null;
                if (Key.Tariff_p != null)
                {
                    ret = Session.GetObjectByKey<Tariff>(Key.Tariff_p);
                }
                return ret ?? null;
            }
        }

        // Origin
        public TripLocation Origin
        {
            get
            {
                TripLocation ret = null;
                if (Key.Origin_p != null)
                {
                    ret = Session.GetObjectByKey<TripLocation>(Key.Origin_p);
                }
                return ret ?? null;
            }
        }

        // Destination
        public TripLocation Destination
        {
            get
            {
                TripLocation ret = null;
                if (Key.Destination_p != null)
                {
                    ret = Session.GetObjectByKey<TripLocation>(Key.Destination_p);
                }
                return ret ?? null;
            }
        }

        // DTRNo
        public string DTRNo { get { return Key.DTRNo_p; } }

        // ReferenceNo
        public string ReferenceNo { get { return Key.ReferenceNo_p; } }

        // SourceNo
        public string SourceNo { get { return Key.SourceNo_p; } }

        // SourceTable
        public string SourceTable { get { return Key.SourceTable_p; } }

        public CombinedTrips(Session session)
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
        }
    }

}
