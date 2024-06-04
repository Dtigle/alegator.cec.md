SET IDENTITY_INSERT SRV.[ConfigurationSettings] ON

DECLARE @SRV_ConfigurationSettings TABLE
(
	configurationSettingId BIGINT NOT NULL,
    [name] NVARCHAR(255) not null,
    [value] NVARCHAR(255) null,
	[applicationName] nvarchar(255) null,
	created DATETIMEOFFSET,
	modified DATETIMEOFFSET,
	deleted DATETIMEOFFSET,
	createdById NVARCHAR(255),
	modifiedById NVARCHAR(255),
	deletedById NVARCHAR(255)
)

INSERT INTO @SRV_ConfigurationSettings (configurationSettingId, [name], [value], [applicationName], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	( 1,  N'MvcReportViewer.Username',								null,																null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 2,  N'MvcReportViewer.Password',								null,																null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 3,  N'SMPT.username',											N'smpt.account@server',												null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 4,  N'SMPT.password',											N'accountPwd',														null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 5,  N'SMPT.host',												N'smpt.host',														null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 6,  N'SMPT.port',												N'110',																null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 7,  N'SaiseExporterJob_IgnoreMissingSAISEIdinSRVRegion',		N'true',															null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 8,  N'SaiseExporterJob_IgnorePeopleWithoutDoc',				N'true',															null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 9,  N'RspUser',												N'cec',																null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(10,  N'RspPass',												N'cec',																null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(11,  N'SSRS_ReportExecutionService',							N'http://localhost:80/ReportServer/ReportExecution2005.asmx',		null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(13,  N'SSRS_UserName',											null,																null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(14,  N'SSRS_Password',											null,																null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(15,  N'SSRS_VoterReport',										N'/SRV/VotersList_A3_2014',											null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(16,  N'SSRS_AbroadVoterReport',								N'/SRV/VotersList_Abroad_A3_2014',									null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(17,  N'SSRS_ResultsReport',									N'/Parlamentare2014/ParliamentElectionStats_ExtWeb',				null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(18,  N'SSRS_TurnoutReportsPath',								N'/Parlamentare2014/ExternalWeb/',									null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(19,  N'ListPrintingJob_ExportPath',							N'd:\temp\',														null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(20,  N'IgnorableDistrictsNumbers',								null,																null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(21,  N'ResultsReportsPrintOutJob_ReportPageHeight',			N'44cm',															null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(22,  N'TurnoutFixationJob_TurnoutControlTimes',				N'Open;09:30;12:30;15:30;18:30;21:30',								null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(23,  N'TurnoutReportsPrintOutJob_ReportPageHeight',			N'50cm',															null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(24,  N'RspModificationProcessorJob_IgnoreNewEntries',			N'true',															null,			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(25,  N'UpdateClassifiersEnabled',								N'false',															null,			SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE SRV.[ConfigurationSettings] AS target
USING
	(
		SELECT configurationSettingId, [name], [value], [applicationName], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @SRV_ConfigurationSettings
	) AS source
ON source.configurationSettingId = target.configurationSettingId
WHEN MATCHED AND (source.[name] <> target.[name]
OR source.[value] <> target.[value]
OR source.[applicationName] <> target.[applicationName]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[name] = source.[name],
			target.[value] = source.[value],
			target.[applicationName] = source.[applicationName],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT (configurationSettingId, [name], [value], [applicationName], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.configurationSettingId, source.[name], source.[value], source.[applicationName], 
				source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);

SET IDENTITY_INSERT SRV.[ConfigurationSettings] OFF