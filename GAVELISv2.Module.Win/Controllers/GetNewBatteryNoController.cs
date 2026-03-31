using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GetNewBatteryNoController : ViewController
    {
        private PopupWindowShowAction getNewBatteryNo;
        private IObjectSpace _ObjectSpace;
        private Battery _Batt;
        private NewBatteryNo _Obj;
        public GetNewBatteryNoController()
        {
            this.TargetObjectType = typeof(Battery);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.GetNewBatteryNo", this.GetType().
            Name);
            this.getNewBatteryNo = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.getNewBatteryNo.Caption = "Get New No.";
            //this.SuppressConfirmation = true;
            this.getNewBatteryNo.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            getNewBatteryNo_CustomizePopupWindowParams);
            this.getNewBatteryNo.Execute += new
            PopupWindowShowActionExecuteEventHandler(getNewBatteryNo_Execute);
        }
        private void getNewBatteryNo_CustomizePopupWindowParams(object sender,
CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _Batt = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
            as Battery;
            _Obj = new NewBatteryNo();
            XPCollection<Battery> batts = new XPCollection<Battery>(((ObjectSpace)_ObjectSpace).Session);
            var data = batts.OrderBy(o => o.BatteryNo).Where(o => o.Oid > 0).LastOrDefault();
            if (data != null)
            {
                _Obj.NewNo = GetNewNo(data.BatteryNo);
            }
            else
            {
                _Obj.NewNo = "1000";
            }
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "NewBatteryNo_Detail", true, _Obj);
        }
        private void getNewBatteryNo_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            if (_Obj.NewNo != "1000")
            {
                _Batt.BatteryNo = _Obj.NewNo;
            }
            else
            {
                _Batt.BatteryNo = "1000";
            }
        }

        private string GetNewNo(string lastNo)
        {
            string seqNo;
            string incNo;
            int inc = 1;
            if (!string.IsNullOrEmpty(lastNo))
            {
                seqNo = lastNo
                    ;
            }
            else
            {
                seqNo = "1000";
            }
            string digits = "0123456789";
            string defaultFormat = "{0:D5}";
            string formatString = string.Empty;
            string num = string.Empty;
            int c = 0;
            int i, x;
            i = x = seqNo.LastIndexOfAny(digits.ToCharArray());
            while (i >= 0 && isDigit(seqNo[i]))
            {
                num = seqNo[i] + num;
                c++;
                i--;
            }
            int n = int.Parse(num) + inc;
            formatString = defaultFormat.Replace("5", c.ToString());
            incNo = string.Format(formatString, n);
            x = x + 1 - num.Length;
            seqNo = seqNo.Remove(x, num.Length);
            seqNo = seqNo.Insert(x, string.Empty + incNo);
            lastNo = seqNo;
            // Update the No Series Line
            //UnitOfWork uow = new UnitOfWork();
            //ObjectSpace os = new ObjectSpace(uow);
            //Session _session = new Session(objectSpace.Session.DataLayer);
            //_session.BeginTransaction();
            //NoSeriesLine nSLine = _session.FindObject<NoSeriesLine>(new
            //BinaryOperator("Oid", nsLineNo));
            //nSLine.LastDateUsed = DateTime.Today;
            //nSLine.LastNoUsed = seqNo;
            ////nSLine.;
            //nSLine.Save();
            //_session.CommitTransaction();
            return seqNo;
        }
        private static bool isDigit(char c)
        {
            string digits = "0123456789";
            return digits.IndexOf(c) == -1 ? false : true;
        }
    }
}
