CREATE TABLE [Audit].[PublicAdministrations_AUD]
(
	 publicAdministrationId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       name NVARCHAR(255) null,
       surname NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       regionId BIGINT null,
       managerTypeId BIGINT null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_PublicAdministrations_AUD] primary key (publicAdministrationId, REV),
	   constraint FK96B483B3AAE62361 foreign key (REV) references Audit.REVINFO
)
