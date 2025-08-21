-- Fix deprecated fetaures

ALTER TABLE [dbo].[Auditlog] ALTER COLUMN [Parameters] VARCHAR(MAX) NULL;

ALTER TABLE [dbo].[AuditlogArchived] ALTER COLUMN [Parameters] VARCHAR(MAX) NULL;

-- Schema Fixes 
 ALTER TABLE dbo.tSites ADD CONSTRAINT PK_PlantNo PRIMARY KEY CLUSTERED (PlantNo);  -- Creating Primary Key for tSites table.
