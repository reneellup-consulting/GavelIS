using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CorrectOdometer : ViewController {
        private IObjectSpace _ObjectSpace;
        private FixedAsset _FAsset;
        private OdometerRegister _Obj;
        private System.Collections.IList _Sels;
        private PopupWindowShowAction correctOdometer;
        public CorrectOdometer() {
            this.TargetObjectType = typeof(FixedAsset);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("CorrectOdometer");
            this.correctOdometer = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.correctOdometer.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CorrectOdometer_CustomizePopupWindowParams);
            this.correctOdometer.Execute += new 
            PopupWindowShowActionExecuteEventHandler(CorrectOdometer_Execute);
        }

        private void CorrectOdometer_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            decimal lastReading = 0;
            decimal lastLife = 0;
            int icnt = 0;
            _ObjectSpace = Application.CreateObjectSpace();
            _FAsset = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as FixedAsset;
            DetailView curr = (DevExpress.ExpressApp.DetailView)this.View;
            OdometerRegister odr = null;
            long tSeq = 0L;
            foreach (ListPropertyEditor editor in curr.GetItems<
            ListPropertyEditor>()) {
                if (editor.ListView != null && editor.ListView.Id == 
                "FixedAsset_VehicleOdoRegisters_ListView") {
                    _Sels = editor.ListView.SelectedObjects;
                    //foreach (OdometerRegister item in _Sels) {
                    //    if (item.LogType==MeterLogTypeEnum.Fuel || item.LogType==MeterLogTypeEnum.Service)
                    //    {
                    //        throw new UserFriendlyException("Fuel and Service log types cannot be corrected.");
                    //    }
                    //    if (icnt == 0) {
                    //        lastReading = item.Reading;
                    //        lastLife = item.Life;
                    //    } else {
                    //        item.Corrected = true;
                    //    }
                    //    icnt++;
                    //}
                    odr = _Sels[0] as OdometerRegister;
                    OdometerRegister odrdel = _ObjectSpace.GetObjectByKey<OdometerRegister>(
                    odr.Oid);
                    tSeq = odrdel.SeqNo;
                    odrdel.Delete();
                }
            }
            //ObjectSpace.CommitChanges();
            FixedAsset thisFA = _ObjectSpace.GetObjectByKey<FixedAsset>(
            _FAsset.Oid);
            _Obj = _ObjectSpace.CreateObject<OdometerRegister>();
            _Obj.Fleet = thisFA;
            if (thisFA.GetType() == typeof(FATruck))
            {
                _Obj.ReportedBy = ((FATruck)thisFA).Operator != null ? ((FATruck)thisFA).Operator : null;
            }
            _Obj.EntryDate = odr.EntryDate;
            
            _Obj.LogType = odr.LogType;

            _Obj.MeterType = odr.MeterType;
            _Obj.Reference = odr.Reference;
            _Obj.Liters = odr.Liters;
            _Obj.Cost = odr.Cost;
            _Obj.PreviousReading = thisFA.GetPreviousReading(tSeq);
            _Obj.Reading = odr.Reading;
            e.View = Application.CreateDetailView(_ObjectSpace,
             "LogOdometer_Detail", true, _Obj);
        }

        private void CorrectOdometer_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            //foreach (OdometerRegister item in _Sels)
            //{
            //    if (item.Corrected==true)
            //    {
            //        item.Delete();
            //    }
            //}
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_FAsset);
            ObjectSpace.Refresh();
        }
    }
}
