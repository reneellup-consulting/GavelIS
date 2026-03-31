using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Workflow.Activities;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module
{
    public sealed class CreatePOFromOFRSBuffer : ObjectSpaceActivityBase
    {
        public InArgument<string> frsNo { get; set; }
        public InArgument<DateTime> requestDate { get; set; }
        public InArgument<string> tripDocNo { get; set; }
        public InArgument<string> releasedBy { get; set; }
        public InArgument<DateTime> releasedDate { get; set; }
        public InArgument<string> checkedBy { get; set; }
        public InArgument<DateTime> checkedDate { get; set; }
        public InArgument<string> approvedBy { get; set; }
        public InArgument<DateTime> approvedDate { get; set; }
        public InArgument<string> purpose { get; set; }
        public InArgument<string> remarks { get; set; }
        public InArgument<string> unitNo { get; set; }
        public InArgument<string> unitType { get; set; }
        public InArgument<string> operator1 { get; set; }
        public InArgument<decimal> odometer { get; set; }
        public InArgument<string> tariff { get; set; }
        public InArgument<string> fuelStation { get; set; }
        public InArgument<string> itemNo { get; set; }
        public InArgument<decimal> cost { get; set; }
        public InArgument<decimal> quantity { get; set; }
        public OutArgument<string> result { get; set; }
        public OutArgument<PurchaseOrder> poResult { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            IObjectSpace objectspace = this.GetObjectSpace(context);
            PurchaseOrder porder = objectspace.CreateObject<PurchaseOrder>();
            // Vendor
            Vendor vndr = objectspace.FindObject<Vendor>(BinaryOperator.Parse("[No]=?",fuelStation.Get(context).ToString()));
            porder.Vendor = vndr ?? null;
            // RerenceNo as FRSNo
            porder.ReferenceNo = frsNo.Get(context).ToString();
            // EntryDate
            porder.EntryDate = DateTime.Now;
            // FromOFRS
            porder.IsOnlineFrs = true;
            // Status
            porder.Status = PurchaseOrderStatusEnum.Approved;
            // ApprovedDate
            porder.ApprovedDate = approvedDate.Get<DateTime>(context);
            porder.Approved = true;
            // Remarks > 
            StringBuilder sb = new StringBuilder();

            sb.Append("Online FRS");
            sb.AppendLine();
            sb.Append("-------------------");
            sb.AppendLine();
            sb.AppendFormat("Purpose: {0}", purpose != null ? purpose.Get(context).ToString() : string.Empty);
            sb.AppendLine();
            Tariff trf = objectspace.FindObject<Tariff>(BinaryOperator.Parse("[Code]=?", tariff.Get(context).ToString()));
            sb.AppendFormat("Origin and Dest.: {0} to {1}", trf.Origin.Code, trf.Destination.Code);
            sb.AppendLine();
            sb.AppendFormat("Remarks: {0}", remarks != null ? remarks.Get(context).ToString() : string.Empty);
            sb.AppendLine();
            sb.AppendFormat("Released by: {0} on {1}", releasedBy.Get(context).ToString(), releasedDate.Get<DateTime>(context).ToShortDateString());
            sb.AppendLine();
            sb.AppendFormat("Checked by: {0} on {1}", checkedBy.Get(context).ToString(), checkedDate.Get<DateTime>(context).ToShortDateString());
            sb.AppendLine();
            sb.AppendFormat("Approved by: {0} on {1}", approvedBy.Get(context).ToString(), approvedDate.Get<DateTime>(context).ToShortDateString());
            sb.AppendLine();
            porder.Remarks = sb.ToString();
            
            // Details
            PurchaseOrderDetail podtl = objectspace.CreateObject<PurchaseOrderDetail>();
            podtl.GenJournalID = porder;
            // ItemNo
            podtl.ItemNo = objectspace.FindObject<Item>(BinaryOperator.Parse("[No]=?", itemNo.Get(context).ToString()));
            // ChargeTo
            podtl.CostCenter = objectspace.FindObject<CostCenter>(BinaryOperator.Parse("[Code]=?", unitNo.Get(context).ToString()));
            // RequestedBy
            podtl.RequestedBy = objectspace.FindObject<Employee>(BinaryOperator.Parse("[No]=?", operator1.Get(context).ToString()));
            // LineApprovalStatus
            podtl.LineApprovalStatus = POLineStatusEnum.Released;
            // Quantity
            podtl.Quantity = quantity.Get(context);
            // BaseCost
            podtl.BaseCost = cost.Get(context);
            // Remarks
            podtl.Remarks = purpose.Get(context).ToString();
            poResult.Set(context, porder);
            result.Set(context, "Successfully Created!");
        }
    }
}
