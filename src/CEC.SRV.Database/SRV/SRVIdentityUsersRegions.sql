CREATE TABLE [SRV].[SRVIdentityUsersRegions] (
	identityUserId NVARCHAR(255) NOT NULL,
    regionId BIGINT NOT NULL,

    CONSTRAINT [PK_SRVIdentityUsersRegions] primary key (identityUserId, regionId),
    CONSTRAINT [FK_SRVIdentityUsersRegions_Regions] FOREIGN KEY (regionId) REFERENCES Lookup.Regions,
	CONSTRAINT [FK_SRVIdentityUsersRegions_SRVIdentityUsers] FOREIGN KEY (identityUserId) REFERENCES SRV.SRVIdentityUsers
	
)

GO

CREATE INDEX [IX_SRVIdentityUsersRegions_identityUserId] ON [SRV].[SRVIdentityUsersRegions] ([identityUserId]) include ([regionId])
