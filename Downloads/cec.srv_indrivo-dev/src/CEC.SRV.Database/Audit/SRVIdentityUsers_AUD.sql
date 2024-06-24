CREATE TABLE [Audit].[SRVIdentityUsers_AUD] (
    identityUserId NVARCHAR(255) not null,
    REV INT not null,
    comments NVARCHAR(255) null,
	[isBuiltIn] BIT NULL, 
	isBlocked BIT NULL,
	blockedDate DATETIMEOFFSET NULL,
	loginAttempts INT NULL,
    [lastLogin] DATETIMEOFFSET NULL, 
    [lastLogout] DATETIMEOFFSET NULL, 
	created DATETIMEOFFSET null,
	createdById NVARCHAR(255) null,
    CONSTRAINT [PK_SRVIdentityUsers_AUD] PRIMARY KEY (identityUserId, REV),
	CONSTRAINT [FKA1DE8D2B524728DA] FOREIGN KEY (REV) REFERENCES [Audit].[REVINFO]
)
