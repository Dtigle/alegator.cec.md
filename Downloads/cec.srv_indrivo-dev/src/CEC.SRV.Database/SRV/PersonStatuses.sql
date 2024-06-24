CREATE TABLE [SRV].[PersonStatuses]
(
	personStatusId BIGINT IDENTITY NOT NULL,
       personId BIGINT not null,
       statusTypeId BIGINT not null,
       confirmation NVARCHAR(255) not null,
	   isCurrent BIT not null default (1),
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
	   CONSTRAINT [PK_PersonStatuses]  PRIMARY KEY NONCLUSTERED (personStatusId) ,
		constraint FK_PersonStatuses_People_PersonStatuses foreign key (personId) references SRV.People,
		constraint [FK_PersonStatuses_PersonStatusTypes_statusTypeId] foreign key (statusTypeId) references Lookup.PersonStatusTypes,
	
		CONSTRAINT [FK_PersonStatuses_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
		CONSTRAINT [FK_PersonStatuses_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
		CONSTRAINT [FK_PersonStatuses_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers,

		
)

GO

CREATE CLUSTERED INDEX UX_PersonStatuses_PersonId_isCurrent on SRV.PersonStatuses(personId,isCurrent);
GO

CREATE INDEX IX_PersonStatuses_statusTypeid on SRV.PersonStatuses(statusTypeId);

GO


CREATE NONCLUSTERED INDEX [IX_PersonStatus_IsCurrent]
ON [SRV].[PersonStatuses] ([isCurrent])
INCLUDE ([personId])

GO

CREATE NONCLUSTERED INDEX [IX_PersonStatuses_IsCurrent_w_StatusTypeId]
ON [SRV].[PersonStatuses] ([isCurrent])
INCLUDE ([personId],[statusTypeId])
