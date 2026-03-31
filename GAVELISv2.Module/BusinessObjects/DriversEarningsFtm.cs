using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Editors;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class DriversEarningsFtm : BaseObject
    {
        private int _TargetYear;
        private MonthsEnum _TargetMonth;
        // TargetMonth
        public MonthsEnum TargetMonth
        {
            get { return _TargetMonth; }
            set { SetPropertyValue("TargetMonth", ref _TargetMonth, value); }
        }
        // TargetYear
        [Custom("DisplayFormat", "d")]
        [Custom("EditMask", "d")]
        public int TargetYear
        {
            get { return _TargetYear; }
            set { SetPropertyValue("TargetYear", ref _TargetYear, value); }
        }

        [Aggregated,
        Association("DriversEarningFtmDetails")]
        public XPCollection<DriversEarningFtmDetail> DriversEarningFtmDetails
        {
            get
            {
                return
                    GetCollection<DriversEarningFtmDetail>("DriversEarningFtmDetails");
            }
        }

        public DriversEarningsFtm(Session session)
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
        }

        public static DriversEarningsFtm GetInstance(Session session)
        {
            //Get the Singleton's instance if it exists 
            DriversEarningsFtm result = session.FindObject<
            DriversEarningsFtm>(null);
            //Create the Singleton's instance 
            if (result == null)
            {
                result = new DriversEarningsFtm(session);
                result.TargetYear = DateTime.Now.Year;

                // Get the current month
                DateTime currentDate = DateTime.Now;
                int currentMonth = currentDate.Month;
                // Convert the current month to MonthsEnum
                MonthsEnum targetMonth = (MonthsEnum)currentMonth;
                result.TargetMonth = targetMonth;

                result.Save();
            }
            return result;
        }

        //Prevent the Singleton from being deleted 
        protected override void OnDeleting()
        {
            throw new UserFriendlyException(
                "The system prohibits the deletion of Adjust Item Cost Prices Object."
                );
        }
    }
}
