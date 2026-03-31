using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using System.Collections;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;
using DevExpress.XtraEditors;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReceiveFromRetreadersController : ViewController
    {
        private Receipt receipt;
        public PopupWindowShowAction receiptFromRetreadersAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ReceiveFromRetreadersController()
        {
            this.TargetObjectType = typeof(Receipt);
            this.TargetViewType = ViewType.DetailView;

            #region Receive Tire from Retreaders

            this.receiptFromRetreadersAction = new PopupWindowShowAction(this, "Receipt.ReceiveFromRetreaders", PredefinedCategory.RecordEdit);
            this.receiptFromRetreadersAction.Caption = "Receive from Retreaders";
            this.receiptFromRetreadersAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(receiptFromRetreadersAction_CustomizePopupWindowParams);
            this.receiptFromRetreadersAction.Execute += new PopupWindowShowActionExecuteEventHandler(receiptFromRetreadersAction_Execute);

            #endregion

        }

        void receiptFromRetreadersAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            StringBuilder sb;
            if (!string.IsNullOrEmpty(receipt.ReferenceNo))
            {
                sb = new StringBuilder(string.Format("{0},", receipt.ReferenceNo));
            }
            else
            {
                sb = new StringBuilder();
            }

            foreach (TireToRetDetail item in e.PopupWindow.View.SelectedObjects)
            {
                TireToRetDetail ttd = receipt.Session.GetObjectByKey<TireToRetDetail>(
                item.Oid);
                if (!sb.ToString().Split(',').Contains(ttd.DocNo.DocNo))
                {
                    sb.AppendFormat("{0},", ttd.DocNo.DocNo);
                }
                ReceiptDetail recpt = new ReceiptDetail(receipt.Session);
                recpt.GenJournalID = receipt;
                // Generic Recap Tire
                TireItem tireItm = receipt.Session.FindObject<TireItem>(new BinaryOperator("No", item.TireNo.TireItem.No));
                recpt.ItemNo = tireItm;
                // Description Vary "11R22.5 TT ROADSHINE/RS612/070913 4883 3"
                if (ttd.TireNo.TireItem.Size==null)
                {
                    throw new ApplicationException(string.Format("Tire item size was not set in detail #{0}", ttd.Oid));
                }
                if (!ttd.Regrooved && ttd.PreferredType == null)
                {
                    throw new ApplicationException(string.Format("Recap type was not set in detail #{0}", ttd.Oid));
                }
                if (ttd.TireNo.TireItem.Make==null)
                {
                    throw new ApplicationException(string.Format("Tire item make was not set in detail #{0}", ttd.Oid));
                }
                if (ttd.TireNo.TireItem.Type==null)
                {
                    throw new ApplicationException(string.Format("Tire item tread type was not set in detail #{0}", ttd.Oid));
                }
                recpt.Description = string.Format("{0} {1} {2}/{3}/{4}", ttd.TireNo.TireItem.Size.Code, ttd.PreferredType!=null?ttd.PreferredType.Code:string.Format("REGROOVED"), ttd.TireNo.TireItem.Make.Code, ttd.TireNo.TireItem.Type.Code, ttd.SerialBrandingNo);
                recpt.Ordered = 1m;
                recpt.Received = 1m;
                recpt.Quantity = 1m;
                recpt.UOM = tireItm.BaseUOM;
                recpt.Factor = 1m;
                recpt.BaseCost = 0m;
                recpt.TireToRetDetailId = ttd;
                recpt.RecapType = ttd.PreferredType??null;
                if (recpt.RecapType !=null && recpt.RecapType.Code == "SR")
                {
                    recpt.WithSR = true;
                }
                recpt.Regrooved = ttd.Regrooved;
                //recpt.RequisitionNo = tmp.RequisitionNo != null ? tmp.RequisitionNo : null;
                recpt.CostCenter = ttd.DocNo.CostCenter ?? null;
                //recpt.RequestedBy = tmp.RequestedBy != null ? tmp.RequestedBy : null;
                recpt.Save();
                ttd.ReceiptDetailId = recpt;
                ttd.Save();
                receipt.ReceiptDetails.BaseAdd(recpt);
            }
            if (sb.ToString().LastOrDefault() == ',')
                sb.Remove(sb.Length - 1, 1);
            receipt.ReferenceNo = sb.ToString();
            receipt.Save();
        }

        void receiptFromRetreadersAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            receipt = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as Receipt;
            string listViewId = "TireToRetDetail_ListView_Select";
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(TireToRetDetail), listViewId);
            if (receipt.Vendor == null)
            {
                throw new ApplicationException("Please specify vendor first");
            }
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(string.Format("[DocNo.Vendor.No] = '{0}'",receipt.Vendor.No));
            e.View = Application.CreateListView(listViewId, collectionSource, true);
        }
    }
}
