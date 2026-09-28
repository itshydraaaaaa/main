# LabControl Pro (Industrie) - Complete Walkthrough & Documentation

## 1. Executive Summary

**LabControl Pro** is a modern industrial supervision and workstation management web application built with **.NET 9**, **ASP.NET Core Blazor Interactive Server**, and the **Radzen.Blazor** UI component library.

---

## 2. Architecture & Tech Stack

- **Framework**: .NET 9 (C#)
- **Frontend**: Blazor Interactive Server (real-time DOM updates via SignalR)
- **UI Suite**: Radzen.Blazor 11.2 (DataGrid, Radial Gauges, Donut & Bar Charts, Modals)
- **Database Engine**: Supabase PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`) with local InMemory fallback
- **Cryptography**: Authenticated AES-GCM (256-bit) with random nonces & authentication tags

---

## 3. Supabase Setup Instructions

1. Log into your [Supabase Dashboard](https://supabase.com/dashboard).
2. Go to **SQL Editor** and execute the provided `supabase_schema.sql` script located in the `Industrie/` directory.
3. Retrieve your project connection string from **Project Settings > Database > Connection Pooling (or Direct Connection)**.
4. Set your connection string in `Industrie/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-0-eu-central-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.[PROJECT-REF];Password=[YOUR-PASSWORD];SSL Mode=Require;Trust Server Certificate=true;",
    "Provider": "PostgreSql"
  },
  "Supabase": {
    "Url": "https://[YOUR-PROJECT-REF].supabase.co",
    "PublishableKey": "YOUR_SUPABASE_PUBLISHABLE_KEY",
    "SecretKey": "YOUR_SUPABASE_SECRET_KEY"
  }
}
```

---

## 4. How to Run the Application

```powershell
# Ensure .NET 9 is in your PATH
$env:PATH = "C:\Users\MSI\.dotnet;" + $env:PATH

# Navigate to the project directory
cd Industrie

# Run the web application
dotnet run --urls "http://localhost:5014"
```

Access the app in your browser at: `http://localhost:5014`

---

## 5. Application Features

- **Dashboard (`/`)**: Real-time KPI summaries, equipment distribution by site (donut chart) and OS (column chart), backup compliance gauge, and critical unsecured workstation alerts.
- **Machine Registry (`/machines`)**: Multi-attribute search and filtering, reversible AES-GCM password reveal eye toggle, network IP inspection, and safe deletion.
- **Machine Form (`/machines/form`)**: Add and edit machine records with automated date picker, password management, and drop-down relations.
- **Hardware Parameters (`/parametrage`)**: Tabbed management for Sites, Screens, and Operating Systems with full CRUD dialogs.
