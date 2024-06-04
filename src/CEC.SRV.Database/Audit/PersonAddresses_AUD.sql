CREATE TABLE [Audit].[PersonAddresses_AUD]
(
	  personAddressId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       apSuffix NVARCHAR(10) null,
       apNumber INT null,
       isEligible BIT null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       personId BIGINT null,
       addressId BIGINT null,
	   dateOfRegistration DateTime2 null,
	   dateOfExpiration DateTime2 null,
	   [personAddressTypeId] BIGINT null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_APersonAddresses_AUD] primary key (personAddressId, REV),
	   constraint FK78B52865AAE62361   foreign key (REV) references Audit.REVINFO
)
