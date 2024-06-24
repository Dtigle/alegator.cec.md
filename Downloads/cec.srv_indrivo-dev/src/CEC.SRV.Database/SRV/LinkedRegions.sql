create table SRV.LinkedRegions (
        linkedRegionId BIGINT IDENTITY NOT NULL,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
	   CONSTRAINT [PK_LinkedRegions] primary key (linkedRegionId),
	   constraint FK_LinkedRegions_IdentityUsers_createdById foreign key (createdById) references Access.IdentityUsers,
	   constraint FK_LinkedRegions_IdentityUsers_modifiedById foreign key (modifiedById) references Access.IdentityUsers,
	   constraint FK_LinkedRegions_IdentityUsers_deletedById foreign key (deletedById) references Access.IdentityUsers
    )
