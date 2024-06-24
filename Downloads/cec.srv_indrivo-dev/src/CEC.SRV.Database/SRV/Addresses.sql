CREATE TABLE [SRV].[Addresses] (
	   addressId BIGINT IDENTITY NOT NULL,
       houseNumber INT null,
       [suffix] NVARCHAR(255) null,
	   pollingStationId BIGINT null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       streetId BIGINT not null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
	   [buildingType] INT NOT NULL , 
    [geolatitude] DOUBLE PRECISION NULL, 	  
    [geolongitude] DOUBLE PRECISION NULL, 
    CONSTRAINT [PK_Addresses] primary key (addressId),
    CONSTRAINT [FK_Addresses_Streets_Addresses] FOREIGN KEY (streetId) REFERENCES Lookup.Streets,
	CONSTRAINT [FK_Addresses_PollingStations_Addresses] FOREIGN KEY (pollingStationId) REFERENCES SRV.PollingStations,
	CONSTRAINT [FK_Addresses_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Addresses_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Addresses_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [UQ_LookupStreets] unique nonclustered (streetId asc, [houseNumber] asc, [suffix] asc, [deleted] asc)	
)

GO
CREATE INDEX IX_Addresses_PollingStationId on SRV.Addresses(pollingStationId);
go

CREATE INDEX [IX_Addresses_StreetId] ON [SRV].[Addresses] ([streetId])

GO

CREATE INDEX [IX_Addresses_deleted] 
ON [SRV].[Addresses] ([deleted])
INCLUDE ([addressId],[houseNumber],[suffix],[pollingStationId],[streetId])
