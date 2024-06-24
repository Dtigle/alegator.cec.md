CREATE TABLE [Audit].[Regions_AUD]
(
	  regionId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       registruId BIGINT null,
       saiseId BIGINT null,
       hasStreets BIT null,
       name NVARCHAR(255) null,
       description NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       parentId BIGINT null,
       regionTypeId BIGINT null,
       publicAdministrationId BIGINT null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       [circumscription] INT NULL, 
    CONSTRAINT [PK_Regions_AUD] primary key (regionId, REV),
	   constraint FK11F9767EAAE62361   foreign key (REV) references Audit.REVINFO
)
