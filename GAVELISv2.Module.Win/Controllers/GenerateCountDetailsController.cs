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
    public partial class GenerateCountDetailsController : ViewController
    {
        private SimpleAction generateCountDetailsAction;
        private ItemsMovementGroup _ItemsMovementGroup;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateCountDetailsController()
        {
            this.TargetObjectType = typeof(ItemsMovementGroup);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "generateCountDetailsActionId";
            this.generateCountDetailsAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateCountDetailsAction.Caption = "Generate Count Details";
            this.generateCountDetailsAction.Execute += new SimpleActionExecuteEventHandler(generateCountDetailsAction_Execute);
        }
        private void generateCountDetailsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _ItemsMovementGroup = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as ItemsMovementGroup;

            ObjectSpace.CommitChanges();

            Session t_session = ((ObjectSpace)ObjectSpace).Session;
            string qry = string.Format("select * from vDistinctForPhysCount where GroupId={0}", _ItemsMovementGroup.Oid);
            SelectedData data = t_session.ExecuteQuery(qry);
            int count = data.ResultSet[0].Rows.Count();
            if (data == null || count == 0)
            {
                XtraMessageBox.Show("There are no retrieved data",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            _FrmProgress = new ProgressForm("Generating...", count,
                        "Processing data {0} of {1}");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(data);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            SelectedData trans = (SelectedData)e.Argument;
            try
            {
                ItemsMovementGroup o_img = session.GetObjectByKey<ItemsMovementGroup>(_ItemsMovementGroup.Oid);
                // Delete existing ItemsMovementGroupDetails
                string d_qry = string.Format("delete ItemsMovGrpCountDetail where HeaderId={0}", o_img.Oid);
                int d_res = session.ExecuteNonQuery(d_qry);
                string w_qry = string.Format("select Warehouse, count(Warehouse) as Instance" + 
                    " from vDistinctForPhysCount where GroupId={0} group by Warehouse", o_img.Oid);
                SelectedData w_data = session.ExecuteQuery(w_qry);
                foreach (var w_row in w_data.ResultSet[0].Rows)
                {
                    Warehouse o_whse = session.GetObjectByKey<Warehouse>(Guid.Parse(w_row.Values[0].ToString()));
                    // select * from vDistinctForPhysCount where GroupId=8 and Warehouse='BADE4C0E-977E-4CCC-ADC1-08F0561E4675'
                    string i_qry = string.Format("select * from vDistinctForPhysCount where " +
                        "GroupId={0} and Warehouse='{1}' order by Activity", o_img.Oid, w_row.Values[0].ToString());
                    SelectedData i_data = session.ExecuteQuery(i_qry);
                    foreach (var i_row in i_data.ResultSet[0].Rows)
                    {
                        index++;

                        #region Algorithm here...

                        ItemsMovementGroupDetail o_imgd = session.GetObjectByKey<ItemsMovementGroupDetail>(Convert.ToInt32(i_row.Values[3].ToString()));
                        ItemsMovGrpCountDetail o_imgcd = ReflectionHelper.CreateObject<ItemsMovGrpCountDetail>(session);
                        o_imgcd.HeaderId = o_img;
                        o_imgcd.ItemNo = o_imgd.ItemNo;
                        o_imgcd.Warehouse = o_whse;
                        o_imgcd.Activity = Convert.ToInt32(i_row.Values[5].ToString());
                        o_imgcd.CurrQty = Convert.ToDecimal(i_row.Values[6].ToString());
                        o_imgcd.StockUnit = o_imgd.ItemNo.StockUOM;
                        o_imgcd.Save();

                        #endregion

                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                        CommitUpdatingSession(session);
                        _message = string.Format("Processing data {0} succesfull.", index);
                        _BgWorker.ReportProgress(1, _message);
                    }
                }
                
            }
            finally
            {
                if (index == trans.ResultSet[0].Rows.Count())
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
