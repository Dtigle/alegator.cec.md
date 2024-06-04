CREATE TABLE [SRV].[Elections]
(
	electionId BIGINT IDENTITY NOT NULL,
    electionTypeId BIGINT not null,
    electionDate DATETIME2 null,
	saiseId BIGINT null,
	acceptAbroadDeclaration bit not null default 0,
    comments NVARCHAR(255) null,
    created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null
	CONSTRAINT [PK_Elections] primary key (electionId),
	CONSTRAINT [FK_Elections_ElectionTypes_electionTypeId] foreign key (electionTypeId) references [Lookup].[ElectionTypes],
	CONSTRAINT [FK_Elections_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Elections_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_Elections_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers
)
