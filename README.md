# Leave Management System

A lightweight, secure, and easy-to-use **Leave Management System** built with **ASP.NET Core MVC** and **Microsoft SQL Server (SSMS)**. It features separate user and admin roles, dynamic leave balance tracking via a ledger system, and a modern Bootstrap-powered UI.

## ✨ Features

* **User Authentication:** Secure registration and login system with role-based routing (User vs. Admin).
* **Automated Leave Ledger:** Every new user automatically receives initial leave credits (Casual, Privilege, Maternity, etc.).
* **Leave Application & Tracking:** Employees can apply for leaves, track pending/approved/rejected statuses, and view real-time remaining leave balances.
* **Admin Dashboard:** Admins can view all company-wide leave requests and approve/reject them.
* **Automatic Balance Deduction:** Approving a leave automatically debits the correct number of days from the user's leave balance ledger.
* **Responsive UI:** Built using Bootstrap 5 and clean custom CSS.

## 🛠️ Tech Stack

* **Backend:** C#, ASP.NET Core MVC, Entity Framework Core
* **Database:** Microsoft SQL Server (SSMS)
* **Frontend:** HTML5, CSS3, Bootstrap 5, Razor Views (.cshtml)

## 🚀 Getting Started Locally

Follow these steps to set up and run the project on your local machine.

### 1. Clone the Repository
```bash
git clone https://github.com
cd LeaveManagementSystem
```

### 2. Configure the Database Connection
1. Open **SQL Server Management Studio (SSMS)** and ensure your local SQL Server instance is running.
2. Open the `appsettings.json` file in the root of the project.
3. Update the `DefaultConnection` string with your local server credentials:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=LeaveManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

### 3. Apply Database Migrations
Run the following commands in your terminal or Package Manager Console to create the database schema:
```bash
dotnet ef database update
```

### 4. Run the Application
Start the development server:
```bash
dotnet run
```
Once started, navigate to `https://localhost:5001` or `http://localhost:5000` in your web browser to access the system.
