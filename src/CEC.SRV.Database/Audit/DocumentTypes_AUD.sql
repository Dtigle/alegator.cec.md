CREATE TABLE [Audit].[DocumentTypes_AUD]
(
	 documentTypeId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       isPrimary BIT null,
       name NVARCHAR(255) null,
       description NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_DocumentTypes_AUD] primary key (documentTypeId, REV),
	   constraint FKDB4309D4AAE62361 foreign key (REV) references Audit.REVINFO
)
