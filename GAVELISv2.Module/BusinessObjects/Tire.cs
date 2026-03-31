using System;
using System.Text;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects {

    public enum TireCarryOutTypeEnum
    {
        None,
        Usable,
        [DisplayName("Recap")]
        OnRecap,
        [DisplayName("Regroove")]
        OnRegroove,
        [DisplayName("Repair")]
        OnRepair,
        [DisplayName("Scrap")]
        Scrapped,
        [DisplayName("For Sale")]
        ForSale
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("TireId")]
    public class Tire : XPObject {
        private Guid _RowID;
        private string _TireNo;
        private string _Description;
        private TireItem _TireItem;
        //private TireMake _Make;
        //private TireType _Type;
        private TireItemClassEnum _TireItemClass;
        private TirePhysicalStatusEnum _TireStatus;
        private Vendor _PurchaseFrom;
        private DateTime _PurchaseDate;
        private string _SerialNo;
        //private TireSize _Size;
        private string _InvoiceNo;
        private decimal _Cost;
        private RwsTireDetail _TireReqId;
        private bool _Inspected;
        private bool _ForInspection;
        private string _FindingsAndOperations;
        private DateTime _DateScrapped;
        private DateTime _DateSold;
        private GenJournalHeader _SoldRef;
        private DateTime _DateDeclared;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string TireId {
            get { return string.Format("{0:TR00000000}", Oid); }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [NonPersistent]
        [DisplayName("Prev. Branding")]
        public string PrevTireNo
        {
            get
            {
                try
                {
                    var distinctNos = this.TireServiceDetails2.Select(o => new { o.BrandingNo }).Distinct();
                    if (distinctNos != null && distinctNos.Count() > 1)
                    {
                        int n = distinctNos.Count();
                        return distinctNos.ElementAt(n - 2).BrandingNo;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
        //[RuleRequiredField("", DefaultContexts.Save)]
        //[RuleUniqueValue("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        //[EditorAlias("UpperCaseStringWinPropertyEditor")]
        [NonPersistent]
        public string TireNo {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.BrandingNo;
                    } else
                    {
                        return string.Empty;
                    }
                } catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        public WheelPosition LastWheelPos {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.WheelPos ?? null;
                    } else
                    {
                        return null;
                    }
                } catch (Exception)
                {
                    return null;
                }
            }
        }

        [NonPersistent]
        public TireDettachReason Condition {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.Reason ?? null;
                    } else
                    {
                        return null;
                    }
                } catch (Exception)
                {
                    return null;
                }
            }
        }

        [NonPersistent]
        [Size(500)]
        [DisplayName("Last Remarks")]
        public string Remarks {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.Remarks;
                    } else
                    {
                        return string.Empty;
                    }
                } catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        public DateTime FirstActivityDate
        {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).FirstOrDefault();
                    if (data != null)
                    {
                        return data.ActivityDate;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
                catch (Exception)
                {
                    return DateTime.MinValue;
                }
            }
        }

        [NonPersistent]
        public TireActivityTypeEnum FirstActivityType
        {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).FirstOrDefault();
                    if (data != null)
                    {
                        return data.ActivityType;
                    }
                    else
                    {
                        return TireActivityTypeEnum.None;
                    }
                }
                catch (Exception)
                {
                    return TireActivityTypeEnum.None;
                }
            }
        }
        [NonPersistent]
        public DateTime LastActivityDate {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.ActivityDate;
                    } else
                    {
                        return DateTime.MinValue;
                    }
                } catch (Exception)
                {
                    return DateTime.MinValue;
                }
            }
        }

        [NonPersistent]
        public TireActivityTypeEnum LastActivityType {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.ActivityType;
                    } else
                    {
                        return TireActivityTypeEnum.None;
                    }
                } catch (Exception)
                {
                    return TireActivityTypeEnum.None;
                }
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TireServiceDetail2 LastDetail {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data;
                    } else
                    {
                        return null;
                    }
                } catch (Exception)
                {
                    return null;
                }
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string LastBrandingNo {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.BrandingNo;
                    } else
                    {
                        return string.Empty;
                    }
                } catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string LastRecapped {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0 && o.Reason.Recapped).LastOrDefault();
                    if (data != null)
                    {
                        return data.Reason.Code;
                    } else
                    {
                        return string.Empty;
                    }
                } catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        [DisplayName("Serial/Branding No.")]
        [Custom("AllowEdit", "False")]
        public string SerialBranding {
            get
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        if (!string.IsNullOrEmpty(_SerialNo))
                        {
                            sb.AppendFormat("{0}/{1}", _SerialNo, data.BrandingNo);
                        } else
                        {
                            sb.Append(data.BrandingNo);
                        }
                        return sb.ToString();
                    } else
                    {
                        return string.Empty;
                    }
                } catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string DettachedStatus {
            get
            {
                // IN USE; 3RD BRANDING; R/GC/TT w/SR
                // TireStatus/LVL BrandingLevel/LastRecapped 
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0};", TireStatus);
                if (BrandingLevel > 0)
                {
                    sb.AppendFormat("LVL {0};", BrandingLevel);
                }
                if (!string.IsNullOrEmpty(LastRecapped))
                {
                    sb.AppendFormat("{0};", LastRecapped);
                }
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public FixedAsset LastFleet {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.Fleet ?? null;
                    } else
                    {
                        return null;
                    }
                } catch (Exception)
                {
                    return null;
                }
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal LastTreadDepth {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.TreadDepth;
                    } else
                    {
                        return 0m;
                    }
                } catch (Exception)
                {
                    return 0m;
                }
            }
        }

        [NonPersistent]
        public int BrandingLevel {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.TireNo).Where(o => o.Oid > 0 && o.Reason.Branding).LastOrDefault();
                    if (data != null)
                    {
                        return data.Reason.BrandingLevel;
                    } else
                    {
                        return 0;
                    }
                } catch (Exception)
                {
                    return 0;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public TireItem TireItem {
            get { return _TireItem; }
            set
            {
                SetPropertyValue<TireItem>("TireItem", ref _TireItem, value);
                if (!IsLoading && !IsSaving && _TireItem != null)
                {
                    Description = _TireItem.Description;
                }
            }
        }
        public TireItemClassEnum TireItemClass
        {
            get { return _TireItemClass; }
            set { SetPropertyValue("TireItemClass", ref _TireItemClass, value); }
        }
        
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>("Description", ref _Description, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        //public TireMake Make {
        //    get { return _Make; }
        //    set { SetPropertyValue<TireMake>("Make", ref _Make, value); }
        //}

        //[RuleRequiredField("", DefaultContexts.Save)]
        //public TireType Type {
        //    get { return _Type; }
        //    set { SetPropertyValue<TireType>("Type", ref _Type, value); }
        //}

        //public TireItemClassEnum TireItemClass
        //{
        //    get { return _TireItemClass; }
        //    set { SetPropertyValue<TireItemClassEnum>("TireItemClass", ref _TireItemClass, value); }
        //}

        public TirePhysicalStatusEnum TireStatus {
            get
            {
                try
                {
                    var data = this.TireServiceDetails2.OrderBy(o => o.Oid).LastOrDefault();
                    if (data != null && data.Reason.Code.Contains("SCRAP"))
                    {
                        return TirePhysicalStatusEnum.Scrap;
                    } else if (data != null && data.Reason.Code.Contains("SOLD"))
                    {
                        return TirePhysicalStatusEnum.Sold;
                    } else if (data != null && data.Reason.Code.Contains("LOST"))
                    {
                        return TirePhysicalStatusEnum.Lost;
                    } else if (data != null && data.Reason.Code.Contains("USABLE"))
                    {
                        return TirePhysicalStatusEnum.Available;
                    } else if (data != null && (!data.Reason.Code.Contains("SOLD") || !data.Reason.Code.Contains("SCRAP") || !data.Reason.Code.Contains("LOST") || !data.Reason.Code.Contains("USABLE")))
                    {
                        return TirePhysicalStatusEnum.InUse;
                    } else
                    {
                        return TirePhysicalStatusEnum.Available;
                    }
                } catch (Exception)
                {
                    return TirePhysicalStatusEnum.Available;
                }
            }
        }

        public Vendor PurchaseFrom {
            get { return _PurchaseFrom; }
            set { SetPropertyValue<Vendor>("PurchaseFrom", ref _PurchaseFrom, value); }
        }

        public DateTime PurchaseDate {
            get { return _PurchaseDate; }
            set { SetPropertyValue<DateTime>("PurchaseDate", ref _PurchaseDate, value); }
        }

        public string SerialNo {
            get { return _SerialNo; }
            set { SetPropertyValue<string>("SerialNo", ref _SerialNo, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        //public TireSize Size {
        //    get { return _Size; }
        //    set { SetPropertyValue<TireSize>("Size", ref _Size, value); }
        //}

        public string InvoiceNo {
            get { return _InvoiceNo; }
            set { SetPropertyValue<string>("InvoiceNo", ref _InvoiceNo, value); }
        }

        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue<decimal>("Cost", ref _Cost, value); }
        }

        [Custom("AllowEdit", "False")]
        public RwsTireDetail TireReqId {
            get { return _TireReqId; }
            set { SetPropertyValue<RwsTireDetail>("TireReqId", ref _TireReqId, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool Inspected {
            get { return _Inspected; }
            set { SetPropertyValue<bool>("Inspected", ref _Inspected, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool ForInspection {
            get { return _ForInspection; }
            set { SetPropertyValue<bool>("ForInspection", ref _ForInspection, value); }
        }

        //[NonPersistent]
        [Size(500)]
        [DisplayName("Findings/Recommendations")]
        public string FindingsAndOperations {
            get { return _FindingsAndOperations; }
            set { SetPropertyValue<string>("FindingsAndOperations", ref _FindingsAndOperations, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DateScrapped {
            get { return _DateScrapped; }
            set { SetPropertyValue<DateTime>("DateScrapped", ref _DateScrapped, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DateSold {
            get { return _DateSold; }
            set { SetPropertyValue<DateTime>("DateSold", ref _DateSold, value); }
        }

        [Custom("AllowEdit", "False")]
        public GenJournalHeader SoldRef {
            get { return _SoldRef; }
            set { SetPropertyValue<GenJournalHeader>("SoldRef", ref _SoldRef, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DateDeclared {
            get { return _DateDeclared; }
            set { SetPropertyValue<DateTime>("DateDeclared", ref _DateDeclared, value); }
        }

        private TireCarryOutTypeEnum _CarriedOut;
        [Custom("AllowEdit", "False")]
        public TireCarryOutTypeEnum CarriedOut {
            get { return _CarriedOut; }
            set { SetPropertyValue<TireCarryOutTypeEnum>("CarriedOut", ref _CarriedOut, value); }
        }

        private TireCarryOutTypeEnum _LastCarriedOut;
        [Custom("AllowEdit", "False")]
        public TireCarryOutTypeEnum LastCarriedOut {
            get { return _LastCarriedOut; }
            set { SetPropertyValue<TireCarryOutTypeEnum>("LastCarriedOut", ref _LastCarriedOut, value); }
        }

        //private TireToRetDetail _TireToRetDetailId;
        //[Custom("AllowEdit", "False")]
        //public TireToRetDetail TireToRetDetailId {
        //    get { return _TireToRetDetailId; }
        //    set { SetPropertyValue<TireToRetDetail>("TireToRetDetailId", ref _TireToRetDetailId, value); }
        //}

        [Aggregated,
        Association("Tire-ServiceDetails")]
        public XPCollection<TireServiceDetail> ServiceDetails {
            get { return
                GetCollection<TireServiceDetail>("ServiceDetails"); }
        }

        [Aggregated,
        Association("TireServiceDetails2")]
        public XPCollection<TireServiceDetail2> TireServiceDetails2 {
            get { return
                GetCollection<TireServiceDetail2>("TireServiceDetails2"); }
        }

        [Aggregated,
        Association("TireInspectionConditions")]
        public XPCollection<TireInsCondDetail> TireInspectionConditions {
            get { return GetCollection<TireInsCondDetail>("TireInspectionConditions"); }
        }

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }

        #endregion

        public Tire(Session session)
            : base(session) {
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            RowID = Guid.NewGuid();

            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }

            #endregion

        }

        protected override void OnSaving() {
            base.OnSaving();
            if (!IsDeleted && _ForInspection && !string.IsNullOrEmpty(_FindingsAndOperations))
            {
                ForInspection = false;
                Inspected = true;
            }

            #region Saving Modified

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                ModifiedBy = currentUser.UserName;
                ModifiedOn = DateTime.Now;
            }

            #endregion

        }

        protected override void OnDeleting() {
            if (TireStatus != TirePhysicalStatusEnum.Available)
            {
                throw new ApplicationException("This tire can no longer be deleted because there are already history details in it.");
            }
        }
    }
}
