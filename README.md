# 💻 LapAdvisor — Laptop Recommendation System

> **Graduation Project** — Faculty of Information Technology, World Islamic Sciences and Education University (WISE)  
> **Academic Year:** 2025/2026 | **Semester I**

A full-stack, web-based **laptop recommendation and e-commerce platform** that helps users find the right laptop based on their needs, budget, and usage purpose — built with ASP.NET Core MVC and SQL Server.

---

## 📌 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Project Architecture](#-project-architecture)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Database Setup](#-database-setup)

---

## 🔍 Overview

LapAdvisor solves a real problem: choosing the right laptop from hundreds of options is confusing. Most e-commerce platforms don't offer smart, use-case-based guidance — especially for the Jordanian market.

LapAdvisor combines:
- A **rule-based recommendation engine** that scores laptops based on purpose, budget, priority, and screen size
- A **side-by-side comparison tool** to evaluate multiple laptops at once
- Full **e-commerce functionality** (cart, wishlist, orders, checkout)
- A powerful **admin panel** to manage all products and orders
- **External store links** so users can compare prices across local Jordanian stores

---

## ✨ Features

### 👤 Customer Features
| Feature | Description |
|---|---|
| 🔐 Auth | Register & login with role-based access |
| 🛍️ Browse | Browse all available laptops with pagination |
| 🔍 Filter | Filter by brand, type, OS, processor, RAM, price |
| ⭐ Recommend | Get scored laptop recommendations based on purpose & budget |
| ⚖️ Compare | Compare up to 4 laptops side by side (specs, price) |
| 🔗 External Links | View the same laptop on local stores (City Center, PC Circle, GTS) with approximate prices |
| 🛒 Cart | Add/remove/update items, proceed to checkout |
| ❤️ Wishlist | Save and manage favorite laptops |
| 📦 Orders | Place orders and view full order history |
| ✍️ Reviews | Submit ratings and feedback on products |

### 🛠️ Admin Features
| Feature | Description |
|---|---|
| 📊 Dashboard | Overview of weekly sales, orders, and visitors |
| 🖥️ Products | Add / edit / delete laptops with images |
| 🗂️ Categories | Manage laptop categories and brands |
| 📋 Orders | View all customer orders, update status (Pending → Processing → Shipped → Delivered → Cancelled) |

---

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| **Backend** | ASP.NET Core MVC 8.0, C# |
| **ORM** | Entity Framework Core 8.0 |
| **Database** | SQL Server Express |
| **Authentication** | ASP.NET Core Identity |
| **Frontend** | HTML5, CSS3, JavaScript, Bootstrap |
| **Architecture** | MVC + Layered Architecture (Domains → BL → Web) |
| **DI** | Built-in ASP.NET Core Dependency Injection |
| **Version Control** | Git & GitHub |
| **IDE** | Visual Studio 2022 |
| **Methodology** | Agile Scrum (4 sprints × 2 weeks) |

---

## 🏗️ Project Architecture

The system follows a **3-layer architecture**:

```
┌─────────────────────────────────────────────────────┐
│                  LapAdvisor (Web)                   │
│         ASP.NET Core MVC — Controllers, Views       │
│  Areas/Admin | Controllers | Views | ViewModels     │
└──────────────────────┬──────────────────────────────┘
                       │ depends on
┌──────────────────────▼──────────────────────────────┐
│              Bl (Business Logic Layer)              │
│    ClsItems, ClsCategories, ClsSalesInvoice, ...    │
│    LapAdvisorDbContext, Migrations, AppUser         │
└──────────────────────┬──────────────────────────────┘
                       │ depends on
┌──────────────────────▼──────────────────────────────┐
│              Domains (Domain Models)                │
│    TbItem, TbCategory, TbSalesInvoice, TbWishlist   │
│    DTOs: ItemFilterDto, RecommendationRequestDto    │
└─────────────────────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│              SQL Server Database                    │
│                 LapAdvisorDB                        │
└─────────────────────────────────────────────────────┘
```

### Recommendation Engine Flow
```
User selects → [Purpose] [Budget] [Priority] [Screen Size]
                          ↓
              Score calculated per laptop
              (matching criteria = higher score)
                          ↓
              Ranked results displayed
              with Compare / External Links / Add to Cart
```

---

## 📁 Project Structure

```
LapAdvisor/
│
├── LapAdvisor/                    ← Main Web Application (ASP.NET Core MVC)
│   ├── Areas/
│   │   └── admin/                 ← Admin Panel (separate area)
│   │       ├── Controller/        ← CategoriesController, ItemsController, OrdersController
│   │       └── Views/             ← Admin Views (Dashboard, Categories, Items, Orders)
│   ├── Controllers/               ← Customer-facing controllers
│   │   ├── HomeController.cs
│   │   ├── ItemsController.cs
│   │   ├── RecommendationController.cs
│   │   ├── CompareController.cs
│   │   ├── OrderController.cs
│   │   ├── WishlistController.cs
│   │   ├── UsersController.cs
│   │   ├── ContactController.cs
│   │   └── PagesController.cs
│   ├── Views/                     ← Razor Views (.cshtml)
│   ├── Model/                     ← ViewModels (VmHomePage, VmItemDetails, etc.)
│   ├── ApiControllers/            ← REST API endpoints
│   ├── Utlities/                  ← Helper classes, Session extensions
│   ├── Resources/                 ← Localization resources
│   ├── wwwroot/                   ← Static files
│   │   ├── Frontend/              ← Customer-facing CSS/JS/Images
│   │   ├── Admin/                 ← Admin panel CSS/JS
│   │   ├── Custom/                ← Custom CSS/JS overrides
│   │   └── Uploads/               ← Product & slider images
│   ├── appsettings.json           ← App configuration (update connection string!)
│   ├── Program.cs                 ← App entry point & DI configuration
│   └── LapAdvisor.sln             ← Visual Studio Solution file
│
├── Bl/                            ← Business Logic Layer
│   ├── ClsItems.cs                ← Items CRUD + filtering logic
│   ├── ClsCategories.cs           ← Category management
│   ├── ClsSalesInvoice.cs         ← Order processing
│   ├── ClsSalesInvoiceItems.cs    ← Order line items
│   ├── ClsWishlist.cs             ← Wishlist operations
│   ├── ClsFeedback.cs             ← Reviews & ratings
│   ├── ClsItemImages.cs           ← Product image management
│   ├── ClsItemTypes.cs            ← Laptop type management
│   ├── ClsOs.cs                   ← Operating system data
│   ├── ClsSliders.cs              ← Home page sliders
│   ├── ClsPages.cs                ← Static pages management
│   ├── ApplicationUser.cs         ← Extended Identity user
│   ├── LapAdvisorDbContext.cs     ← EF Core DbContext
│   └── Migrations/                ← EF Core migration history
│
├── Domains/                       ← Domain Entities & DTOs
│   ├── TbItem.cs                  ← Laptop entity
│   ├── TbCategory.cs              ← Category entity
│   ├── TbSalesInvoice.cs          ← Order/Invoice entity
│   ├── TbSalesInvoiceItem.cs      ← Order line item entity
│   ├── TbWishlist.cs              ← Wishlist entity
│   ├── TbFeedback.cs              ← Review entity
│   ├── TbSlider.cs                ← Slider entity
│   ├── TbItemImage.cs             ← Product image entity
│   ├── TbItemType.cs              ← Item type entity
│   ├── TbO.cs                     ← OS entity
│   ├── TbPages.cs                 ← Static page entity
│   ├── TbPurchaseInvoice.cs       ← Purchase invoice entity
│   ├── TbSupplier.cs              ← Supplier entity
│   ├── VwItem.cs                  ← Item view/projection
│   ├── VwItemCategory.cs          ← Item + Category view
│   ├── VwSalesInvoice.cs          ← Invoice view
│   └── Dtos/
│       ├── ItemFilterDto.cs       ← Laptop filter parameters
│       └── RecommendationRequestDto.cs ← Recommendation input
│
├── Database/
│   └── LapAdvisorDB               ← SQL Server database backup
│
├── Docs/
│   └── LapAdvisor_Documentation.pdf  ← Full project report
│
├── .gitignore
└── README.md
```

---

## 🚀 Getting Started

### Prerequisites

Make sure you have these installed:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Developer)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with ASP.NET and web development workload)

### Setup Steps

**1. Clone the repository**
```bash
git clone https://github.com/YOUR_USERNAME/LapAdvisor.git
cd LapAdvisor
```

**2. Restore the database**

Open SSMS, connect to your SQL Server instance, then:
- Right-click on **Databases** → **Restore Database**
- Choose **Device** → browse to `Database/LapAdvisorDB`
- Click **OK** to restore

**3. Update the connection string**

Open `LapAdvisor/appsettings.json` and update the server name:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=LapAdvisorDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
> Replace `YOUR_SERVER_NAME` with your SQL Server instance name (e.g., `localhost`, `.\SQLEXPRESS`, or your machine name).

**4. Open the solution**

Open `LapAdvisor/LapAdvisor.sln` in Visual Studio 2022.

**5. Build and run**
```bash
# From the solution root
dotnet build
dotnet run --project LapAdvisor/LapAdvisor.csproj
```
Or simply press **F5** in Visual Studio.

**6. Access the app**
- **Customer site:** `https://localhost:7291`
- **Admin panel:** `https://localhost:7291/admin`

---

## 🗄️ Database Setup

The database backup is located at `Database/LapAdvisorDB`.

If you prefer running migrations instead of restoring the backup:
```bash
cd Bl
dotnet ef database update --startup-project ../LapAdvisor/LapAdvisor.csproj
```

> ⚠️ **Important:** Make sure the connection string in `appsettings.json` is updated to point to your local SQL Server before running migrations.


---

## 📄 License

This project was developed as a graduation project for academic purposes.
