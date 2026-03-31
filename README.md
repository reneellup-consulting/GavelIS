# GavelIS (v2) - Fleet & Operations Management System

GavelIS is a robust, enterprise-grade desktop application designed specifically for hauling, trucking, and fleet management businesses. Built using C# and the **DevExpress eXpressApp Framework (XAF)**, it provides an end-to-end solution for managing fleet operations, specialized logistics (e.g., Dolefil, Stanfilco), human resources, inventory, and full-cycle accounting.

## 🚀 Key Features

### 🚛 Fleet & Equipment Management
* **Asset Tracking:** Complete registry and tracking for Trucks, Trailers, and Generator Sets (Gensets).
* **Tire & Battery Management:** Track tire life cycles, inspections, tread statuses, retreads, and battery deployments.
* **Maintenance & Repairs:** Manage preventive maintenance, work orders, mechanics, and revolving parts/small tools.
* **Fuel Management:** Track fuel pump registers, fuel purchase orders, daily purchases, and fuel efficiency/usage.

### 📋 Logistics & Operations
* **Trip Management:** Specialized modules for different operational trip types including Dolefil trips, Stanfilco trips, Shunting, Knockdowns (KD), and standard cargo hauling.
* **Calculations & Tariffs:** Automated trip calculations, tariff driver classification, and turnaround travel time reporting.
* **Job Orders:** Creation and execution of operational Job Orders and Trucking Operations.

### 👥 Human Resources & Payroll
* **Employee Management:** Track drivers, mechanics, and general staff.
* **Attendance & Biometrics:** Direct integration with biometric devices for attendance, overtime, and leave tracking.
* **Driver Payroll:** Advanced driver payroll calculation covering trip-based earnings, commissions, incentives, and various deductions (Dole, non-Dole).
* **Staff Payroll:** Standard corporate payroll generation including 13th-month pay and adjustments.

### 💰 Accounting & Financials
* **General Ledger:** Fully integrated Chart of Accounts, General Journals, and Balance Sheets.
* **Accounts Payable (AP) & Receivable (AR):** Manage vendor balances, customer billing, and statements of account.
* **Invoicing & Billing:** Generate invoices, Cash Sales, Debit/Credit memos, and Check Vouchers.
* **Taxation:** Integrated modules for Monthly VAT declarations, WHT (Withholding Tax) remittances, and Form 2307.

### 📦 Inventory & Warehousing
* **Stock Control:** Multi-warehouse inventory tracking, item movement frequency analysis, and physical adjustments.
* **Transfer Orders:** Inter-warehouse and inter-facility stock transfers.

## 💻 Technology Stack

* **Language:** C# (.NET Framework)
* **Application Framework:** DevExpress eXpressApp Framework (XAF)
* **ORM:** DevExpress eXpress Persistent Objects (XPO)
* **UI Interface:** Windows Forms (WinForms)
* **Background Services:** Windows Workflow Foundation (`Gavel2012.WorkflowService`)

## 📂 Project Structure

The solution follows the standard DevExpress XAF modular architecture:

* `GAVELISv2.Module/`
    * *Platform-agnostic module containing the core business logic, Domain/Business Objects (e.g., `Employee`, `Invoice`, `TruckRegistry`), and generalized controllers.*
* `GAVELISv2.Module.Win/`
    * *Windows Forms-specific module containing UI customizations, custom property editors, and WinForms-based View Controllers.*
* `GAVELISv2.Win/`
    * *The main executable Application Project for Windows. Contains `Program.cs`, connection string configurations, and the `Model.xafml` application model.*
* `Gavel2012.WorkflowService/`
    * *A Windows Service project designed to handle automated, long-running workflow tasks in the background.*

## ⚙️ Getting Started

### Prerequisites
* Visual Studio 2019 or later.
* **DevExpress Universal Subscription** (v12.x or whichever version corresponds to the original build).
* Microsoft SQL Server (Express or higher).

### Installation & Setup
1. Clone the repository:
   ```bash
   git clone [https://github.com/reneellup-consulting/gavelis.git](https://github.com/reneellup-consulting/gavelis.git)
