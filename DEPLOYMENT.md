# 🚀 LabControl Pro — Production Deployment & Run Guide

This guide provides end-to-end instructions for building, running, containerizing, and deploying the **LabControl Pro** (*Industrie*) application across local environments, Docker hosts, and modern cloud providers.

---

## 📋 Table of Contents

1. [Architectural Overview & Requirements](#1-architectural-overview--requirements)
2. [Local Development (Run & Debug)](#2-local-development-run--debug)
3. [Database Setup (Supabase PostgreSQL)](#3-database-setup-supabase-postgresql)
4. [Docker Container Deployment](#4-docker-container-deployment)
5. [Cloud Platform Deployments](#5-cloud-platform-deployments)
   - [A. Deploying to Render.com](#a-deploying-to-rendercom)
   - [B. Deploying to Railway.app](#b-deploying-to-railwayapp)
   - [C. Deploying to Azure App Service](#c-deploying-to-azure-app-service)
   - [D. Deploying to Fly.io](#d-deploying-to-flyio)
   - [E. Vercel Web Portal (`main-nu.vercel.app`)](#e-vercel-web-portal-main-nuvercelapp)
6. [Environment Variables Reference](#6-environment-variables-reference)
7. [Production Hardening Checklist](#7-production-hardening-checklist)

---

## 1. Architectural Overview & Requirements

**LabControl Pro** is built on **ASP.NET Core 9 Blazor Server**. Unlike single-page applications (SPAs) or static sites, Blazor Server maintains a stateful, real-time **SignalR circuit (WebSocket)** between the browser client and the server process.

### Runtime Requirements
- **WebSockets / Persistent Connections**: The host platform must support long-lived HTTP/WebSocket connections.
- **Port Binding**: Kestrel defaults to port `5014` (HTTP) and `7111` (HTTPS) locally, and port `8080` in Docker.
- **Memory**: Minimum 256 MB RAM (512 MB recommended for 50+ concurrent operator circuits).
- **CPU**: 0.5 vCPU minimum.

---

## 2. Local Development (Run & Debug)

### Option A: Via .NET CLI
```bash
# Navigate to the project directory
cd Industrie

# Restore packages and compile
dotnet build

# Launch with dual HTTP/HTTPS bindings
dotnet run --launch-profile https
```

**Active Endpoints**:
- **Plain HTTP**: `http://localhost:5014` *(Recommended for quick local testing)*
- **HTTPS**: `https://localhost:7111`

### Option B: Via Visual Studio 2022+
1. Open `Industrie.sln`.
2. Select the `https` profile from the debug target dropdown.
3. Press `F5` to start debugging.

---

## 3. Database Setup (Supabase PostgreSQL)

LabControl Pro natively supports **Supabase PostgreSQL** via Entity Framework Core (`Npgsql`).

### Step 1: Execute Database DDL
1. Log in to your [Supabase Dashboard](https://supabase.com/dashboard).
2. Open the **SQL Editor** tab.
3. Copy and run the schema script located at `Industrie/supabase_schema.sql`:

```sql
-- Creates tables with ON DELETE RESTRICT foreign keys
-- Sites, OSList, Ecrans, Machines
```

### Step 2: Configure the Connection String
In your production environment or local `Industrie/appsettings.json`, set:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-0-eu-central-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres;Password=YOUR_DB_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;",
    "Provider": "PostgreSql"
  }
}
```

> [!TIP]
> Use the **Session Pooler** (Port `6543`) or **Transaction Pooler** from Supabase for optimal scalability with multiple app instances.

---

## 4. Docker Container Deployment

The repository includes a production-ready multi-stage `Dockerfile` based on Microsoft's official .NET 9 images.

### Step 1: Build the Image
From the repository root:
```bash
docker build -t labcontrol-pro:latest .
```

### Step 2: Run the Container
```bash
docker run -d \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=YOUR_HOST;Port=6543;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;" \
  -e ConnectionStrings__Provider="PostgreSql" \
  -e Encryption__Key="IndustrieLabControl2026SecretKey!" \
  --name labcontrol \
  labcontrol-pro:latest
```

Open `http://localhost:8080` in your browser.

### Docker Compose Example (`docker-compose.yml`)
```yaml
version: '3.8'

services:
  labcontrol:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__DefaultConnection=Host=aws-0-eu-central-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres;Password=YOUR_DB_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;
      - ConnectionStrings__Provider=PostgreSql
      - Encryption__Key=IndustrieLabControl2026SecretKey!
    restart: always
```

---

## 5. Cloud Platform Deployments

### A. Deploying to Render.com
1. Create an account at [Render.com](https://render.com).
2. Click **New +** -> **Web Service**.
3. Connect your GitHub repository: `https://github.com/itshydraaaaaa/main`.
4. Configure service settings:
   - **Runtime**: `Docker`
   - **Region**: Frankfurt (EU Central) or closest to Supabase
   - **Instance Type**: Free or Starter ($7/mo)
5. Under **Environment Variables**, add:
   - `ConnectionStrings__DefaultConnection`: *Your Supabase connection string*
   - `ConnectionStrings__Provider`: `PostgreSql`
   - `Encryption__Key`: *Your 32-character AES-GCM key*
6. Click **Create Web Service**. Render will automatically build the `Dockerfile` and provide a public URL (`https://labcontrol-pro.onrender.com`).

---

### B. Deploying to Railway.app
1. Go to [Railway.app](https://railway.app).
2. Click **New Project** -> **Deploy from GitHub repo**.
3. Select `itshydraaaaaa/main`.
4. Railway will automatically detect the `Dockerfile` in the root directory.
5. In **Variables**, add:
   - `PORT`: `8080`
   - `ConnectionStrings__DefaultConnection`: *Your Supabase string*
   - `ConnectionStrings__Provider`: `PostgreSql`
   - `Encryption__Key`: *Your secret key*
6. Click **Deploy**.

---

### C. Deploying to Azure App Service
1. Create a **Web App** in the Azure Portal.
2. Select:
   - **Publish**: `Docker Container`
   - **Operating System**: `Linux`
3. Configure the container source pointing to your GitHub or Azure Container Registry (ACR).
4. Under **Configuration** -> **General Settings**:
   - Turn **WebSockets** to `On` *(Mandatory for Blazor Server SignalR circuits)*.
   - Turn **ARR Affinity (Sticky Sessions)** to `On`.
5. Under **Application Settings**, add your connection string and encryption key.

---

### D. Deploying to Fly.io
```bash
# Install flyctl and authenticate
fly auth login

# Launch configuration
fly launch --no-deploy

# Set secret environment variables
fly secrets set ConnectionStrings__DefaultConnection="YOUR_CONN_STRING"
fly secrets set Encryption__Key="IndustrieLabControl2026SecretKey!"

# Deploy
fly deploy
```

---

### E. Vercel Web Portal (`main-nu.vercel.app`)

Vercel hosts the static showcase and architecture portal from the repository root (`index.html` + `vercel.json`):

- **Public URL**: [https://main-nu.vercel.app/](https://main-nu.vercel.app/)
- **Deployment Protection**: If Vercel Deployment Protection is active on your account, append your bypass token to the URL:
  `https://main-nu.vercel.app/?_vercel_protection_bypass=YOUR_VERCEL_TOKEN`

---

## 6. Environment Variables Reference

| Variable Name | Required | Default / Example | Purpose |
| :--- | :---: | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Yes | `Production` | App environment (`Development` activates AD bypass and InMemory fallback). |
| `ASPNETCORE_URLS` | Yes | `http://+:8080` | Network sockets Kestrel listens on. |
| `ConnectionStrings__DefaultConnection` | Optional | `Host=...;Port=6543;Database=...` | Supabase PostgreSQL or SQL Server connection string. |
| `ConnectionStrings__Provider` | Optional | `PostgreSql` | Database provider (`PostgreSql`, `SqlServer`, `InMemory`). |
| `Encryption__Key` | Yes | 32-character string | Master key used for AES-256 GCM encryption. |

---

## 7. Production Hardening Checklist

- [x] **Relational Integrity**: Foreign keys configured with `ON DELETE RESTRICT` to protect production equipment.
- [x] **Decryption Fault Tolerance**: Decryption catches corrupted rows cleanly without terminating the SignalR circuit.
- [x] **Unmanaged Memory Management**: Active Directory `PrincipalContext` instances disposed deterministically.
- [x] **Sticky Sessions / WebSocket Support**: Enabled on reverse proxies and container orchestrators.
- [x] **Secret Sanitation**: No credentials or API tokens committed to version control.
