using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CloseCheckVoucherController : ViewController
    {
        private MultiCheckVoucher checkVoucher;
        private SimpleAction closeCheckVoucherAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public CloseCheckVoucherController()
        {
            this.TargetObjectType = typeof(MultiCheckVoucher);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.CloseCheckVoucher", this.GetType().Name);
            this.closeCheckVoucherAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.closeCheckVoucherAction.Caption = "Close Voucher";
            this.closeCheckVoucherAction.Execute += new
            SimpleActionExecuteEventHandler(closeCheckVoucherAction_Execute);
            this.closeCheckVoucherAction.Executed += new EventHandler<
            ActionBaseEventArgs>(closeCheckVoucherAction_Executed);
            this.closeCheckVoucherAction.ConfirmationMessage =
            "Do you really want to close this opened voucher?";
            UpdateActionState(false);
        }
        private void closeCheckVoucherAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            checkVoucher = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as MultiCheckVoucher;
            ObjectSpace.CommitChanges();

            var count = checkVoucher.MCheckVoucherDetails.Count;
            _FrmProgress = new ProgressForm("Closing opened voucher...", count,
            "Updating payment entry {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(checkVoucher);
            _FrmProgress.ShowDialog();
        }
        private void closeCheckVoucherAction_Executed(object sender,
        ActionBaseEventArgs e)
        {
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            MultiCheckVoucher _multiCheckVoucher = (MultiCheckVoucher)e.Argument;
            MultiCheckVoucher thisCheckVoucher = session.GetObjectByKey<MultiCheckVoucher>(
            _multiCheckVoucher.Oid);

            string alterId = Guid.NewGuid().ToString();

            try
            {
                foreach (var item in thisCheckVoucher.MCheckVoucherDetails)
                {
                    #region Algorithm here...
                    string ff = string.Format("[CheckNo] = '{0}' And [AlterId] != '{1}'", item.CheckNo, alterId);
                    // Reset CheckPayment to current
                    IObjectSpace objs = Application.CreateObjectSpace();
                    XPCollection<ResetVerificationDetail> rvdtls = new XPCollection<ResetVerificationDetail>(((ObjectSpace)objs).Session);
                    CheckPayment chkpmt = null;
                    //chkpmt = session.FindObject<CheckPayment>(CriteriaOperator.Parse(string.Format("[CheckNo] = '{0}'", item.CheckNo)));
                    if (!string.IsNullOrEmpty(item.OldCheckNo))
                    {
                        chkpmt = session.FindObject<CheckPayment>(CriteriaOperator.Parse(string.Format("[CheckNo] = '{0}' And [AlterId] <> '{1}'", item.OldCheckNo, alterId)));
                    }
                    else
                    {
                        chkpmt = session.FindObject<CheckPayment>(CriteriaOperator.Parse(string.Format("[CheckNo] = '{0}' And [AlterId] <> '{1}'", item.CheckNo, alterId)));
                    }
                    int jCount = 0;

                    if (chkpmt == null)
                    {
                        chkpmt = session.FindObject<CheckPayment>(CriteriaOperator.Parse(string.Format("[CheckNo] = '{0}' And [AlterId] <> '{1}'", item.CheckNo, alterId)));
                        if (chkpmt == null)
                        {
                            //throw new ApplicationException("Cannot find check no.: " + item.CheckNo);
                            chkpmt = ReflectionHelper.CreateObject<CheckPayment>(session);
                        }
                    }

                    if (chkpmt.AlterId == alterId)
                    {
                        _message = string.Format("Updating check payment {0} succesfull.",
                        thisCheckVoucher.MCheckVoucherDetails.Count - 1);
                        _BgWorker.ReportProgress(1, _message);
                        index++;
                        continue;
                    }

                    chkpmt.AlterId = alterId;

                    foreach (object obj in session.CollectReferencingObjects(chkpmt))
                    {
                        if (obj.GetType() == typeof(GenJournalDetail) && jCount == 0)
                        {
                            jCount++;
                            StringBuilder sb = new StringBuilder();
                            string descript = string.Format("Set transaction to current");
                            ResetVerificationDetail rvdet = ReflectionHelper.CreateObject<ResetVerificationDetail>(((ObjectSpace)objs).Session);
                            rvdet.Action = ResetVerifyOptionEnum.Update;
                            rvdet.Description = descript;
                            GenJournalDetail gjd = obj as GenJournalDetail;
                            sb.AppendFormat("update GenJournalHeader set Approved = 0 where Oid={0}", gjd.GenJournalID.Oid);
                            string name = chkpmt.GetType().Name;
                            sb.AppendLine();
                            sb.AppendFormat("update {0} set Status = 0 where Oid={1}", name, gjd.GenJournalID.Oid);
                            rvdet.Script = sb.ToString();
                            rvdet.Process = true;
                            rvdtls.Add(rvdet);
                        }
                        foreach (XPMemberInfo property in session.GetClassInfo(obj).PersistentProperties)
                        {
                            if (property.MemberType.IsAssignableFrom(chkpmt.GetType()))
                            {
                                string descript = string.Format("Property Name: {0} -->> Obj. Name: {1} >> {2}", property.Name, obj.GetType().Name,
                                    session.GetKeyValue(obj));
                                ResetVerificationDetail rvdet = ReflectionHelper.CreateObject<ResetVerificationDetail>(((ObjectSpace)objs).Session);
                                rvdet.Description = descript;
                                if (session.GetKeyValue(obj).GetType().Name == "Int32")
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

                                rvdtls.Add(rvdet);
                            }
                        }

                    }

                    foreach (var rvdt in rvdtls)
                    {
                        if (rvdt.Process)
                        {
                            session.ExecuteNonQuery(rvdt.Script);
                        }
                    }

                    bool IsExpense = false;
                    // Verify if Expense
                    foreach (MCheckEffectiveDetails mced in thisCheckVoucher.
                    MCheckEffectiveDetails) { if (mced.Expense) { IsExpense = true; } }
                    // If Expense = true
                    Account tmpExp = null;
                    if (IsExpense)
                    {
                        tmpExp = Company.GetInstance(session).TemporaryExpenseAcct;
                        if (tmpExp == null)
                        {
                            throw new ApplicationException(
                                "Temporary account was not provided in the company setup card"
                                );
                        }
                    }

                    chkpmt.EntryDate = item.EntryDate;
                    chkpmt.PostDated = item.PostDated;
                    chkpmt.CheckDate = item.CheckDate;
                    chkpmt.BankCashAccount = item.BankAccount;
                    chkpmt.PaymentMode = PaymentTypeEnum.Check;
                    chkpmt.CheckNo = item.CheckNo;
                    chkpmt.ReferenceNo = item.CheckVoucher.CheckVoucherNo;
                    chkpmt.Memo = item.CheckVoucher.Memo;
                    chkpmt.Comments = item.CheckVoucher.Comments;
                    chkpmt.PayToOrder = item.CheckVoucher.Payee;
                    chkpmt.ExpenseType = item.ExpenseType;
                    chkpmt.SubExpenseType = item.SubExpenseType != null ? item.SubExpenseType : null;
                    chkpmt.CheckAmount = item.Amount;
                    chkpmt.Status = CheckStatusEnum.Approved;
                    chkpmt.Save();

                    // Create the following entry
                    // If Expense = true Dr. Temporary Expense
                    // If Expense = false Dr. Accounts Payable
                    // Cr. Cash in bank
                    if (IsExpense)
                    {
                        // If Expense = true Dr. Temporary Expense
                        GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde1.GenJournalID = chkpmt;
                        _gjde1.GenJournalID.Approved = true;
                        _gjde1.Account = tmpExp;
                        _gjde1.DebitAmount = Math.Abs(chkpmt.CheckAmount);
                        debitAmount = debitAmount + _gjde1.DebitAmount;
                        _gjde1.Description = "Payments Made" + " " + item.
                        CheckVoucher.CheckVoucherNo;
                        _gjde1.SubAccountNo = chkpmt.PayToOrder;
                        _gjde1.SubAccountType = chkpmt.PayToOrder.ContactType;
                        _gjde1.Approved = true;
                        _gjde1.Save();
                    }
                    else
                    {
                        // If Expense = false Dr. Accounts Payable
                        GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde1.GenJournalID = chkpmt;
                        _gjde1.GenJournalID.Approved = true;
                        switch (chkpmt.PayToOrder.ContactType)
                        {
                            case ContactTypeEnum.Customer:
                                _gjde1.Account = ((Customer)chkpmt.PayToOrder).
                                Account;
                                break;
                            case ContactTypeEnum.Vendor:
                                _gjde1.Account = ((Vendor)chkpmt.PayToOrder).
                                Account;
                                break;
                            //case ContactTypeEnum.Payee:
                            //    break;
                            //case ContactTypeEnum.Employee:
                            //    break;
                            default:
                                break;
                        }
                        _gjde1.DebitAmount = Math.Abs(chkpmt.CheckAmount);
                        debitAmount = debitAmount + _gjde1.DebitAmount;
                        _gjde1.Description = "Payments Made" + " " + item.
                        CheckVoucher.CheckVoucherNo;
                        ;
                        _gjde1.SubAccountNo = chkpmt.PayToOrder;
                        _gjde1.SubAccountType = chkpmt.PayToOrder.ContactType;
                        _gjde1.Approved = true;
                        _gjde1.Save();
                    }
                    // Credit Cash in Bank
                    GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde2.GenJournalID = chkpmt;
                    _gjde2.GenJournalID.Approved = true;
                    _gjde2.Account = chkpmt.BankCashAccount;
                    _gjde2.CreditAmount = Math.Abs(chkpmt.CheckAmount);
                    creditAmount = creditAmount + _gjde2.CreditAmount;
                    _gjde2.Description = "Payments Made" + " " + item.
                    CheckVoucher.CheckVoucherNo;
                    _gjde2.SubAccountNo = chkpmt.PayToOrder;
                    _gjde2.SubAccountType = chkpmt.PayToOrder.ContactType;
                    _gjde2.ExpenseType = chkpmt.ExpenseType;
                    _gjde2.SubExpenseType = chkpmt.SubExpenseType != null ? chkpmt.SubExpenseType : null;

                    _gjde2.Approved = true;
                    _gjde2.Save();

                    CommitUpdatingSession(session);

                    #endregion
                    
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Updating check payment {0} succesfull.",
                    thisCheckVoucher.MCheckVoucherDetails.Count - 1);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
                thisCheckVoucher.Reopened = false;
                thisCheckVoucher.Status = MultiCheckStatusEnum.Released;
                thisCheckVoucher.Save();

                CommitUpdatingSession(session);
            }
            finally
            {
                session.Dispose();
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
                    "Re-opening of voucher has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("Voucher " + e.Result +
                    " has been successfully re-opened");

                    ObjectSpace.ReloadObject(checkVoucher);
                    ObjectSpace.Refresh();

                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        private void UpdateActionState(bool inProgress)
        {
            closeCheckVoucherAction.
                Enabled.SetItemValue("Re-opening", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
