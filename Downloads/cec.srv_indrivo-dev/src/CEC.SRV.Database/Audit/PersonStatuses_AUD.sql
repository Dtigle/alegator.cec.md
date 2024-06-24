CREATE TABLE [Audit].[PersonStatuses_AUD]
(
	personStatusId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
	   personId BIGINT not null,
       statusTypeId BIGINT not null,
       confirmation NVARCHAR(255) not null,
	   isCurrent BIT null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_PersonStatus_AUD] primary key (personStatusId, REV),
	   constraint FKA8A15FB4AAE62361 foreign key (REV) references Audit.REVINFO
)
