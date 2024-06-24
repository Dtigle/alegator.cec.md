CREATE TABLE [Audit].[RegionTypes_AUD]
(
	regionTypeId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       [rank] TINYINT null,
       name NVARCHAR(255) null,
       description NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_RegionTypes_AUD] primary key (regionTypeId, REV),
	   constraint FK92BD8DAAAAE62361 foreign key (REV) references Audit.REVINFO
)
