using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using System.Threading.Tasks;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CreateReceiptFromFuelPO : ViewController
    {
        private PopupWindowShowAction createReceiptFromFuelPO;
        private ReceiptFuel _Receipt;
        private Vendor _FVendor;
        public CreateReceiptFromFuelPO()
        {
            this.TargetObjectType = typeof(ReceiptFuel);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "ReceiptFuel.CreateReceiptFromPO";
            this.createReceiptFromFuelPO = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.createReceiptFromFuelPO.Caption="Receive from PO";
            this.createReceiptFromFuelPO.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CreateReceiptFromPO_CustomizePopupWindowParams);
            this.createReceiptFromFuelPO.Execute += new 
            PopupWindowShowActionExecuteEventHandler(CreateReceiptFromPO_Execute
            );
        }
        private void CreateReceiptFromPO_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _Receipt = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ReceiptFuel;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "POrderFuelDetail_ListView_ToReceive";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(POrderFuelDetail),
            listViewId);
            if (_Receipt.Vendor != null)
            {
                collectionSource.Criteria[
                "ModelCriteria"] = CriteriaOperator.Parse(
                "[PurchaseInfo.Status] In ('Approved', 'PartiallyReceived') And [RemainingQty] <> 0.0m And [PurchaseInfo.Vendor.No] = '"
                 + _Receipt.Vendor.No + "'");
            }
            else
            {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse("[PurchaseInfo.Status] In ('Approved', 'PartiallyReceived') And [RemainingQty] <> 0.0m");
            }
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void CreateReceiptFromPO_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            var selectedPoDetails = e.PopupWindow.View.SelectedObjects.Cast<POrderFuelDetail>().ToList();
            if (!selectedPoDetails.Any())
            {
                return;
            }

            if (selectedPoDetails.Count > 1)
            {
                throw new UserFriendlyException("Multiple PO Detail selected not allowed.");
            }

            if (_Receipt.ReceiptFuelDetails.Count > 0)
            {
                throw new UserFriendlyException("One item only per Receipt Fuel transaction.");
            }

            var poSession = _Receipt.Session;
            var firstPoDetail = selectedPoDetails.First();
            var firstPoHeader = firstPoDetail.PurchaseInfo;

            // ===================================================================================
            // VALIDATION: Ensure all selected items are consistent with each other.
            // ===================================================================================
            //foreach (var otherPoDetail in selectedPoDetails.Skip(1))
            //{
            //    var otherPoHeader = otherPoDetail.PurchaseInfo;
            //    if (otherPoHeader.Vendor != firstPoHeader.Vendor)
            //        throw new UserFriendlyException("All selected items must be from the same Vendor.");
            //    if (otherPoHeader.Customer != firstPoHeader.Customer)
            //        throw new UserFriendlyException("All selected items must be for the same Customer.");
            //    if (otherPoHeader.Driver != firstPoHeader.Driver)
            //        throw new UserFriendlyException("All selected items must have the same Driver.");
            //    if (otherPoHeader.TruckNo != firstPoHeader.TruckNo || otherPoHeader.GensetNo != firstPoHeader.GensetNo || otherPoHeader.OtherVehicle != firstPoHeader.OtherVehicle)
            //        throw new UserFriendlyException("All selected items must be for the same Vehicle/Genset.");
            //    if (otherPoDetail.CostCenter != firstPoDetail.CostCenter)
            //        throw new UserFriendlyException("All selected items must have the same 'Charge To' Cost Center.");
            //    if (otherPoHeader.FuelUsageClassification != firstPoHeader.FuelUsageClassification)
            //        throw new UserFriendlyException("All selected items must have the same Fuel Usage Classification.");
            //    if (otherPoHeader.TripType != firstPoHeader.TripType)
            //        throw new UserFriendlyException("All selected items must have the same Trip Type.");
            //}

            // ===================================================================================
            // VALIDATION: Ensure selection is consistent with the current Receipt header, if it has existing data.
            // ===================================================================================
            //if (_Receipt.Vendor != null && _Receipt.Vendor != firstPoHeader.Vendor)
            //    throw new UserFriendlyException(string.Format("The selected items are from Vendor '{0}', which conflicts with the receipt's existing Vendor '{1}'.", firstPoHeader.Vendor.Name, _Receipt.Vendor.Name));
            //if (_Receipt.TruckNo != null && _Receipt.TruckNo != firstPoHeader.TruckNo)
            //    throw new UserFriendlyException("The selected items are for a different truck than the one already on this receipt.");
            //// (Add similar checks for other fields like Driver, GensetNo, etc. if needed)
            //if (_Receipt.GensetNo != null && _Receipt.GensetNo != firstPoHeader.GensetNo)
            //    throw new UserFriendlyException("The selected items are for a different genset than the one already on this receipt.");
            //if (_Receipt.OtherNo != null && _Receipt.OtherNo != firstPoHeader.OtherNo)
            //    throw new UserFriendlyException("The selected items are for a different other vehicle than the one already on this receipt.");
            //if (_Receipt.OtherVehicle != null && _Receipt.OtherVehicle != firstPoHeader.OtherVehicle)
            //    throw new UserFriendlyException("The selected items are for a different other vehicle than the one already on this receipt.");

            // ===================================================================================
            // AUTO-POPULATE RECEIPT HEADER from the first selected item.
            // ===================================================================================
            // This section populates the main form's fields based on the first selected item.
            _Receipt.Vendor = poSession.GetObjectByKey<Vendor>(firstPoHeader.Vendor.Oid);
            _Receipt.VendorAddress = firstPoHeader.VendorAddress;
            _Receipt.ShipToAddress = firstPoHeader.ShipToAddress;
            _Receipt.Terms = poSession.GetObjectByKey<Terms>(firstPoHeader.Terms.Oid);
            _Receipt.PurchaseOrderNo = poSession.GetObjectByKey<PurchaseOrderFuel>(firstPoHeader.Oid);
            
            _Receipt.Driver = poSession.GetObjectByKey<Employee>(firstPoHeader.Driver.Oid);
            _Receipt.TruckOrGenset = firstPoHeader.TruckOrGenset;
            switch (_Receipt.TruckOrGenset)
            {
                case TruckOrGensetEnum.Truck:
                    _Receipt.TruckNo = firstPoHeader.TruckNo != null ? poSession.GetObjectByKey<FATruck>(firstPoHeader.TruckNo.Oid) : null;
                    break;
                case TruckOrGensetEnum.Genset:
                    _Receipt.GensetNo = firstPoHeader.GensetNo != null ? poSession.GetObjectByKey<FAGeneratorSet>(firstPoHeader.GensetNo.Oid) : null;
                    break;
                case TruckOrGensetEnum.NotApplicable:
                    break;
                case TruckOrGensetEnum.Other:
                    _Receipt.OtherNo = firstPoHeader.OtherNo != null ? poSession.GetObjectByKey<FAOtherVehicle>(firstPoHeader.OtherNo.Oid) : null;
                    break;
                default:
                    break;
            }
            //_Receipt.OtherVehicle = firstPoHeader.OtherVehicle != null ? poSession.GetObjectByKey<FAOtherVehicle>(firstPoHeader.OtherVehicle.Oid) : null;
            _Receipt.FuelUsageClassification = firstPoHeader.FuelUsageClassification;
            _Receipt.TripType = firstPoHeader.TripType != null ? poSession.GetObjectByKey<TripType>(firstPoHeader.TripType.Oid) : null;

            // Update the concatenated PO reference numbers
            var poNumbers = _Receipt.ReceiptDetails.Select(rd => rd.PODetailID.PurchaseInfo.SourceNo)
                                                  .Union(selectedPoDetails.Select(d => d.PurchaseInfo.SourceNo))
                                                  .Distinct()
                                                  .OrderBy(num => num);
            _Receipt.ReferenceNo = string.Join(", ", poNumbers);
            _Receipt.Remarks = firstPoHeader.Remarks;

            // ===================================================================================
            // CREATE NEW RECEIPT DETAIL LINES
            // ===================================================================================
            ReceiptFuelDetail newReceiptDetail = null;
            foreach (var poDetail in selectedPoDetails)
            {
                // Ensure the item is not already on the receipt
                if (_Receipt.ReceiptFuelDetails.Any(rd => rd.PODetailID == poDetail))
                {
                    continue; // Skip item if it's already added
                }

                if (poDetail.RemainingQty > 0)
                {
                    var poDetailInSession = poSession.GetObjectByKey<POrderFuelDetail>(poDetail.Oid);
                    newReceiptDetail = new ReceiptFuelDetail(poSession);
                    newReceiptDetail.GenJournalID = _Receipt;

                    // Copy properties from PO Detail to Receipt Detail
                    newReceiptDetail.ItemNo = poDetailInSession.ItemNo;
                    newReceiptDetail.Description = poDetailInSession.Description;
                    newReceiptDetail.Ordered = poDetailInSession.Quantity;
                    newReceiptDetail.Received = poDetailInSession.Received;
                    newReceiptDetail.Quantity = poDetailInSession.RemainingQty;
                    newReceiptDetail.UOM = poDetailInSession.UOM;
                    newReceiptDetail.Factor = poDetailInSession.Factor;
                    newReceiptDetail.BaseCost = poDetailInSession.BaseCost;
                    newReceiptDetail.Cost = poDetailInSession.Cost;
                    newReceiptDetail.PODetailID = poDetailInSession;
                    newReceiptDetail.Remarks = poDetailInSession.Remarks;
                    newReceiptDetail.RequisitionNo = poDetailInSession.RequisitionNo;
                    newReceiptDetail.CostCenter = poDetailInSession.CostCenter;
                    newReceiptDetail.Warehouse = poDetailInSession.StockTo;
                    newReceiptDetail.RequestedBy = poDetailInSession.RequestedBy;
                    newReceiptDetail.Facility = poDetailInSession.Facility;
                    newReceiptDetail.FacilityHead = poDetailInSession.FacilityHead;
                    newReceiptDetail.Department = poDetailInSession.Department;
                    newReceiptDetail.DepartmentInCharge = poDetailInSession.DepartmentInCharge;
                    newReceiptDetail.PettyCashID = poDetailInSession.PettyCashID;

                    if (poDetailInSession.RequisitionNo != null && poDetailInSession.RequestID != Guid.Empty)
                    {
                        var rws = poSession.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID]=?", poDetailInSession.RequestID));
                        if (rws != null)
                        {
                            newReceiptDetail.ExpenseType = rws.ExpenseType;
                            newReceiptDetail.SubExpenseType = rws.SubExpenseType;
                        }
                    }
                    _Receipt.ReceiptFuelDetails.Add(newReceiptDetail);
                }
            }

            // ===================================================================================
            // CARRY OVER FUEL USAGE DETAILS FROM PO TO RECEIPT (NEW LOGIC)
            // ===================================================================================
            var firstPoHeaderInSession = poSession.GetObjectByKey<PurchaseOrderFuel>(firstPoHeader.Oid);
            foreach (var poUsageDetail in firstPoHeaderInSession.PurchaseOrderFuelUsageDetails)
            {
                var _type = poUsageDetail.TripNo.GetType();

                if (_type.Name == "StanfilcoTrip")
                {
                    var tripInSession = poSession.GetObjectByKey<StanfilcoTrip>(poUsageDetail.TripNo.Oid);

                    FuelRegister fuelRegister = new FuelRegister(poSession);
                    fuelRegister.TripID = tripInSession;
                    fuelRegister.ReferenceNo = tripInSession.DTRNo;
                    fuelRegister.SourceID = _Receipt.Oid;
                    fuelRegister.SourceType = _Receipt.SourceType;
                    fuelRegister.SourceNo = _Receipt.SourceNo;
                    fuelRegister.TruckOrGenset = _Receipt.TruckOrGenset;
                    fuelRegister.TruckNo = _Receipt.TruckNo != null ? _Receipt.TruckNo :
                    null;
                    fuelRegister.GensetNo = _Receipt.GensetNo != null ? _Receipt.
                    GensetNo : null;
                    fuelRegister.Driver = _Receipt.Driver != null ? _Receipt.Driver :
                    null;
                    fuelRegister.Qty = newReceiptDetail.Quantity;
                    fuelRegister.Total = newReceiptDetail.Total;
                    fuelRegister.ReceiptFuelDetailID = newReceiptDetail;
                    fuelRegister.Save();
                    newReceiptDetail.FuelRegister = fuelRegister;
                    _Receipt.TripUsed = tripInSession;
                    ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(poSession);
                    rfud.TripNo = tripInSession;
                    rfud.FuelQty = newReceiptDetail.Quantity;
                    newReceiptDetail.Rfud = rfud;
                    _Receipt.ReceiptFuelUsageDetails.Add(rfud);
                    _Receipt.Save();

                    tripInSession.Save();
                }

                // Dolefil Trip
                if (_type.Name == "DolefilTrip")
                {
                    var tripInSession = poSession.GetObjectByKey<DolefilTrip>(poUsageDetail.TripNo.Oid);

                    FuelRegister fuelRegister = new FuelRegister(poSession);
                    fuelRegister.TripID = tripInSession;
                    fuelRegister.ReferenceNo = tripInSession.DocumentNo;
                    fuelRegister.SourceID = _Receipt.Oid;
                    fuelRegister.SourceType = _Receipt.SourceType;
                    fuelRegister.SourceNo = _Receipt.SourceNo;
                    fuelRegister.TruckOrGenset = _Receipt.TruckOrGenset;
                    fuelRegister.TruckNo = _Receipt.TruckNo != null ? _Receipt.TruckNo :
                    null;
                    fuelRegister.GensetNo = _Receipt.GensetNo != null ? _Receipt.
                    GensetNo : null;
                    fuelRegister.Driver = _Receipt.Driver != null ? _Receipt.Driver :
                    null;
                    fuelRegister.Qty = newReceiptDetail.Quantity;
                    fuelRegister.Total = newReceiptDetail.Total;
                    fuelRegister.ReceiptFuelDetailID = newReceiptDetail;
                    fuelRegister.Save();
                    newReceiptDetail.FuelRegister = fuelRegister;
                    _Receipt.TripUsed = tripInSession;
                    ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(poSession);
                    rfud.TripNo = tripInSession;
                    rfud.FuelQty = newReceiptDetail.Quantity;
                    newReceiptDetail.Rfud = rfud;
                    _Receipt.ReceiptFuelUsageDetails.Add(rfud);
                    _Receipt.Save();

                    tripInSession.Save();
                }

                // OtherTrip
                if (_type.Name == "OtherTrip")
                {
                    var tripInSession = poSession.GetObjectByKey<OtherTrip>(poUsageDetail.TripNo.Oid);

                    FuelRegister fuelRegister = new FuelRegister(poSession);
                    fuelRegister.TripID = tripInSession;
                    fuelRegister.ReferenceNo = tripInSession.TripReferenceNo;
                    fuelRegister.SourceID = _Receipt.Oid;
                    fuelRegister.SourceType = _Receipt.SourceType;
                    fuelRegister.SourceNo = _Receipt.SourceNo;
                    fuelRegister.TruckOrGenset = _Receipt.TruckOrGenset;
                    fuelRegister.TruckNo = _Receipt.TruckNo != null ? _Receipt.TruckNo :
                    null;
                    fuelRegister.GensetNo = _Receipt.GensetNo != null ? _Receipt.
                    GensetNo : null;
                    fuelRegister.Driver = _Receipt.Driver != null ? _Receipt.Driver :
                    null;
                    fuelRegister.Qty = newReceiptDetail.Quantity;
                    fuelRegister.Total = newReceiptDetail.Total;
                    fuelRegister.ReceiptFuelDetailID = newReceiptDetail;
                    fuelRegister.Save();
                    newReceiptDetail.FuelRegister = fuelRegister;
                    _Receipt.TripUsed = tripInSession;
                    ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(poSession);
                    rfud.TripNo = tripInSession;
                    rfud.FuelQty = newReceiptDetail.Quantity;
                    newReceiptDetail.Rfud = rfud;
                    _Receipt.ReceiptFuelUsageDetails.Add(rfud);
                    _Receipt.Save();

                    tripInSession.Save();
                }
            }

            // ===================================================================================
            // REFRESH THE VIEW
            // ===================================================================================
            View.Refresh();
        }
    }
}
