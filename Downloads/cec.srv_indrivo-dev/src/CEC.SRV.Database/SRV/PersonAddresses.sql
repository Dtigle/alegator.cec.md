CREATE TABLE [SRV].[PersonAddresses] (
	   personAddressId BIGINT IDENTITY NOT NULL,
       [apNumber] INT null,
	   [apSuffix] nvarchar(10) null,
       isEligible BIT null,
	   dateOfRegistration DATETIME2 not null default '0001-01-01',
	   dateOfExpiration datetime2,
       personId BIGINT not null,
       addressId BIGINT not null,
       [personAddressTypeId] BIGINT not null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,

    CONSTRAINT [PK_PersonAddresses] primary key NONCLUSTERED (personAddressId),
	CONSTRAINT [UX_PersonAddresses_PersonId_AddressId] UNIQUE(personId asc, addressId asc, deleted asc),
    CONSTRAINT [FK_PersonAddresses_People_Addresses] FOREIGN KEY (personId) REFERENCES SRV.People,
    CONSTRAINT [FK_PersonAddresses_Addresses_addressId] FOREIGN KEY (addressId) REFERENCES SRV.Addresses,
    CONSTRAINT [FK_PersonAddresses_PersonAddressTypes_personAddressTypeId] FOREIGN KEY (personAddressTypeId) REFERENCES Lookup.PersonAddressTypes,
	--constraint FK_PersonAddresses_PersonFullAddresses_personFullAddressId foreign key (personAddressId) references SRV.PersonFullAddress,
	
	CONSTRAINT [FK_PersonAddresses_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_PersonAddresses_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_PersonAddresses_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers
	
)

GO

CREATE CLUSTERED INDEX [IX_PersonAddresses_PersonId] ON [SRV].[PersonAddresses](personId)

GO
CREATE INDEX IX_PersonAddresses on SRV.PersonAddresses(isEligible,addressId) INCLUDE(apNumber,apSuffix,personId,personAddressTypeId);
GO




CREATE NONCLUSTERED INDEX [IX_PersonAddresses_addressId]
ON [SRV].[PersonAddresses] ([addressId])
INCLUDE ([personAddressId],[apNumber],[apSuffix],[isEligible],[personId],[personAddressTypeId])

GO

CREATE NONCLUSTERED INDEX [IX_PersonAddress_IsEligible]
ON [SRV].[PersonAddresses] ([isEligible])
INCLUDE ([personId],[addressId],[deleted])

GO

CREATE NONCLUSTERED INDEX [IX_PersonAddresses_IsEligible_w_includes]
ON [SRV].[PersonAddresses] ([isEligible])
INCLUDE ([apNumber],[apSuffix],[personId],[addressId],[personAddressTypeId],[deleted])

go

CREATE NONCLUSTERED INDEX [IX_PersonAddresses_isEligible_deleted]
ON [SRV].[PersonAddresses] ([isEligible],[deleted])
INCLUDE ([personId],[addressId])
