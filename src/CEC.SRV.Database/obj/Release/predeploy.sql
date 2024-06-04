/*
Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--print 'Post steps for $(BuildConfiguration)' 

/* User and Roles */
print 'Migrating Person Documents' 
set xact_abort on

IF EXISTS(SELECT 1 FROM sys.tables WHERE name = N'PersonDocuments' and schema_id = SCHEMA_ID('SRV'))
BEGIN
	begin tran
		if not exists(select 1 from sys.columns where name = N'doc_number' and object_id = OBJECT_ID(N'[SRV].[People]'))
		begin
		ALTER TABLE [SRV].People ADD doc_seria NVARCHAR(32) NULL,
		   doc_number NVARCHAR(64) NULL,
		   doc_issuedDate DATETIME2 NULL, 
		   doc_issuedBy NVARCHAR(50) NULL, 
		   doc_validBy DATETIME2 NULL, 
		   doc_typeId BIGINT NULL
		end

		exec('UPDATE p SET
			p.doc_seria = pd.seria,
			p.doc_number = pd.number,
			p.doc_issuedDate = pd.issuedDate,
			p.doc_issuedBy = pd.issuedBy,
			p.doc_validBy = pd.validBy,
			p.doc_typeId = pd.typeId

		FROM [SRV].[PersonDocuments] pd
		INNER JOIN [SRV].People p ON pd.personId = p.personId')

		DROP TABLE [SRV].PersonDocuments

	commit
END
set xact_abort off
print 'Migrating Person Documents succeeded.' 
GO
