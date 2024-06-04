create table [Audit].[ConfigurationSettings_AUD] (
       configurationSettingId BIGINT NOT NULL,
	   REV INT not null,
       REVTYPE TINYINT not null,
       [name] NVARCHAR(255) not null,
       [value] NVARCHAR(255) null,
	   [applicationName] nvarchar(255) null,
	   created DATETIMEOFFSET,
	   modified DATETIMEOFFSET,
	   deleted DATETIMEOFFSET,
	   createdById NVARCHAR(255),
	   modifiedById NVARCHAR(255),
	   deletedById NVARCHAR(255),
       CONSTRAINT [PK_ConfigurationSettings_AUD] primary key (configurationSettingId, REV),
	   CONSTRAINT [FK_ConfigurationSettings_AUD_REV] foreign key (REV) references Audit.REVINFO
    )

GO

CREATE INDEX [IX_ConfigurationSettings_AUD_key] ON [Audit].[ConfigurationSettings_AUD] ([name], [applicationName])