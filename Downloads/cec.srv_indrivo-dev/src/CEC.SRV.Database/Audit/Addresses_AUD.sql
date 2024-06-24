CREATE TABLE [Audit].[Addresses_AUD]
(
	 addressId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       buildingType INT null,
       houseNumber BIGINT null,
       suffix NVARCHAR(255) null,
	   [geolatitude] DOUBLE PRECISION NULL, 
       [geolongitude] DOUBLE PRECISION NULL, 
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       streetId BIGINT null,
       pollingStationId BIGINT null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,     
    CONSTRAINT [PK_Addresses_AUD] primary key (addressId, REV),
	   constraint FK458CF8FCAAE62361  foreign key (REV) references Audit.REVINFO
)
