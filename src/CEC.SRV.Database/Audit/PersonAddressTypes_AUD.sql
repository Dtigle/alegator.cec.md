CREATE TABLE [Audit].[PersonAddressTypes_AUD]
(
	personAddressTypeId BIGINT not null,
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
	   CONSTRAINT [PK_PersonAddressTypes_AUD] primary key (personAddressTypeId, REV),
	   constraint FK45026B43AAE62361 foreign key (REV) references Audit.REVINFO
)
