CREATE TABLE [Print].[ExportPollingStations] (
	   exportPollingStationId BIGINT IDENTITY NOT NULL,
	   pollingStationId BIGINT NOT NULL,
       printSessionId BIGINT NOT null,
       startDate DATETIMEOFFSET null,
       endDate DATETIMEOFFSET null,
       [status] INT NOT null,
       statusMessage NVARCHAR(255) null,
       created DATETIMEOFFSET null,
		modified DATETIMEOFFSET null,
		deleted DATETIMEOFFSET null,
		createdById NVARCHAR(255) null,
		modifiedById NVARCHAR(255) null,
		deletedById NVARCHAR(255) null
    CONSTRAINT [PK_ExportPollingStations] primary key (exportPollingStationId),
    CONSTRAINT [FK_ExportPollingStations_PollingStations_pollingStationId] FOREIGN KEY (pollingStationId) REFERENCES SRV.PollingStations,
    constraint FK_ExportPollingStations_PrintSessions_ExportPollingStations foreign key (printSessionId) references [Print].PrintSessions,
	
	constraint [FK_ExportPollingStations_SRVIdentityUsers_createdById] foreign key (createdById) references SRV.SRVIdentityUsers,
	constraint [FK_ExportPollingStations_SRVIdentityUsers_modifiedById] foreign key (modifiedById) references SRV.SRVIdentityUsers,
	constraint [FK_ExportPollingStations_SRVIdentityUsers_deletedById] foreign key (deletedById) references SRV.SRVIdentityUsers
)
