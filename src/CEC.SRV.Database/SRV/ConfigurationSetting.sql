create table [SRV].[ConfigurationSettings] (
       configurationSettingId BIGINT IDENTITY NOT NULL,
       [name] NVARCHAR(255) not null,
       [value] NVARCHAR(255) null,
	   [applicationName] nvarchar(255) null,
	   created DATETIMEOFFSET,
	   modified DATETIMEOFFSET,
	   deleted DATETIMEOFFSET,
	   createdById NVARCHAR(255),
	   modifiedById NVARCHAR(255),
	   deletedById NVARCHAR(255),
       CONSTRAINT [PK_ConfigurationSettings] primary key (configurationSettingId),
	   constraint FK_ConfigurationSettings_IdentityUsers_createdById foreign key (createdById) references Access.IdentityUsers,
	   constraint FK_ConfigurationSettings_IdentityUsers_modifiedById foreign key (modifiedById) references Access.IdentityUsers,
	   constraint FK_ConfigurationSettings_IdentityUsers_deletedById foreign key (deletedById) references Access.IdentityUsers,
	   CONSTRAINT [UQ_ConfigurationSettings] unique nonclustered (name asc, applicationName asc, [deleted] asc)
    )

GO

CREATE INDEX [IX_ConfigurationSettings_key] ON [SRV].[ConfigurationSettings] ([name], [applicationName])