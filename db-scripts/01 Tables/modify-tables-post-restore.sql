/* Fix deprecated fetaures */

ALTER TABLE [dbo].[Auditlog] ALTER COLUMN [Parameters] VARCHAR(MAX) NULL;

ALTER TABLE [dbo].[AuditlogArchived] ALTER COLUMN [Parameters] VARCHAR(MAX) NULL;