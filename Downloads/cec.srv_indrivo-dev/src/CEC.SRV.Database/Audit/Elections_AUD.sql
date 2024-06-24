CREATE TABLE [Audit].[Elections_AUD]
(
	electionId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       electionDate DATETIME null,
	   saiseId BIGINT null,
	   acceptAbroadDeclaration bit null,
       comments NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       electionTypeId BIGINT null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
	   CONSTRAINT [PK_Elections_AUD] primary key (electionId, REV),
	   constraint FK71FC9549AAE62361 foreign key (REV) references Audit.REVINFO
)
