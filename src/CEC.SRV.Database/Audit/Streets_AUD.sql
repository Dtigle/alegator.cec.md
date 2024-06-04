CREATE TABLE [Audit].[Streets_AUD]
(
	streetId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       ropId BIGINT null,
       saiseId BIGINT null,
       name NVARCHAR(255) null,
       description NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       regionId BIGINT null,
       streetTypeId BIGINT null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_Streets_AUD] primary key (streetId, REV),
	   constraint FK34692601AAE62361 foreign key (REV) references Audit.REVINFO
)
