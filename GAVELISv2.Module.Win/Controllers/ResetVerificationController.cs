using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ResetVerificationController : ViewController
    {
        private PopupWindowShowAction ShowDependencyAction;
        public ResetVerificationController()
        {
            this.TargetObjectType = typeof(XPCustomObject);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.VerifyReset", this.GetType().
            Name);
            this.ShowDependencyAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.ShowDependencyAction.Caption = "Verify Reset";
            this.ShowDependencyAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(ShowDependencyAction_CustomizePopupWindowParams);
            this.ShowDependencyAction.Execute += new PopupWindowShowActionExecuteEventHandler(ShowDependencyAction_Execute);
        }

        void ShowDependencyAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (XtraMessageBox.Show("This operation cannot be undone! Do you really want to continue?","Confirm",System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Warning)== System.Windows.Forms.DialogResult.Yes)
            {
                XPCustomObject gjh = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as XPCustomObject;
                Session session = gjh.Session;
                ListView listView = e.PopupWindow.View as ListView;
                IList list = listView.CollectionSource.List;
                foreach (ResetVerificationDetail item in list)
                {
                    if (item.Process)
                    {
                        session.ExecuteNonQuery(item.Script);
                    }
                }
                e.PopupWindow.View.Close();
                ObjectSpace.Refresh();
            }
        }
        void ShowDependencyAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            XPCustomObject gjh = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as XPCustomObject;
            Session session = gjh.Session;
            //StringBuilder sb = new StringBuilder("The dependecies of the selected transaction:");
            //sb.AppendLine();
            IObjectSpace objs = Application.CreateObjectSpace();
            CollectionSource collectionSource = new CollectionSource(objs, typeof(ResetVerificationDetail));
            if ((collectionSource.Collection as XPBaseCollection) != null)
            {
                ((XPBaseCollection)collectionSource.Collection).LoadingEnabled = false;
            }
            int jCount = 0;
            if (gjh.GetType()==typeof(TransferOrder))
            {
                StringBuilder sb = new StringBuilder();
                string descript = string.Format("Set transaction to current");
                ResetVerificationDetail rvdet = objs.CreateObject<ResetVerificationDetail>();
                rvdet.Action = ResetVerifyOptionEnum.Update;
                rvdet.Description = descript;
                sb.AppendFormat("update GenJournalHeader set Approved = 0 where Oid={0}", (gjh as TransferOrder).Oid);
                string name = gjh.GetType().Name;
                sb.AppendLine();
                sb.AppendFormat("update {0} set Status = 0 where Oid={1}", name, (gjh as TransferOrder).Oid);
                rvdet.Script = sb.ToString();
                rvdet.Process = true;
                collectionSource.List.Add(rvdet);
            }
            foreach (object obj in session.CollectReferencingObjects(gjh))
            {
                if (obj.GetType() == typeof(GenJournalDetail) && jCount == 0)
                {
                    jCount++;
                    StringBuilder sb = new StringBuilder();
                    string descript = string.Format("Set transaction to current");
                    ResetVerificationDetail rvdet = objs.CreateObject<ResetVerificationDetail>();
                    rvdet.Action = ResetVerifyOptionEnum.Update;
                    rvdet.Description = descript;
                    GenJournalDetail gjd = obj as GenJournalDetail;
                    sb.AppendFormat("update GenJournalHeader set Approved = 0 where Oid={0}", gjd.GenJournalID.Oid);
                    string name = gjh.GetType().Name;
                    sb.AppendLine();
                    sb.AppendFormat("update {0} set Status = 0 where Oid={1}", name, gjd.GenJournalID.Oid);
                    rvdet.Script = sb.ToString();
                    rvdet.Process = true;
                    collectionSource.List.Add(rvdet);
                }
                foreach (XPMemberInfo property in session.GetClassInfo(obj).PersistentProperties)
                {
                    if (property.MemberType.IsAssignableFrom(gjh.GetType()))
                    {
                        string descript =  string.Format("Property Name: {0} -->> Obj. Name: {1} >> {2}", property.Name, obj.GetType().Name,
                            session.GetKeyValue(obj));
                        ResetVerificationDetail rvdet = objs.CreateObject<ResetVerificationDetail>();
                        rvdet.Description = descript;
                        if (session.GetKeyValue(obj).GetType().Name== "Int32")
                        {
                            rvdet.Action = ResetVerifyOptionEnum.Delete;
                            rvdet.Script = string.Format("delete {0} where Oid = {1}", obj.GetType().Name, session.GetKeyValue(obj));
                        }
                        else
                        {
                            rvdet.Action = ResetVerifyOptionEnum.Delete;
                            rvdet.Script = string.Format("delete {0} where Oid = '{1}'", obj.GetType().Name, session.GetKeyValue(obj));
                        }
                        if (new[] { "RequisitionWorksheet" }.Any(o => obj.GetType().Name.Contains(o)))
                        {
                            //if (session.GetKeyValue(obj).GetType().Name == "Int32")
                            //{
                            //    rvdet.Action = ResetVerifyOptionEnum.Update;
                            //    rvdet.Script = string.Format("update {0} set {1} = NULL, CurrentQtyBase = NULL where Oid = {2}", obj.GetType().Name, property.Name, session.GetKeyValue(obj));
                            //}
                            //else
                            //{
                            //    rvdet.Action = ResetVerifyOptionEnum.Update;
                            //    rvdet.Script = string.Format("update {0} {1} = NULL, CurrentQtyBase = NULL where Oid = '{2}'", obj.GetType().Name, property.Name, session.GetKeyValue(obj));
                            //}
                            if (session.GetKeyValue(obj).GetType().Name == "Int32")
                            {
                                rvdet.Action = ResetVerifyOptionEnum.Update;
                                rvdet.Script = string.Format("update {0} set CurrentQtyBase = NULL where Oid = {1}", obj.GetType().Name, session.GetKeyValue(obj));
                            }
                            else
                            {
                                rvdet.Action = ResetVerifyOptionEnum.Update;
                                rvdet.Script = string.Format("update {0} set CurrentQtyBase = NULL where Oid = '{1}'", obj.GetType().Name, session.GetKeyValue(obj));
                            }
                        }
                        if (!new[] { "PhysicalAdjustmentDetail", "TransferOrderDetail", "ReceiptDetail", 
                            "PurchaseOrderDetail", "InvoiceDetail", "PaymentsApplied", "CreditMemoDetail", 
                            "CollectionDetail", "DebitMemoDetail", "BillDetail", "MCheckVoucherDetail",
                            "MCheckEffectiveDetails", "MultiCheckVouchPayDetail", "JobOrderDetail", "WorkOrderItemDetail",
                            "WorkOrderJobsDetail", "ReceiptFuelDetail", "FuelOdoRegistry", "FuelPumpRegisterDetail", "ServiceOdoRegistry" }.Any(o => obj.GetType().Name.Contains(o)))
                        {
                            rvdet.Process = true;
                        }
                        
                        collectionSource.List.Add(rvdet);
                    }
                }

            }
            ListView view = Application.CreateListView(Application.GetListViewId(typeof(ResetVerificationDetail)), collectionSource, false);
            view.Editor.AllowEdit = true;
            view.AllowNew.SetItemValue("AllowNew", false);
            e.View = view;
            e.DialogController.CanCloseWindow = false;
            e.DialogController.AcceptAction.Caption = "Process Actions";
            e.DialogController.SaveOnAccept = false;
        }
    }
}
