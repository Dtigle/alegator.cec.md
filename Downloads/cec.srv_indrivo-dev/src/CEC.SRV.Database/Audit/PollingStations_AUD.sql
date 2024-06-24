CREATE TABLE [Audit].[PollingStations_AUD]
(
	 pollingStationId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       number NVARCHAR(255) null,
	   subNumber NVARCHAR(64) null,
       contactInfo NVARCHAR(255) null,
	   pollingStationType tinyint null,
       saiseId BIGINT null,
       location NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       geolatitude DOUBLE PRECISION null,
       geolongitude DOUBLE PRECISION null,
       pollingStationAddressId BIGINT null,
       regionId BIGINT null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
       CONSTRAINT [PK_PollingStations_AUD] primary key (pollingStationId, REV),
	   constraint FK70D9CAE6AAE62361  foreign key (REV) references Audit.REVINFO
)
