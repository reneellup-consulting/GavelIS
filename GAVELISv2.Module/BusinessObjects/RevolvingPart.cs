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
namespace GAVELISv2.Module.BusinessObjects {
    public enum RevolvingPartsStatusEnum
    {
        Available,
        Attached,
        Dettached,
        Scrap
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("PartNo")]
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save,"Prefix, No")]
    public class RevolvingPart : XPObject {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        private RevolvingPartsCategory _Category;
        private string _Prefix;
        private string _No;
        private string _PartNo;
        private Item _ItemId;
        private string _Description;
        private string _SerialNo;
        private Vendor _PurchaseFrom;
        private DateTime _PurchaseDate;
        private string _InvoiceNo;
        private decimal _Cost;
        private RevolvingPartsCondition _Condition;
        private RevolvingPartDetail _LastDetail;
        private RevolvingPartDetail _LastDettachDetail;
        private RevolvingPartDetail _LastAttachDetail;
        private Warehouse _CurrentLocation;
        private JobOrder _LastJobsId;
        private WorkOrder _LastWorkOrderId;
        private DateTime _DateDisposed;
        private PhysicalAdjustment _DisposalDocId;
        private DateTime _DateSold;
        private Invoice _SoldDocId;
        private Requisition _ReqId;
        private RequisitionWorksheet _ReqWorksheetId;
        private string _Remarks;
        [RuleRequiredField("", DefaultContexts.Save)]
        public RevolvingPartsCategory Category
        {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value);
            if (!IsSaving && !IsLoading && _Category!=null)
            {
                Prefix = _Category.Prefix;
                XPCollection<RevolvingPart> revs = new XPCollection<RevolvingPart>(Session);
                var data = revs.OrderBy(o => o.Category).ThenBy(o=>o.No).Where(o => o.Category==_Category && o.Oid > 0).LastOrDefault();
                if (data != null)
                {
                    No = GetNewNo(data.No);
                }
                else
                {
                    No = "1000";
                }
            }
            }
        }
        [Custom("AllowEdit", "False")]
        public string Prefix
        {
            get { return _Prefix; }
            set { SetPropertyValue("Prefix", ref _Prefix, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [NonCloneable]
        public string No
        {
            get { return _No; }
            set { SetPropertyValue("No", ref _No, value); }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public string PartNo
        {
            get { return string.Format("{0}-{1}",_Prefix,_No); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Item")]
        public Item ItemId
        {
            get { return _ItemId; }
            set { SetPropertyValue("ItemId", ref _ItemId, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        public string SerialNo
        {
            get { return _SerialNo; }
            set { SetPropertyValue("SerialNo", ref _SerialNo, value); }
        }
        public Vendor PurchaseFrom
        {
            get { return _PurchaseFrom; }
            set { SetPropertyValue("PurchaseFrom", ref _PurchaseFrom, value); }
        }
        public DateTime PurchaseDate
        {
            get { return _PurchaseDate; }
            set { SetPropertyValue("PurchaseDate", ref _PurchaseDate, value); }
        }
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set { SetPropertyValue("InvoiceNo", ref _InvoiceNo, value); }
        }
        public decimal Cost
        {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public RevolvingPartsCondition Condition
        {
            get
            {
                if (LastDetail != null)
                {
                    return LastDetail.Condition??null;
                }
                else
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public RevolvingPartDetail LastDetail
        {
            get {
                try
                {
                    var data = this.RevolvingPartsDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data ?? null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        private FixedAsset _LastUnit;
        private DateTime _LastActivityDate;
        private RevolvingPartActivityTypeEnum _LastActivityType;
        private FixedAsset _LastUnitAttachedTo;
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public DateTime LastActivityDate
        {
            get
            {
                if (LastDetail != null)
                {
                    return LastDetail.ActivityDate;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public RevolvingPartActivityTypeEnum LastActivityType
        {
            get
            {
                if (LastDetail != null)
                {
                    return LastDetail.ActivityType;
                }
                else
                {
                    return RevolvingPartActivityTypeEnum.None;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public FixedAsset LastUnitAttachedTo
        {
            get
            {
                try
                {
                    var data = this.RevolvingPartsDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == RevolvingPartActivityTypeEnum.Attach).LastOrDefault();
                    if (data != null)
                    {
                        return data.Unit ?? null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public FixedAsset LastUnit
        {
            get {
                if (LastDetail != null)
                {
                    return LastDetail.Unit ?? null;
                }
                else
                {
                    return null;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public RevolvingPartDetail LastDettachDetail
        {
            get
            {
                try
                {
                    var data = this.RevolvingPartsDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == RevolvingPartActivityTypeEnum.Dettach).LastOrDefault();
                    if (data != null)
                    {
                        return data ?? null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public RevolvingPartDetail LastAttachDetail
        {
            get
            {
                try
                {
                    var data = this.RevolvingPartsDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == RevolvingPartActivityTypeEnum.Attach).LastOrDefault();
                    if (data != null)
                    {
                        return data ?? null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public Warehouse CurrentLocation
        {
            get
            {
                if (LastDetail != null)
                {
                    return LastDetail.Location ?? null;
                }
                else
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public JobOrder LastJobsId
        {
            get
            {
                try
                {
                    var data = this.RevolvingPartsDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == RevolvingPartActivityTypeEnum.Dettach && o.JobOrderDoc != null).LastOrDefault();
                    if (data != null)
                    {
                        return data.JobOrderDoc ?? null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public WorkOrder LastWorkOrderId
        {
            get
            {
                try
                {
                    var data = this.RevolvingPartsDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == RevolvingPartActivityTypeEnum.Dettach && o.WorkOrderDoc != null).LastOrDefault();
                    if (data != null)
                    {
                        return data.WorkOrderDoc ?? null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public DateTime DateDisposed
        {
            get
            {
                RevolvingPartsActivityReason findObject = Session.FindObject<RevolvingPartsActivityReason>(BinaryOperator.Parse("[Code]=?", "DISPOSAL"));
                if (findObject != null)
                {
                    var data = this.RevolvingPartsDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.Reason == findObject).LastOrDefault();
                    if (data != null)
                    {
                        return data.ActivityDate;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Disposal Doc.")]
        public PhysicalAdjustment DisposalDocId
        {
            get
            {
                RevolvingPartsActivityReason findObject = Session.FindObject<RevolvingPartsActivityReason>(BinaryOperator.Parse("[Code]=?", "DISPOSAL"));
                if (findObject != null)
                {
                    var data = this.RevolvingPartsDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.Reason == findObject).LastOrDefault();
                    if (data != null)
                    {
                        return data.AdjustmentDoc;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public DateTime DateSold
        {
            get
            {
                InvoiceDetail invDet = Session.FindObject<InvoiceDetail>(BinaryOperator.Parse("[RevPartSoldId]=?", this));
                if (invDet != null)
                {
                    return invDet.GenJournalID.EntryDate;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public Invoice SoldDocId
        {
            get
            {
                InvoiceDetail invDet = Session.FindObject<InvoiceDetail>(BinaryOperator.Parse("[RevPartSoldId]=?", this));
                if (invDet != null)
                {
                    return invDet.GenJournalID as Invoice;
                }
                else
                {
                    return null;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public Requisition ReqId
        {
            get { return _ReqId; }
            set { SetPropertyValue("ReqId", ref _ReqId, value); }
        }
        [Custom("AllowEdit", "False")]
        public RequisitionWorksheet ReqWorksheetId
        {
            get { return _ReqWorksheetId; }
            set { SetPropertyValue("ReqWorksheetId", ref _ReqWorksheetId, value); }
        }
        [Size(500)]
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public string Remarks
        {
            get
            {
                if (LastDetail != null)
                {
                    return LastDetail.Remarks;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        private string _AvailabilityStatus;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string AvailabilityStatus
        {
            get
            {
                if (LastDetail != null)
                {
                    switch (LastDetail.Status)
                    {
                        case RevolvingPartsStatusEnum.Available:
                            return "AVAILABLE";
                        case RevolvingPartsStatusEnum.Attached:
                            return "CURRENTLY ATTACHED";
                        case RevolvingPartsStatusEnum.Dettached:
                            return "CURRENTLY DETTACHED";
                        case RevolvingPartsStatusEnum.Scrap:
                            if (DateSold != DateTime.MinValue)
                            {
                                return "ALREADY SOLD";
                            }
                            else
                            {
                                return "ALREADY SCRAPPED";
                            }
                            
                        default:
                            return "AVAILABLE";
                    }
                }
                else
                {
                    return "AVAILABLE";
                }
            }
        }
        
        [Aggregated,
        Association("RevolvingPartsDetails")]
        public XPCollection<RevolvingPartDetail> RevolvingPartsDetails
        {
            get
            {
                return
                    GetCollection<RevolvingPartDetail>("RevolvingPartsDetails");
            }
        }
        private string GetNewNo(string lastNo)
        {
            string seqNo;
            string incNo;
            int inc = 1;
            if (!string.IsNullOrEmpty(lastNo))
            {
                seqNo = lastNo
                    ;
            }
            else
            {
                seqNo = "1000";
            }
            string digits = "0123456789";
            string defaultFormat = "{0:D5}";
            string formatString = string.Empty;
            string num = string.Empty;
            int c = 0;
            int i, x;
            i = x = seqNo.LastIndexOfAny(digits.ToCharArray());
            while (i >= 0 && isDigit(seqNo[i]))
            {
                num = seqNo[i] + num;
                c++;
                i--;
            }
            int n = int.Parse(num) + inc;
            formatString = defaultFormat.Replace("5", c.ToString());
            incNo = string.Format(formatString, n);
            x = x + 1 - num.Length;
            seqNo = seqNo.Remove(x, num.Length);
            seqNo = seqNo.Insert(x, string.Empty + incNo);
            lastNo = seqNo;
            return seqNo;
        }
        private static bool isDigit(char c)
        {
            string digits = "0123456789";
            return digits.IndexOf(c) == -1 ? false : true;
        }
        #region Records Creation

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

        #endregion
        public RevolvingPart(Session session): base(session) {
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

        protected override void OnSaving()
        {
            base.OnSaving();


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
    }
}
