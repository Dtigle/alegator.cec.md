CREATE TABLE [SRV].[SRVIdentityUsers] (
	identityUserId NVARCHAR(255) not null,
	additionalInfoId BIGINT null,
    [isBuiltIn] BIT NOT NULL DEFAULT 0,
	[isBlocked] BIT NOT NULL DEFAULT 0,
	blockedDate DATETIMEOFFSET NULL,
	[loginAttempts] INT NOT NULL DEFAULT 0,
    comments NVARCHAR(255) null,
    [lastLogin] DATETIMEOFFSET NULL,
    [lastLogout] DATETIMEOFFSET NULL,
	created DATETIMEOFFSET null,
	createdById NVARCHAR(255) null, 
    CONSTRAINT [PK_SRVIdentityUsers] primary key (identityUserId),
    CONSTRAINT [FKF9C1AFC24D071132] FOREIGN KEY (identityUserId) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_SRVIdentityUsers_SRVIdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES SRV.SRVIdentityUsers,
	constraint FK_SRVIdentityUsers_AdditionalUserInfos_additionalInfoId foreign key (additionalInfoId) references SRV.AdditionalUserInfos
)
