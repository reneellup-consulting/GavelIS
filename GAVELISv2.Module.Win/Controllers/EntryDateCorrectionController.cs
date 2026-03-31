using System;
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
    public partial class EntryDateCorrectionController : ViewController
    {
        private PopupWindowShowAction CorrectEntryDateAction;
        private IObjectSpace _ObjectSpace;
        private GenJournalHeader _GenJrnHead;
        private CorrectEntryDate _CorEntDate;
        public EntryDateCorrectionController()
        {
            this.TargetObjectType = typeof(GenJournalHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.CorrectEntryDate", this.GetType().
            Name);
            this.CorrectEntryDateAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.CorrectEntryDateAction.Caption = "Correct Entry Date";
            this.CorrectEntryDateAction.ConfirmationMessage =
            "Before you continue changing the entry date of the selected transactions, make sure that you know what you are doing.\r\n" +
            "Do you really want to continue?";
            this.CorrectEntryDateAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(CorrectEntryDateAction_CustomizePopupWindowParams);
            this.CorrectEntryDateAction.Execute += new PopupWindowShowActionExecuteEventHandler(CorrectEntryDateAction_Execute);
        }

        void CorrectEntryDateAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            XPCustomObject gjh = this.View.CurrentObject as XPCustomObject;
            Session session = gjh.Session;
            DateTime entryDate = _CorEntDate.EntryDate;
            (gjh as GenJournalHeader).EntryDate = _CorEntDate.EntryDate;
            foreach (object obj in session.CollectReferencingObjects(gjh))
            {
                foreach (XPMemberInfo property in session.GetClassInfo(obj).PersistentProperties)
                {
                    if (property.MemberType.IsAssignableFrom(gjh.GetType()))
                    {
                        if (obj.GetType() == typeof(GenJournalDetail))
                        {
                            (obj as GenJournalDetail).CVLineDate = entryDate;
                            (obj as GenJournalDetail).Save();
                        }
                        if (obj.GetType() == typeof(IncomeAndExpense))
                        {
                            (obj as IncomeAndExpense).EntryDate = entryDate;
                            (obj as IncomeAndExpense).Save();
                        }
                        if (obj.GetType() == typeof(IncomeAndExpense02))
                        {
                            (obj as IncomeAndExpense02).EntryDate = entryDate;
                            (obj as IncomeAndExpense02).Save();
                        }
                        if (obj.GetType() == typeof(MCheckVoucherDetail))
                        {
                            (obj as MCheckVoucherDetail).EntryDate = entryDate;
                            (obj as MCheckVoucherDetail).Save();
                        }
                        if (obj.GetType() == typeof(ARRegistry))
                        {
                            (obj as ARRegistry).Date = entryDate;
                            (obj as ARRegistry).Save();
                        }
                        if (obj.GetType() == typeof(CargoRegistry))
                        {
                            (obj as CargoRegistry).Date = entryDate;
                            (obj as CargoRegistry).Save();
                        }
                        if (obj.GetType() == typeof(DriverRegistry))
                        {
                            (obj as DriverRegistry).Date = entryDate;
                            (obj as DriverRegistry).Save();
                        }
                        if (obj.GetType() == typeof(TruckRegistry))
                        {
                            (obj as TruckRegistry).Date = entryDate;
                            (obj as TruckRegistry).Save();
                        }
                    }
                }
            }
            session.CommitTransaction();
        }

        void CorrectEntryDateAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _GenJrnHead = this.View.CurrentObject as GenJournalHeader;
            _CorEntDate = new CorrectEntryDate();
            _CorEntDate.OldEntryDate = _GenJrnHead.EntryDate;
            _CorEntDate.EntryDate = _GenJrnHead.EntryDate;
            e.View = Application.CreateDetailView(_ObjectSpace, _CorEntDate, true);
        }
    }
}
