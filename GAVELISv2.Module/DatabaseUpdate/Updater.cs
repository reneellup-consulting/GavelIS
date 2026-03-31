using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Workflow.Xpo;
namespace GAVELISv2.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion): base
            (objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            SecurityUser anonymousUser = ObjectSpace.FindObject<SecurityUser>(
            new BinaryOperator("UserName", SecurityStrategy.AnonymousUserName));
            if (anonymousUser == null) {
                anonymousUser = ObjectSpace.CreateObject<SecurityUser>();
                anonymousUser.UserName = SecurityStrategy.AnonymousUserName;
                anonymousUser.IsActive = true;
                anonymousUser.SetPassword(string.Empty);
                anonymousUser.Save();
            }
            SecurityRole administratorRole = ObjectSpace.FindObject<SecurityRole
            >(new BinaryOperator("Name", SecurityStrategy.AdministratorRoleName)
            );
            if (administratorRole == null) {
                administratorRole = ObjectSpace.CreateObject<SecurityRole>();
                administratorRole.Name = SecurityStrategy.AdministratorRoleName;
                ModelOperationPermissionData modelPermission = ObjectSpace.
                CreateObject<ModelOperationPermissionData>();
                modelPermission.Save();
                administratorRole.BeginUpdate();
                administratorRole.Permissions.GrantRecursive(typeof(object), 
                SecurityOperations.Read);
                administratorRole.Permissions.GrantRecursive(typeof(object), 
                SecurityOperations.Write);
                administratorRole.Permissions.GrantRecursive(typeof(object), 
                SecurityOperations.Create);
                administratorRole.Permissions.GrantRecursive(typeof(object), 
                SecurityOperations.Delete);
                administratorRole.Permissions.GrantRecursive(typeof(object), 
                SecurityOperations.Navigate);
                administratorRole.EndUpdate();
                administratorRole.PersistentPermissions.Add(modelPermission);
                administratorRole.Save();
            }
            string adminName = "Administrator";
            SecurityUser administratorUser = ObjectSpace.FindObject<SecurityUser
            >(new BinaryOperator("UserName", adminName));
            if (administratorUser == null) {
                administratorUser = ObjectSpace.CreateObject<SecurityUser>();
                administratorUser.UserName = adminName;
                administratorUser.IsActive = true;
                administratorUser.SetPassword(string.Empty);
                administratorUser.Roles.Add(administratorRole);
                administratorUser.Save();
            }
            SecurityUser workflowServiceUser = ObjectSpace.FindObject<SecurityUser>(
            new BinaryOperator("UserName", "WorkflowService"));
            if (workflowServiceUser == null)
            {
                workflowServiceUser = ObjectSpace.CreateObject<SecurityUser>();
                workflowServiceUser.UserName = "WorkflowService";
                workflowServiceUser.IsActive = true;
                workflowServiceUser.SetPassword(string.Empty);
                workflowServiceUser.Roles.Add(administratorRole);
                workflowServiceUser.Save();
            }
            SecurityRole defaultRole = ObjectSpace.FindObject<SecurityRole>(new 
            BinaryOperator("Name", "Default"));
            if (defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<SecurityRole>();
                defaultRole.Name = "Default";
                ObjectOperationPermissionData myDetailsPermission = ObjectSpace.
                CreateObject<ObjectOperationPermissionData>();
                myDetailsPermission.TargetType = typeof(SecurityUser);
                myDetailsPermission.Criteria = "[Oid] = CurrentUserId()";
                myDetailsPermission.AllowNavigate = true;
                myDetailsPermission.AllowRead = true;
                myDetailsPermission.Save();
                defaultRole.PersistentPermissions.Add(myDetailsPermission);
                MemberOperationPermissionData userNamePermission = ObjectSpace.
                CreateObject<MemberOperationPermissionData>();
                userNamePermission.TargetType = typeof(SecurityUser);
                userNamePermission.Members = "ChangePasswordOnFirstLogon";
                userNamePermission.AllowWrite = true;
                userNamePermission.Save();
                defaultRole.PersistentPermissions.Add(userNamePermission);
                MemberOperationPermissionData ownPasswordPermission = 
                ObjectSpace.CreateObject<MemberOperationPermissionData>();
                ownPasswordPermission.TargetType = typeof(SecurityUser);
                ownPasswordPermission.Members = "StoredPassword";
                ownPasswordPermission.AllowWrite = true;
                ownPasswordPermission.Save();
                defaultRole.PersistentPermissions.Add(ownPasswordPermission);
            }

            SecurityRole correctEntryDateRole = ObjectSpace.FindObject<SecurityRole>(new 
            BinaryOperator("Name", "CorrectEntryDate"));
            if (correctEntryDateRole == null)
            {
                defaultRole = ObjectSpace.CreateObject<SecurityRole>();
                defaultRole.Name = "CorrectEntryDate";
            }

            SecurityRole showDependencyRole = ObjectSpace.FindObject<SecurityRole>(new
            BinaryOperator("Name", "ShowDependency"));
            if (showDependencyRole == null)
            {
                defaultRole = ObjectSpace.CreateObject<SecurityRole>();
                defaultRole.Name = "ShowDependency";
            }

            SecurityRole reqApprover = ObjectSpace.FindObject<SecurityRole>(new
            BinaryOperator("Name", "REQApprover"));
            if (showDependencyRole == null)
            {
                defaultRole = ObjectSpace.CreateObject<SecurityRole>();
                defaultRole.Name = "REQApprover";
            }
            #region Singleton Initialization
            BusinessObjects.Company.GetInstance(((ObjectSpace)ObjectSpace).
            Session);
            BusinessObjects.AdjustItemCostPrices2.GetInstance(((ObjectSpace)ObjectSpace).
            Session);
            BusinessObjects.DriversEarningsFtm.GetInstance(((ObjectSpace)ObjectSpace).
            Session);
            BusinessObjects.DriversBonusGeneratorHeader.GetInstance(((ObjectSpace)ObjectSpace).
            Session);
            //Create the Singleton object
            //BusinessObjects.AttendanceCalculator.GetInstance(((ObjectSpace)ObjectSpace).Session);
            BusinessObjects.IncomeExpenseReporter.GetInstance(((ObjectSpace)ObjectSpace).Session);
            BusinessObjects.ExpensesAnalyticsHeader.GetInstance(((ObjectSpace)ObjectSpace).Session);
            #endregion
            #region Update Address size/lenght to 500
            if (CurrentDBVersion < new Version("2.0.0.0")
             && CurrentDBVersion > new Version("0.0.0.0"))
            {
                // Receipt
                ExecuteNonQueryCommand(
                "alter table Receipt alter column VendorAddress nvarchar(500)", true);
                ExecuteNonQueryCommand(
                "alter table Receipt alter column ShipToAddress nvarchar(500)", true);

                // CreditMemo
                ExecuteNonQueryCommand(
                "alter table CreditMemo alter column CustomerAddress nvarchar(500)", true);
                ExecuteNonQueryCommand(
                "alter table CreditMemo alter column ShipToAddress nvarchar(500)", true);

                // DebitMemo
                ExecuteNonQueryCommand(
                "alter table DebitMemo alter column VendorAddress nvarchar(500)", true);
                ExecuteNonQueryCommand(
                "alter table DebitMemo alter column BillToAddress nvarchar(500)", true);

                // Invoice
                ExecuteNonQueryCommand(
                "alter table Invoice alter column CustomerAddress nvarchar(500)", true);
                ExecuteNonQueryCommand(
                "alter table Invoice alter column ShipToAddress nvarchar(500)", true);

                // JobOrder
                ExecuteNonQueryCommand(
                "alter table JobOrder alter column VendorAddress nvarchar(500)", true);

                // PurchaseOrder
                ExecuteNonQueryCommand(
                "alter table PurchaseOrder alter column VendorAddress nvarchar(500)", true);
                ExecuteNonQueryCommand(
                "alter table PurchaseOrder alter column ShipToAddress nvarchar(500)", true);

                // PurchaseOrderFuel
                ExecuteNonQueryCommand(
                "alter table PurchaseOrderFuel alter column VendorAddress nvarchar(500)", true);
                ExecuteNonQueryCommand(
                "alter table PurchaseOrderFuel alter column ShipToAddress nvarchar(500)", true);

                // ReceiptFuel
                ExecuteNonQueryCommand(
                "alter table ReceiptFuel alter column VendorAddress nvarchar(500)", true);
                ExecuteNonQueryCommand(
                "alter table ReceiptFuel alter column ShipToAddress nvarchar(500)", true);

                // PurchaseOrderFuel
                ExecuteNonQueryCommand(
                "alter table PurchaseOrderFuel alter column ReferenceNo nvarchar(MAX)", true);

                // SalesOrder
                ExecuteNonQueryCommand(
                "alter table SalesOrder alter column CustomerAddress nvarchar(500)", true);
                ExecuteNonQueryCommand(
                "alter table SalesOrder alter column ShipToAddress nvarchar(500)", true);

                // PayBillExistingCredit
                ExecuteNonQueryCommand(
                "alter table PayBillExistingCredit alter column [Transaction] nvarchar(500)", true);

                // PayBillExistingCharge
                ExecuteNonQueryCommand(
                "alter table PayBillExistingCharge alter column [Transaction] nvarchar(500)", true);

                // TireServiceDetail2
                ExecuteNonQueryCommand(
                "alter table TireServiceDetail2 alter column [Remarks] nvarchar(500)", true);

                // PurchaseOrder
                ExecuteNonQueryCommand(
                "alter table PurchaseOrder alter column [Remarks] nvarchar(MAX)", true);

                // PurchaseOrderDetail
                ExecuteNonQueryCommand(
                "alter table PurchaseOrderDetail alter column [Remarks] nvarchar(MAX)", true);

                // IncomeExpense02 Description 1 & 2
                ExecuteNonQueryCommand(
                "alter table IncomeAndExpense02 alter column [Description1] nvarchar(MAX)", true);
                ExecuteNonQueryCommand(
                "alter table IncomeAndExpense02 alter column [Description2] nvarchar(MAX)", true);

                // InvoiceReconPayment
                ExecuteNonQueryCommand(
                "alter table InvoiceReconPayment alter column [Transaction] nvarchar(MAX)", true);
                // GenJournalDetail
                ExecuteNonQueryCommand(
                "alter table GenJournalDetail alter column [Description] nvarchar(MAX)", true);
                ExecuteNonQueryCommand(
                "alter table GenJournalDetail alter column [Description2] nvarchar(MAX)", true);
                // EmployeeChargeSlipExpenseDetail
                ExecuteNonQueryCommand(
                "alter table EmployeeChargeSlipExpenseDetail alter column [Description] nvarchar(MAX)", true);
                // CheckPayment
                ExecuteNonQueryCommand(
                "alter table CheckPayment alter column [Memo] nvarchar(MAX)", true);
                // PayBillExistingCredit
                ExecuteNonQueryCommand(
                "alter table PayBillExistingCredit alter column [Transaction] nvarchar(MAX)", true);
                // InvoiceReconCharge
                ExecuteNonQueryCommand(
                "alter table InvoiceReconCharge alter column [Transaction] nvarchar(MAX)", true);
                // MultiPayCheckDetails
                ExecuteNonQueryCommand(
                "alter table InvoiceReconCharge alter column [Particulars] nvarchar(1000)", true);
                // Purpose in FuelPumpRegister
                ExecuteNonQueryCommand(
                "alter table FuelPumpRegister alter column [Purpose] nvarchar(MAX)", true);
                // Explanation in PayrollDeductionOther
                ExecuteNonQueryCommand(
                "alter table PayrollDeductionOther alter column [Explanation] nvarchar(MAX)", true);
                // WorkOrder ReferenceNo
                ExecuteNonQueryCommand(
                "alter table WorkOrder alter column [ReferenceNo] nvarchar(1000)", true);
                // EmployeeChargeSlip ReferenceNo
                ExecuteNonQueryCommand(
                "alter table EmployeeChargeSlip alter column [ReferenceNo] nvarchar(1000)", true);
                // DriverPayrollDeduction DeductionName
                ExecuteNonQueryCommand(
                "alter table DriverPayrollDeduction alter column [DeductionName] nvarchar(1000)", true);
                // DriverPayrollDeduction Caption
                ExecuteNonQueryCommand(
                "alter table DriverPayrollDeduction alter column [Caption] nvarchar(1000)", true);
                // DriverPayrollDeduction RefNo
                ExecuteNonQueryCommand(
                "alter table DriverPayrollDeduction alter column [RefNo] nvarchar(1000)", true);
                // ExpenseAnalyticsBuffer Description
                ExecuteNonQueryCommand(
                "alter table ExpenseAnalyticsBuffer alter column [Description] nvarchar(1000)", true);
                // FuelSoaDetail OdoRead
                ExecuteNonQueryCommand(
                "alter table FuelSoaDetail alter column [OdoRead] money", true);
            }
            #endregion
        }
    }
}
