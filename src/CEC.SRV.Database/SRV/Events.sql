CREATE TABLE [SRV].[Events]
(
	eventId BIGINT IDENTITY NOT NULL,
	eventType INT not null,
	entityType NVARCHAR(255) not null,
	entityId BIGINT not null,
	created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null
	CONSTRAINT [PK_Events] primary key (eventId),
	CONSTRAINT [FK_Events_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Events_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Events_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers
)
