CREATE TABLE [Print].[PrintSessions] (
	   printSessionId BIGINT IDENTITY NOT NULL,
	   electionId bigint NOT NULL,
       startDate DATETIMEOFFSET null,
       endDate DATETIMEOFFSET null,
       [status] INT NOT null,
	   created DATETIMEOFFSET null,
		modified DATETIMEOFFSET null,
		deleted DATETIMEOFFSET null,
		createdById NVARCHAR(255) null,
		modifiedById NVARCHAR(255) null,
		deletedById NVARCHAR(255) null
    CONSTRAINT [PK_PrintSessions] primary key (printSessionId),

	constraint [FK_PrintSessions_SRVIdentityUsers_createdById] foreign key (createdById) references SRV.SRVIdentityUsers,
	constraint [FK_PrintSessions_SRVIdentityUsers_modifiedById] foreign key (modifiedById) references SRV.SRVIdentityUsers,
	constraint [FK_PrintSessions_SRVIdentityUsers_deletedById] foreign key (deletedById) references SRV.SRVIdentityUsers,
	CONSTRAINT [FK_PrintSessions_Elections_electionId] FOREIGN KEY (electionId) REFERENCES SRV.Elections,
)
