CREATE TABLE [SRV].[StayStatements]
(
	stayStatementId BIGINT IDENTITY NOT NULL,
	created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    personId BIGINT not null,
    baseAddressId BIGINT not null,
    declaredStayAddressId BIGINT not null,
    electionInstanceId BIGINT not null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null,
	CONSTRAINT [PK_StayStatements] PRIMARY KEY (stayStatementId),
	CONSTRAINT [FK_StayStatements_People_personId] FOREIGN KEY (personId) references SRV.People,
	constraint [FK_StayStatements_PersonAddresses_baseAddressId] foreign key (baseAddressId) references SRV.PersonAddresses,
	constraint [FK_StayStatements_PersonAddresses_declaredStayAddressId] foreign key (declaredStayAddressId) references SRV.PersonAddresses,
	constraint [FK_StayStatements_Elections_electionInstanceId] foreign key (electionInstanceId) references SRV.Elections,
	CONSTRAINT [FK_StayStatements_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_StayStatements_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_StayStatements_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers
)
