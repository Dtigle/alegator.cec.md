﻿CREATE TABLE [SRV].[AdditionalUserInfos] (
	additionalUserInfoId BIGINT IDENTITY NOT NULL,
	identityUserId NVARCHAR(255) null,
	firstName NVARCHAR(255) null,
	lastName NVARCHAR(255) null,
	[dateOfBirth] DATETIME2 NULL, 
	email NVARCHAR(255) null,
	landlinePhone NVARCHAR(255) null,
	mobPhone NVARCHAR(255) null,
	workInfo NVARCHAR(255) null,
	created DATETIMEOFFSET null,
	modified DATETIMEOFFSET null,
	deleted DATETIMEOFFSET null,
	createdById NVARCHAR(255) null,
	modifiedById NVARCHAR(255) null,
	deletedById NVARCHAR(255) null,
    [genderId] BIGINT NOT NULL, 
    CONSTRAINT [PK_AdditionalUserInfos] PRIMARY KEY (additionalUserInfoId),
	CONSTRAINT [FK_AdditionalUserInfos_IdentityUsers_identityUserId] FOREIGN KEY (identityUserId) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_AdditionalUserInfos_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_AdditionalUserInfos_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_AdditionalUserInfos_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers,
	constraint [FK_AdditionalUserInfos_Genders_genderId] foreign key (genderId) references [Lookup].[Genders]
)