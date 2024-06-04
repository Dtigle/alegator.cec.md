CREATE TABLE [SRV].[Notifications]
(
	notificationId BIGINT IDENTITY NOT NULL,
	messageBody nvarchar(255) not null,
	eventId BIGINT not null,
	created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null
	CONSTRAINT [PK_Notifications] primary key (notificationId),
	CONSTRAINT [FK_Notifications_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Notifications_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Notifications_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Notifications_Events_eventId] FOREIGN KEY (eventId) REFERENCES SRV.Events,
)
