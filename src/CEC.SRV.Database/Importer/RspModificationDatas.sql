CREATE TABLE Importer.[RspModificationDatas]
(
	[rspModificationDataId]  BIGINT IDENTITY NOT NULL,

	idnp nvarchar(25) not null,
	lastName nvarchar(53) not null,
	firstName nvarchar(35) not null,
	secondName nvarchar(35) not null,
	birthdate date not null,
	sexCode nvarchar(2) not null,
	dead bit not null,
	citizenRm bit not null,
	doctypecode int null,
    series nvarchar(9) null,
    number nvarchar(21) null,
    issuedate datetime2 null,
    expirationdate datetime2 null,
    validity bit null,
	statusConflictCode int default 0,	
	acceptConflictCode int default 0,	
	rejectConflictCode int default 0,

	[source] NVARCHAR(255) null,
	[status] NVARCHAR(255) null,
	statusMessage NVARCHAR(255) null,
	created DATETIMEOFFSET null,
	statusDate DATETIMEOFFSET null,
	comments NVARCHAR(255) null,

	constraint [PK_RspModificationDatas] primary key clustered (rspModificationDataId asc)
)

GO

CREATE NONCLUSTERED INDEX [IX_StatusConflictCode_Id]
ON [Importer].[RspModificationDatas] ([statusConflictCode])
INCLUDE ([rspModificationDataId])

GO

CREATE INDEX [IX_RspModificationDatas_ConflictColumn] ON [Importer].[RspModificationDatas] ([statusConflictCode])

GO

CREATE NONCLUSTERED INDEX [IX_RspModificationDatas_statusConflictCode]
ON [Importer].[RspModificationDatas] ([statusConflictCode])
INCLUDE ([rspModificationDataId],[acceptConflictCode])

GO

CREATE NONCLUSTERED INDEX [IX_RspModificationDatas_IsInConflict]
ON [Importer].[RspRegistrationDatas] ([isInConflict])
INCLUDE ([administrativecode],[rspModificationDataId])

GO

CREATE NONCLUSTERED INDEX [IX_RspModficationDatas_idnp]
ON [Importer].[RspModificationDatas] ([idnp])

GO