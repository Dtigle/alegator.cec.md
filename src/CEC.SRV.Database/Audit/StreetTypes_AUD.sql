CREATE TABLE [Audit].[StreetTypes_AUD]
(
	 streetTypeId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       name NVARCHAR(255) null,
       description NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_StreetTypes_AUD] primary key (streetTypeId, REV),
	   constraint FKAE6ED247AAE62361 foreign key (REV) references Audit.REVINFO
)
