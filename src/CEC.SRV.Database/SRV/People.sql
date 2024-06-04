CREATE TABLE [SRV].[People] (
	   personId BIGINT IDENTITY NOT NULL,
       idnp NVARCHAR(13) not null unique,
       firstName NVARCHAR(255) not null,
       surname NVARCHAR(255) not null,
       middleName NVARCHAR(255) null,
	   [dateOfBirth] DATETIME2 NOT NULL, 
       genderId BIGINT not null,
	   exportedToSaise BIT not null DEFAULT (0),
	   created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
	   
	   doc_seria NVARCHAR(32) NULL,
	   doc_number NVARCHAR(64) NULL,
	   doc_issuedDate DATETIME2 NULL, 
	   doc_issuedBy NVARCHAR(50) NULL, 
       doc_validBy DATETIME2 NULL, 
	   doc_typeId BIGINT NULL,

    [comments] NVARCHAR(255) NULL, 
	alegatorId BIGINT NULL,
    CONSTRAINT [PK_People] primary key (personId),
	CONSTRAINT [UX_People_IDNP] UNIQUE(idnp),
    CONSTRAINT [FK_People_Genders_genderId] FOREIGN KEY (genderId) REFERENCES Lookup.Genders,
	CONSTRAINT [FK_People_DocumentTypes_typeId] FOREIGN KEY (doc_typeId) REFERENCES Lookup.DocumentTypes,
	
	CONSTRAINT [FK_People_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_People_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_People_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers

)

GO

CREATE NONCLUSTERED INDEX [IX_People_IDNP] ON [SRV].[People](idnp)

GO

CREATE INDEX IX_People_DELETED on SRV.People(deleted)

GO

CREATE INDEX IX_Perople_DateOfBirth on SRV.People(dateOfBirth)
GO

CREATE NONCLUSTERED INDEX [IX_People_GenderId_Deleted] ON [SRV].[People] ([genderId],[deleted]) INCLUDE ([personId])

GO

CREATE INDEX IX_Perople_Surname on SRV.People(surname)

GO

CREATE INDEX IX_Perople_FirstName on SRV.People(firstName)

GO

CREATE INDEX IX_People_ExportedToSaise ON SRV.People(exportedToSaise)
GO