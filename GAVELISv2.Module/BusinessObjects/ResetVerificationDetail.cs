using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum ResetVerifyOptionEnum
    {
        Delete,
        Update
    }
    public class ResetVerificationDetail : XPObject
    {
        private static int myOid = 0;
        public ResetVerificationDetail(Session session)
            : base(session)
        {
            myOid++;
            Oid = myOid;
        }
        public bool Process
        {
            get { return GetPropertyValue<bool>("Process"); }
            set { SetPropertyValue<bool>("Process", value); }
        }
        public ResetVerifyOptionEnum Action
        {
            get { return GetPropertyValue<ResetVerifyOptionEnum>("Action"); }
            set { SetPropertyValue<ResetVerifyOptionEnum>("Action", value); }
        }
        //[Custom("AllowEdit", "False")]
        //public object ObjectValue
        //{
        //    get { return GetPropertyValue<string>("Value"); }
        //    set { SetPropertyValue<object>("Value", value); }
        //}
        [Size(500)]
        [Custom("AllowEdit", "False")]
        public string Description
        {
            get { return GetPropertyValue<string>("Description"); }
            set { SetPropertyValue<string>("Description", value); }
        }
        [Size(500)]
        [Custom("AllowEdit", "False")]
        public string Script
        {
            get { return GetPropertyValue<string>("Script"); }
            set { SetPropertyValue<string>("Script", value); }
        }
    }
}
