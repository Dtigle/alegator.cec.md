CREATE TABLE [Audit].[IdentityUsers_AUD] (
    identityUserId NVARCHAR(255) not null,
    REV INT not null,
    REVTYPE TINYINT not null,
    userName NVARCHAR(255) null,
    passwordHash NVARCHAR(255) null,
    securityStamp NVARCHAR(255) null,
	CONSTRAINT [PK_IdentityUsers_AUD] PRIMARY KEY (identityUserId, REV),
	CONSTRAINT [FK6200875FAAE62361] FOREIGN KEY (REV) REFERENCES [Audit].[REVINFO]
)