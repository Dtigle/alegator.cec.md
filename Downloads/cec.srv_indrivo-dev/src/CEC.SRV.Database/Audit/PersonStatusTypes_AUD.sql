CREATE TABLE [Audit].[PersonStatusTypes_AUD]
(
	 personStatusTypeId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       isExcludable BIT null,
       name NVARCHAR(255) null,
       description NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_PersonStatusTypes_AUD] primary key (personStatusTypeId, REV),
	   constraint FK4A289073AAE62361 foreign key (REV) references Audit.REVINFO
)
