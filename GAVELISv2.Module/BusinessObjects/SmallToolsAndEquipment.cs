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

namespace GAVELISv2.Module.BusinessObjects
{
    public enum SmallToolsAndEquipmentStatusEnum
    {
        NoHistory,
        Available,
        Reserved,
        CheckedOut,
        UnderRepair,
        Sold,
        Lost,
        ForDisposal
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("ToolNo")]
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "Prefix, No")]
    public class SmallToolsAndEquipment : XPObject
    {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        private SmallToolsAndEquipmentCategory _Category;
        private string _Prefix;
        private string _No;
        private string _ToolNo;
        private Item _ItemId;
        private string _Description;
        private string _SerialNo;
        private Vendor _PurchaseFrom;
        private DateTime _PurchaseDate;
        private string _InvoiceNo;
        private decimal _Cost;
        private SmallToolsAndEquipmentCondition _Condition;
        private SmallToolsAndEquipmentDetail _LastDetail;
        private SmallToolsAndEquipmentDetail _LastCheckedOut;
        private SmallToolsAndEquipmentDetail _LastCheckedIn;
        private Department _LastDepartment;
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
        public SmallToolsAndEquipmentCategory Category
        {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value);
            if (!IsSaving && !IsLoading && _Category != null)
            {
                Prefix = _Category.Prefix;
                XPCollection<SmallToolsAndEquipment> revs = new XPCollection<SmallToolsAndEquipment>(Session);
                var data = revs.OrderBy(o => o.Category).ThenBy(o => o.No).Where(o => o.Category == _Category && o.Oid > 0).LastOrDefault();
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
        public string ToolNo
        {
            get { return string.Format("{0}-{1}", _Prefix, _No); }
        }
        //[RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Item")]
        public Item ItemId
        {
            get { return _ItemId; }
            set { SetPropertyValue("ItemId", ref _ItemId, value);
            if (!IsSaving && !IsLoading && _ItemId != null)
            {
                Description = _ItemId.Description;
            }
            else if (!IsSaving && !IsLoading && _ItemId == null)
            {
                Description = string.Empty;
            }
            }
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
        public SmallToolsAndEquipmentCondition Condition
        {
            get
            {
                if (LastDetail != null)
                {
                    return LastDetail.Condition ?? null;
                }
                else
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SmallToolsAndEquipmentDetail LastDetail
        {
            get
            {
                try
                {
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid)
                        .Where(o => o.Oid > 0).LastOrDefault();
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
        public SmallToolsAndEquipmentDetail LastCheckedOut
        {
            get
            {
                try
                {
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == SmallToolsAndEquipmentActivityTypeEnum.Loaned).LastOrDefault();
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
        public SmallToolsAndEquipmentDetail LastCheckedIn
        {
            get
            {
                try
                {
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == SmallToolsAndEquipmentActivityTypeEnum.Returned).LastOrDefault();
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
        public SmallToolsAndEquipmentDetail FirstValidReserve
        {
            get
            {
                try
                {
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == SmallToolsAndEquipmentActivityTypeEnum.Reserve).Where(o=>o.ReserveDone!=true).FirstOrDefault();
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
        public Department LastDepartment
        {
            get
            {
                if (LastDetail != null)
                {
                    return LastDetail.Department ?? null;
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
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == SmallToolsAndEquipmentActivityTypeEnum.Sent && o.JobOrderDoc != null).LastOrDefault();
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
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == SmallToolsAndEquipmentActivityTypeEnum.Sent && o.WorkOrderDoc != null).LastOrDefault();
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
                try
                {
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == SmallToolsAndEquipmentActivityTypeEnum.Dispose).LastOrDefault();
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
        [Custom("AllowEdit", "False")]
        [DisplayName("Disposal Doc.")]
        public PhysicalAdjustment DisposalDocId
        {
            get
            {
                try
                {
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.ActivityType == SmallToolsAndEquipmentActivityTypeEnum.Dispose).LastOrDefault();
                    if (data != null)
                    {
                        return data.AdjustmentDoc;
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
                        case SmallToolsAndEquipmentStatusEnum.NoHistory:
                            return "NO HISTORY";
                        case SmallToolsAndEquipmentStatusEnum.Available:
                            return "AVAILABLE";
                        case SmallToolsAndEquipmentStatusEnum.Reserved:
                            return "CURRENTLY RESERVED";
                        case SmallToolsAndEquipmentStatusEnum.CheckedOut:
                            return "CURRENTLY CHECKEDOUT";
                        case SmallToolsAndEquipmentStatusEnum.UnderRepair:
                            return "UNDER REPAIR";
                        case SmallToolsAndEquipmentStatusEnum.Sold:
                            return "SOLD";
                        case SmallToolsAndEquipmentStatusEnum.Lost:
                            return "LOST";
                        case SmallToolsAndEquipmentStatusEnum.ForDisposal:
                            return "FOR DISPOSAL";
                        default:
                            return "NO HISTORY";
                    }
                }
                else
                {
                    return "NO HISTORY";
                }
            }
        }
        private bool _Reserved;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public bool Reserved
        {
            get
            {
                try
                {
                    var data = this.SmallToolsAndEquipmentDetails.OrderBy(o => o.Oid)
                        .Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null && data.ActivityType== SmallToolsAndEquipmentActivityTypeEnum.Reserve)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        
        [Aggregated,
        Association("SmallToolsAndEquipmentDetails")]
        public XPCollection<SmallToolsAndEquipmentDetail> SmallToolsAndEquipmentDetails
        {
            get
            {
                return
                    GetCollection<SmallToolsAndEquipmentDetail>("SmallToolsAndEquipmentDetails");
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
        public SmallToolsAndEquipment(Session session)
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
