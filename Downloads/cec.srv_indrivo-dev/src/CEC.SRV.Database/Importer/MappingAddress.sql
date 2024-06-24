CREATE TABLE Importer.[MappingAddresses]
(
	mappingAddressId BIGINT IDENTITY NOT NULL,
	srvAddressId bigint not null,
	rspAdministrativeCode bigint not null,
	rspStreetCode bigint not null,
	rspHouseNr int null,
	rspHouseSuf nvarchar(50)  null,
	constraint [PK_MappingAddresses] primary key clustered (mappingAddressId asc),
	constraint [FK_MappingAddresses_Addresses_addressId] foreign key (srvAddressId) references SRV.Addresses
)

GO

CREATE INDEX [IX_MappingAddresses_srvAddressId] 
ON [Importer].[MappingAddresses] ([srvAddressId])
INCLUDE ([mappingAddressId],[rspAdministrativeCode],[rspStreetCode],[rspHouseNr],[rspHouseSuf])
