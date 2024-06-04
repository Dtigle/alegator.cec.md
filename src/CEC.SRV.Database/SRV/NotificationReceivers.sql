CREATE TABLE [SRV].[NotificationReceivers]
(
	notificationReceiverId BIGINT IDENTITY NOT NULL,
    notificationId BIGINT NOT null,
    identityUserId NVARCHAR(255) not null,
    notificationIsRead BIT not null DEFAULT 0,
    created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null,
	CONSTRAINT [PK_NotificationReceivers] primary key (notificationReceiverId),
	CONSTRAINT [FK_NotificationReceivers_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_NotificationReceivers_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_NotificationReceivers_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers,
	constraint [FK_NotificationReceivers_Notifications_notificationId] foreign key (notificationId) references SRV.Notifications,
	constraint [FK_NotificationReceivers_SRVIdentityUsers_identityUserId] foreign key (identityUserId) references SRV.SRVIdentityUsers
)

GO

CREATE NONCLUSTERED INDEX [IX_NotificationReceivers_User_IsRead]
ON [SRV].[NotificationReceivers] ([identityUserId],[notificationIsRead])
INCLUDE ([deleted],[deletedById])
