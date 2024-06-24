CREATE TABLE [SRV].[PollingStations] (
	   pollingStationId BIGINT IDENTITY NOT NULL,
       number NVARCHAR(255) null,
	   subNumber NVARCHAR(64) null,
	   pollingStationType tinyint DEFAULT 0,
       saiseId BIGINT null,
	   location NVARCHAR(255) null,
       
	   geolatitude DOUBLE PRECISION null,
	   geolongitude DOUBLE PRECISION null,
	   
	   pollingStationAddressId BIGINT null,

       contactInfo NVARCHAR(255) null,
	   [regionId] BIGINT NOT NULL, 
       
	   created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,

    CONSTRAINT [PK_PollingStations] primary key (pollingStationId),
	CONSTRAINT [FK_PollingStations_Addresses_pollingStationAddressId] FOREIGN KEY (pollingStationAddressId) REFERENCES SRV.Addresses,
	CONSTRAINT [FK_PollingStations_Regions_regionId] FOREIGN KEY (regionId) REFERENCES Lookup.Regions,
	CONSTRAINT [FK_PollingStations_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_PollingStations_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_PollingStations_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers
	
)

GO

CREATE INDEX [IX_PollingStations_regionId] ON [SRV].[PollingStations] ([regionId])
