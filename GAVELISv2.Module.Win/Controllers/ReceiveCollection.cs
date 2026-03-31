using System;
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
    public partial class ReceiveCollection : ViewController
    {
        private Collection collection;
        private SimpleAction receiveCollectionAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;

        public ReceiveCollection()
        {
            this.TargetObjectType = typeof(Collection);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Receive", this.GetType().Name);
            this.receiveCollectionAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.receiveCollectionAction.Caption = "Receive";
            this.receiveCollectionAction.Execute += new
            SimpleActionExecuteEventHandler(
            ReceiveCollectionAction_Execute);
            this.receiveCollectionAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReceiveCollectionAction_Executed);
            this.receiveCollectionAction.ConfirmationMessage =
            "Do you really want to receive this payments?";
            UpdateActionState(false);
        }
        private void ReceiveCollectionAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            collection = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as Collection;
            if (collection.Reopened)
            {
                throw new ApplicationException(
                        "Cannot proceed because collection was opened manually");
            }
            switch (collection.ReceiveFrom.ContactType)
            {
                case ContactTypeEnum.Customer:
                    if (((Customer)collection.ReceiveFrom).Account == null)
                    {
                        throw new ApplicationException(
                        "Accounts Receivable account must be specified in the chosen "
                        + collection.ReceiveFrom.ContactType + " card");
                    }
                    break;
                case ContactTypeEnum.Vendor:
                    if (((Vendor)collection.ReceiveFrom).Account == null)
                    {
                        throw new ApplicationException(
                        "Accounts Payable account must be specified in the chosen vendor card"
                        );
                    }
                    break;
                default:
                    break;
            }
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(collection);
        }
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            Collection _collection = (Collection)e.Argument;
            Collection thisCollection = session.GetObjectByKey<Collection>(_collection.Oid);
            try
            {
                //Create ReceivePayment instance for each collection line
                foreach (CollectionDetail item in thisCollection.CollectionDetails)
                {
                    ReceivePayment payment=ReflectionHelper.CreateObject<ReceivePayment>(session);
                    payment.BankCashAccount=item.BankAccount;
                    payment.PaymentMode=thisCollection.PaymentMode;
                    payment.Status = PaymentStatusEnum.Approved;
                    payment.CheckNo = item.CheckNo;
                    payment.PostDated = item.PostDated;
                    payment.EntryDate = thisCollection.EntryDate;
                    payment.BankBranch = item.BankBranch;
                    payment.CheckDate = item.CheckDate;
                    payment.ReferenceNo = item.RefNo;
                    payment.ReceiveFrom = thisCollection.ReceiveFrom;
                    payment.GetFromAccount = thisCollection.GetFromAccount!=null?thisCollection.GetFromAccount:null;
                    payment.CheckAmount=item.Amount;
                    if (item.Withheld!=0)
                    {
                        if (item.OutputTaxAcct==null)
                        {
                            throw new ApplicationException(string.Format("Output Tax Account was not specified in line ID {0}.",item.Oid));
                        }
                        payment.Withheld = item.Withheld;
                        payment.OutputTaxAcct = item.OutputTaxAcct;
                    }
                    
                    payment.IncomeType = item.IncomeType;
                    payment.Memo = thisCollection.Memo;
                    payment.Save();

                    if (payment.ReceiveFrom.ContactType ==
                    ContactTypeEnum.Customer || payment.ReceiveFrom.
                    ContactType == ContactTypeEnum.Vendor)
                    {
                        // Debit Accounts Payable
                        GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde.GenJournalID = payment;
                        _gjde.GenJournalID.Approved = true;
                        switch (payment.ReceiveFrom.ContactType)
                        {
                            case ContactTypeEnum.Customer:
                                _gjde.Account = ((Customer)payment.
                                ReceiveFrom).Account;
                                break;
                            case ContactTypeEnum.Vendor:
                                _gjde.Account = ((Vendor)payment.
                                ReceiveFrom).Account;
                                break;
                            //case ContactTypeEnum.Payee:
                            //    break;
                            //case ContactTypeEnum.Employee:
                            //    break;
                            default:
                                break;
                        }
                        _gjde.CreditAmount = Math.Abs(item.LineAmount
                        );
                        creditAmount = creditAmount + _gjde.CreditAmount;
                        _gjde.Description = "Received Payment";
                        _gjde.SubAccountNo = payment.ReceiveFrom;
                        _gjde.SubAccountType = payment.ReceiveFrom.
                        ContactType;
                        _gjde.Approved = true;
                        _gjde.Save();
                        // Credit Bank or Cash Account
                        GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde2.GenJournalID = payment;
                        _gjde2.GenJournalID.Approved = true;
                        _gjde2.Account = payment.BankCashAccount;
                        _gjde2.DebitAmount = Math.Abs(payment.CheckAmount
                        );
                        debitAmount = debitAmount + _gjde2.DebitAmount;
                        _gjde2.Description = "Received Payment";
                        _gjde2.SubAccountNo = payment.ReceiveFrom;
                        _gjde2.SubAccountType = payment.ReceiveFrom.
                        ContactType;
                        _gjde2.ExpenseType = payment.IncomeType;
                        _gjde2.SubExpenseType = payment.SubIncomeType != null ? payment.SubIncomeType : null;
                        _gjde2.Approved = true;
                        _gjde2.Save();
                    }
                    else
                    {
                        if (payment.GetFromAccount == null)
                        {
                            throw new
                                ApplicationException("Must provide Account to get from")
                                ;
                        }
                        // Debit Accounts Payable
                        GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde.GenJournalID = payment;
                        _gjde.GenJournalID.Approved = true;
                        _gjde.Account = payment.GetFromAccount;
                        _gjde.CreditAmount = Math.Abs(payment.CheckAmount
                        );
                        creditAmount = creditAmount + _gjde.CreditAmount;
                        _gjde.Description = "Received Payment";
                        _gjde.SubAccountNo = payment.ReceiveFrom;
                        _gjde.SubAccountType = payment.ReceiveFrom.
                        ContactType;
                        _gjde.Approved = true;
                        _gjde.Save();
                        // Credit Bank or Cash Account
                        GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde2.GenJournalID = payment;
                        _gjde2.GenJournalID.Approved = true;
                        _gjde2.Account = payment.BankCashAccount;
                        _gjde2.DebitAmount = Math.Abs(payment.CheckAmount
                        );
                        debitAmount = debitAmount + _gjde2.DebitAmount;
                        _gjde2.Description = "Received Payment";
                        _gjde2.SubAccountNo = payment.ReceiveFrom;
                        _gjde2.SubAccountType = payment.ReceiveFrom.
                        ContactType;
                        _gjde2.ExpenseType = payment.IncomeType;
                        _gjde2.SubExpenseType = payment.SubIncomeType != null ? payment.SubIncomeType : null;
                        _gjde2.Approved = true;
                        _gjde2.Save();
                    }
                    if (item.Withheld!=0)
                    {
                        // Credit Bank or Cash Account
                        GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde2.GenJournalID = payment;
                        _gjde2.GenJournalID.Approved = true;
                        _gjde2.Account = payment.OutputTaxAcct;
                        _gjde2.DebitAmount = Math.Abs(payment.Withheld
                        );
                        debitAmount = debitAmount + _gjde2.DebitAmount;
                        _gjde2.Description = "Received Payment";
                        _gjde2.SubAccountNo = payment.ReceiveFrom;
                        _gjde2.SubAccountType = payment.ReceiveFrom.
                        ContactType;
                        _gjde2.ExpenseType = payment.IncomeType;
                        _gjde2.SubExpenseType = payment.SubIncomeType != null ? payment.SubIncomeType : null;
                        _gjde2.Approved = true;
                        _gjde2.Save();
                    }
                    index++;
                }
            }
            finally
            {
                if (index==thisCollection.CollectionDetails.Count)
                {
                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                            ApplicationException("Accounting entries not balance");
                    }
                    thisCollection.Status=CollectionStatusEnum.Received;
                    CommitUpdatingSession(session);

                }

            }

        }
        private void ReceiveCollectionAction_Executed(object sender, ActionBaseEventArgs
        e)
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

        private void BgWorkerRunWorkerCompleted(object sender,
RunWorkerCompletedEventArgs e)
        {
            //_FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Receiving payment operation has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("Payments has been successfully received");
                    //ObjectSpace.ReloadObject(collection);
                    ObjectSpace.Refresh();
                }
            }
        }

        private void UpdateActionState(bool inProgress)
        {
            this.receiveCollectionAction.Enabled.SetItemValue("Receiving Payments", !
            inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
