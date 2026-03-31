using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win {
    public class TempAccount {
        //public TempAccount(Account account, decimal debitAmount, decimal 
        //creditAmount) {
        //    _Account = account;
        //    _DebitAmount = debitAmount;
        //    _CreditAmount = creditAmount;
        //}
        private Account _Account;
        private decimal _DebitAmount;
        private decimal _CreditAmount;
        public Account Account {
            get { return _Account; }
            set { this._Account = value; }
        }
        public decimal DebitAmount {
            get { return _DebitAmount; }
            set { this._DebitAmount = value; }
        }
        public decimal CreditAmount {
            get { return _CreditAmount; }
            set { this._CreditAmount = value; }
        }        
    }
    public class TempAccountCollection : BaseBindingList<TempAccount> {
    }

    public class TempAccount2
    {
        private string _AaUniqueId;
        private Account _Account;
        private decimal _DebitAmount;
        private decimal _CreditAmount;
        public string AaUniqueId
        {
            get { return _AaUniqueId; }
            set { _AaUniqueId = value; }
        }
        public Account Account
        {
            get { return _Account; }
            set { this._Account = value; }
        }
        public decimal DebitAmount
        {
            get { return _DebitAmount; }
            set { this._DebitAmount = value; }
        }
        public decimal CreditAmount
        {
            get { return _CreditAmount; }
            set { this._CreditAmount = value; }
        }
    }
    public class TempAccountCollection2 : BaseBindingList<TempAccount2>
    {
    }

}
