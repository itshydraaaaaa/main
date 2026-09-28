-- =============================================================================
-- Supabase PostgreSQL Schema for LabControl Pro (Industrie)
-- Generated for project database setup
-- =============================================================================

-- 1. Sites / Emplacements
CREATE TABLE IF NOT EXISTS public."Sites" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL
);

-- 2. Systèmes d'Exploitation (OS)
CREATE TABLE IF NOT EXISTS public."OSList" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL
);

-- 3. Types d'Écran
CREATE TABLE IF NOT EXISTS public."Ecrans" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL
);

-- 4. Parc Machines
CREATE TABLE IF NOT EXISTS public."Machines" (
    "Code" VARCHAR(450) PRIMARY KEY,
    "Name" TEXT NOT NULL DEFAULT '',
    "PcName" TEXT NOT NULL DEFAULT '',
    "Session" TEXT NOT NULL DEFAULT '',
    "Mdp" TEXT NOT NULL DEFAULT '',
    "AdresseIpExterne" TEXT NOT NULL DEFAULT '',
    "AdresseIpInterne" TEXT NOT NULL DEFAULT '',
    "Bukup" BOOLEAN NOT NULL DEFAULT FALSE,
    "DateBukup" DATE NOT NULL DEFAULT CURRENT_DATE,
    "EcranId" INTEGER NOT NULL REFERENCES public."Ecrans"("Id") ON DELETE RESTRICT,
    "OSId" INTEGER NOT NULL REFERENCES public."OSList"("Id") ON DELETE RESTRICT,
    "SiteId" INTEGER NOT NULL REFERENCES public."Sites"("Id") ON DELETE RESTRICT
);

-- Indexation des clés étrangères pour optimiser les requêtes LINQ / EF Core
CREATE INDEX IF NOT EXISTS "IX_Machines_EcranId" ON public."Machines"("EcranId");
CREATE INDEX IF NOT EXISTS "IX_Machines_OSId" ON public."Machines"("OSId");
CREATE INDEX IF NOT EXISTS "IX_Machines_SiteId" ON public."Machines"("SiteId");

-- 5. Données de Référence Initiales (Seed Data)
INSERT INTO public."Sites" ("Name")
VALUES 
    ('Site Alpha - Usine Nord'),
    ('Site Bêta - Laboratoire Principal'),
    ('Site Gamma - Zone Conditionnement')
ON CONFLICT DO NOTHING;

INSERT INTO public."OSList" ("Name")
VALUES 
    ('Windows 11 Pro'),
    ('Windows 10 IoT Enterprise'),
    ('Ubuntu 24.04 LTS'),
    ('Debian 12')
ON CONFLICT DO NOTHING;

INSERT INTO public."Ecrans" ("Name")
VALUES 
    ('Écran Tactile Industriel 24"'),
    ('Double Écran 27" 4K'),
    ('Dalle Intégrée Siemens 15"')
ON CONFLICT DO NOTHING;
