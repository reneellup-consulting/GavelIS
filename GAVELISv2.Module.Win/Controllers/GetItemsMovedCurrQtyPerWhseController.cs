using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.Xpo.DB;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GetItemsMovedCurrQtyPerWhseController : ViewController
    {
        private SimpleAction getItemsMovedCurrQtyPerWhseAction;
        private ItemsMovementGroupDetail _ItemsMovementGroupDetail;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GetItemsMovedCurrQtyPerWhseController()
        {
            this.TargetObjectType = typeof(ItemsMovementGroupDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = "getItemsMovedCurrQtyPerWhseActionId";
            this.getItemsMovedCurrQtyPerWhseAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.getItemsMovedCurrQtyPerWhseAction.TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueForAll;
            //this.getItemsMovedCurrQtyPerWhseAction.TargetObjectsCriteria = "[ItemsMovedPerWhseQtyLines][].Count() = 0";
            this.getItemsMovedCurrQtyPerWhseAction.Caption = "Get Qty/Whse";
            this.getItemsMovedCurrQtyPerWhseAction.Execute += new SimpleActionExecuteEventHandler(getItemsMovedCurrQtyPerWhseAction_Execute);
        }
        private void getItemsMovedCurrQtyPerWhseAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no rows selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList selected = null;
            selected = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;

            int count = selected.Count; ;

            //string qry = string.Format("select ItemID, WhseId, sum(Qty) as WhseQty from vInvItemWhseQty where ItemID = '{0}' group by ItemID, WhseId order by ItemID", _ItemsMovementGroupDetail.ItemNo.Oid);

            //Session t_session = ((ObjectSpace)ObjectSpace).Session;
            //SelectedData data = t_session.ExecuteQuery(qry);
            //if (data == null || data.ResultSet[0].Rows.Count() == 0)
            //{
            //    throw new UserFriendlyException("No data was retrieved.");
            //}

            //count = data.ResultSet[0].Rows.Count();

            _FrmProgress = new ProgressForm("Generate...", count,
                        "Processing row {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(selected);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;
            try
            {
                foreach (ItemsMovementGroupDetail item in trans)
                {
                    index++;

                    #region Algorithm here...

                    ItemsMovementGroupDetail oGenerator = session.GetObjectByKey<ItemsMovementGroupDetail>(item.Oid);

                    if (oGenerator.ItemsMovedPerWhseQtyLines.Count > 0)
                    {
                        for (int i = oGenerator.ItemsMovedPerWhseQtyLines.Count - 1; i >= 0; i--)
                        {
                            oGenerator.ItemsMovedPerWhseQtyLines[i].Delete();
                        }
                        CommitUpdatingSession(session);
                    }

                    //select ItemID, WhseId, count(WhseId) as WhseAct, sum(Qty) 
                    //as WhseQty, Uomid from vInvItemWhseQty where ItemID = 'ec9db511-a2ef-4740-af18-673202d8beb8' and 
                    //WhseId='f5e34c8a-0cf6-497d-adcf-5047ea0f2141' group by ItemID, WhseId, Uomid order by ItemID

                    string qry = string.Format("select ItemID, WhseId, count(WhseId) as WhseAct, sum(Qty) as WhseQty, Uomid from vInvItemWhseQty where ItemID = '{0}' group by ItemID, WhseId, Uomid order by ItemID, WhseId", item.ItemNo.Oid);

                    Session t_session = ((ObjectSpace)ObjectSpace).Session;
                    SelectedData data = t_session.ExecuteQuery(qry);

                    Warehouse old_warehouse = null;
                    ItemsMovedPerWhseQty o_impq = null;
                    foreach (var row in data.ResultSet[0].Rows)
                    {
                        if (row.Values[0] != null)
                        {
                            if (row.Values[1].ToString() == ("F5E34C8A-0CF6-497D-ADCF-5047EA0F2141").ToLower())
                            {

                            }
                            Item o_itm = session.GetObjectByKey<Item>(Guid.Parse(row.Values[0].ToString()));
                            Warehouse o_whse = session.GetObjectByKey<Warehouse>(Guid.Parse(row.Values[1].ToString()));

                            if (old_warehouse != o_whse)
                            {
                                old_warehouse = o_whse;
                                o_impq = ReflectionHelper.CreateObject<ItemsMovedPerWhseQty>(session);
                                o_impq.MovementDetId = oGenerator;
                                o_impq.Warehouse = o_whse;
                            }
                            o_impq.Activity += Convert.ToInt32(row.Values[2].ToString());
                            Guid u_id = Guid.Parse(row.Values[4].ToString());
                            UnitOfMeasure r_uom = session.GetObjectByKey<UnitOfMeasure>(u_id);
                            UnitOfMeasure base_uom = o_itm.UOMRelations.Count > 0 ? o_itm.BaseUOM2 : o_itm.BaseUOM;
                            UnitOfMeasure stock_uom = o_itm.StockUOM != null ? o_itm.StockUOM : base_uom;
                            if (o_itm.UOMRelations.Count > 0)
                            {
                                if (base_uom == r_uom)
                                {
                                    var uom_rel_base = o_itm.UOMRelations.Where(o => o.UOM == base_uom).FirstOrDefault();
                                    var uom_rel_stock = o_itm.UOMRelations.Where(o => o.UOM == stock_uom).FirstOrDefault();
                                    var uom_rel = o_itm.UOMRelations.Where(o => o.UOM == r_uom).FirstOrDefault();
                                    if (uom_rel.Factor == uom_rel_base.Factor)
                                    {
                                        o_impq.CurrQty += Convert.ToDecimal(row.Values[3].ToString()) / uom_rel_stock.Factor;
                                    }
                                    else
                                    {
                                        decimal baseQty = Convert.ToDecimal(row.Values[3].ToString()) * uom_rel.Factor;
                                        o_impq.CurrQty += baseQty / uom_rel_stock.Factor;

                                    }
                                }
                                else
                                {
                                    if (r_uom == stock_uom)
                                    {
                                        o_impq.CurrQty += Convert.ToDecimal(row.Values[3].ToString());
                                    }
                                    else
                                    {
                                        var uom_rel_base = o_itm.UOMRelations.Where(o => o.UOM == base_uom).FirstOrDefault();
                                        var uom_rel_stock = o_itm.UOMRelations.Where(o => o.UOM == stock_uom).FirstOrDefault();
                                        var uom_rel = o_itm.UOMRelations.Where(o => o.UOM == r_uom).FirstOrDefault();
                                        if (uom_rel.Factor == uom_rel_base.Factor)
                                        {
                                            o_impq.CurrQty += Convert.ToDecimal(row.Values[3].ToString()) / uom_rel_stock.Factor;
                                        }
                                        else
                                        {
                                            decimal baseQty = Convert.ToDecimal(row.Values[3].ToString()) * uom_rel.Factor;
                                            o_impq.CurrQty += baseQty / uom_rel_stock.Factor;

                                        }
                                    }
                                }
                            }
                            else
                            {
                                o_impq.CurrQty += Convert.ToDecimal(row.Values[3].ToString());
                            }
                            //if (stock_uom == r_uom)
                            //{
                            //    o_impq.CurrQty += Convert.ToDecimal(row.Values[3].ToString());
                            //}
                            //else
                            //{
                            //    if (o_itm.UOMRelations.Count > 0)
                            //    {
                            //        if (stock_uom != base_uom)
                            //        {
                            //            var uom_rel = o_itm.UOMRelations.Where(o => o.UOM == stock_uom).FirstOrDefault();
                            //            o_impq.CurrQty += Convert.ToDecimal(row.Values[3].ToString()) * uom_rel.Factor;
                            //        }
                            //        else
                            //        {
                            //            var uom_rel = o_itm.UOMRelations.Where(o => o.UOM == stock_uom).FirstOrDefault();
                            //            o_impq.CurrQty += Convert.ToDecimal(row.Values[3].ToString()) * uom_rel.Factor;
                            //        }
                                    
                            //    }
                            //    else
                            //    {
                            //        throw new ApplicationException(string.Format("Unreliable quantity read item #{0}", o_itm.No));
                            //    }
                            //}
                                             
                            o_impq.Save();
                        }
                    }

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                    _message = string.Format("Processing row {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.Count)
                {
                    e.Result = index;
                }
                session.Dispose();
            }
        }
        private UnitOfWork CreateUpdatingSession()
        {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }
        private void CommitUpdatingSession(UnitOfWork session)
        {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }
        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session)
        {
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                    , new SessionEventArgs(session));
            }
        }
        protected virtual void OnUpdatingSessionCreated(UnitOfWork session)
        {
            if
                (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                    SessionEventArgs(session));
            }
        }
        private void BgWorkerProgressChanged(object sender,
        ProgressChangedEventArgs e)
        {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                    DoProgress(e.ProgressPercentage);
            }
        }
        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Generation is cancelled.", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                        Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show(
                    "Generation has been successfull.");
                    //ObjectSpace.ReloadObject(_AttendanceCalculator);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
