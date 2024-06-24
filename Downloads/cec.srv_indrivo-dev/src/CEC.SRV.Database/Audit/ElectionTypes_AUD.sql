CREATE TABLE [Audit].[ElectionTypes_AUD]
(
	 electionTypeId BIGINT not null,
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
       CONSTRAINT [PK_ElectionTypes_AUD] primary key (electionTypeId, REV),
	   constraint FKED8F552DAAE62361 foreign key (REV) references Audit.REVINFO
)
