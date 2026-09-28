# LabControl Pro (Industrie) - System Analysis & Architecture Report

## 1. Executive Summary & Purpose
**LabControl Pro** is an industrial IT/OT workstation supervision and compliance tracking web application developed in .NET 9 and Blazor Server. It addresses critical disaster recovery, credential management, and hardware inventory needs across pharmaceutical/industrial production sites.

## 2. Architecture & Components
- **Frontend / UI**: Blazor Interactive Server with Radzen.Blazor UI suite.
- **Service Layer**: MachineService (CRUD), EncryptionService (AES-GCM 256-bit), ActiveDirectoryService (LDAP SSO), DbInitializer.
- **Data Access**: EF Core 9 with Npgsql (Supabase PostgreSQL), SqlServer, and InMemory fallback.
- **Security**: Transparent EF Core ValueConverter encrypting passwords at rest with AES-GCM authenticated cipher.

## 3. Status Matrix
- **Dashboard (`/`)**: ✅ WORKING (KPI cards, Donut chart, OS column chart, Radial gauge, unbacked machines alert table).
- **Registre (`/machines`)**: ✅ WORKING (Inventory DataGrid, multi-criteria search/filtering, reversible AES-GCM password reveal eye toggle, safe delete).
- **Form (`/machines/form`)**: ✅ WORKING (Entity binding, validation, drop-down selection, backup date picker).
- **Paramétrage (`/parametrage`)**: ✅ WORKING (Sites, Ecrans, OS tabs with modal CRUD and cascade safeguards).
- **Database Engine**: ✅ WORKING (PostgreSQL DDL schema, EF Core mapping, automated seed data).
- **Remote Supabase Connection**: ⏳ PENDING USER CONFIG (Requires entering database password/URL in `appsettings.json`).
- **Active Directory SSO**: ⏳ PENDING DOMAIN (Requires physical connection to `MEDIS-NABEUL` domain; gracefully bypassed in development).
