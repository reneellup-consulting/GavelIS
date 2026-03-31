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
    public partial class GetNewSmallToolNoController : ViewController
    {
        private PopupWindowShowAction getNewSmallToolNo;
        private IObjectSpace _ObjectSpace;
        private SmallToolsAndEquipment _stool;
        private NewBatteryNo _Obj;
        public GetNewSmallToolNoController()
        {
            this.TargetObjectType = typeof(SmallToolsAndEquipment);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.GetNewSmallToolNo", this.GetType().
            Name);
            this.getNewSmallToolNo = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.getNewSmallToolNo.Caption = "Get New No.";
            //this.SuppressConfirmation = true;
            this.getNewSmallToolNo.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            getNewRevolvingPartNo_CustomizePopupWindowParams);
            this.getNewSmallToolNo.Execute += new
            PopupWindowShowActionExecuteEventHandler(getNewRevolvingPartNo_Execute);
        }
        private void getNewRevolvingPartNo_CustomizePopupWindowParams(object sender,
CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _stool = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
            as SmallToolsAndEquipment;
            if (_stool.Oid < 0)
            {
                throw new UserFriendlyException("Cannot get new number for unsaved entry");
            }
            SmallToolsAndEquipment otool = _ObjectSpace.GetObject(_stool);
            _Obj = new NewBatteryNo();
            XPCollection<SmallToolsAndEquipment> revs = new XPCollection<SmallToolsAndEquipment>(((ObjectSpace)_ObjectSpace).Session);
            if (_stool.Oid > 0)
            {
                var data = revs.OrderBy(o => o.No).Where(o => o.Category == otool.Category).LastOrDefault();
                if (data != null)
                {
                    _Obj.NewNo = GetNewNo(data.No);
                }
                else
                {
                    _Obj.NewNo = "1000";
                }
            }
            else
            {
                var data = revs.OrderBy(o => o.No).Where(o => o.Category == otool.Category).LastOrDefault();
                if (data != null)
                {
                    _Obj.NewNo = GetNewNo(data.No);
                }
                else
                {
                    _Obj.NewNo = "1000";
                }
            }

            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "NewBatteryNo_Detail", true, _Obj);
        }
        private void getNewRevolvingPartNo_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            if (_Obj.NewNo != "1000")
            {
                _stool.No = _Obj.NewNo;
            }
            else
            {
                _stool.No = "1000";
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
