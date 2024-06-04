CREATE TABLE [Access].[IdentityUserClaims] 
(
       [identityUserClaimId] BIGINT IDENTITY NOT NULL,
       [claimType] NVARCHAR(255) NOT NULL,
       [claimValue] NVARCHAR(255) NOT NULL,
       [userId] NVARCHAR(255) NULL,
       CONSTRAINT [PK_IdentityUserClaims] PRIMARY KEY ([identityUserClaimId]),
	   CONSTRAINT [FK_IdentityUserClaims_IdentityUsers_Claims] FOREIGN KEY ([userId]) references [Access].[IdentityUsers]
)
