CREATE TABLE [dbo].[AssignedVoter] (
    [AssignedVoterId]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [RegionId]                   BIGINT         CONSTRAINT [DF_AssignedVoter_RegionId] DEFAULT ((-1)) NOT NULL,
    [RequestingPollingStationId] BIGINT         CONSTRAINT [DF_AssignedVoter_RequestingPollingStationId] DEFAULT ((-1)) NOT NULL,
    [PollingStationId]           BIGINT         CONSTRAINT [DF_AssignedVoter_PollingStationId] DEFAULT ((-1)) NOT NULL,
    [VoterId]                    BIGINT         CONSTRAINT [DF_AssignedVoter_VoterId] DEFAULT ((-1)) NOT NULL,
    [Category]                   BIGINT         CONSTRAINT [DF_AssignedVoter_Type] DEFAULT ((1)) NOT NULL,
    [Status]                     BIGINT         CONSTRAINT [DF_AssignedVoter_Status] DEFAULT ((0)) NOT NULL,
    [Comment]                    NVARCHAR (MAX) NULL,
    [ElectionListNr]             BIGINT         NULL,
    [EditUserId]                 BIGINT         CONSTRAINT [DF_AssignedVoter_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]                   DATETIME       CONSTRAINT [DF_AssignedVoter_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                    INT            CONSTRAINT [DF_AssignedVoter_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_AssignedVoter] PRIMARY KEY CLUSTERED ([AssignedVoterId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_AssignedVoter_PollingStation] FOREIGN KEY ([PollingStationId]) REFERENCES [dbo].[PollingStation] ([PollingStationId]),
    CONSTRAINT [FK_AssignedVoter_PollingStation1] FOREIGN KEY ([RequestingPollingStationId]) REFERENCES [dbo].[PollingStation] ([PollingStationId]),
    CONSTRAINT [FK_AssignedVoter_Region] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([RegionId]),
    CONSTRAINT [FK_AssignedVoter_Voter] FOREIGN KEY ([VoterId]) REFERENCES [dbo].[Voter] ([VoterId])
);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoter_VoterId]
    ON [dbo].[AssignedVoter]([VoterId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterPollingStation]
    ON [dbo].[AssignedVoter]([PollingStationId] ASC, [Status] ASC);


GO
CREATE TRIGGER [dbo].[tr_AssignedVoter_Modified]
   ON [dbo].[AssignedVoter]
   AFTER UPDATE
AS BEGIN
    SET NOCOUNT ON;   
	INSERT INTO AssignedVoterStatistics
        ([AssignedVoterId],[AssignedVoterStatus],[Gender],[AgeCategoryId],[PollingStationId],[RegionId],[ParentRegionId])
    SELECT
        I.AssignedVoterId, 
		I.Status,
		V.Gender,
		(SELECT TOP 1 [AgeCategoryId] FROM [dbo].[AgeCategories] WITH(NOLOCK) WHERE ((0 + Convert(Char(8),(SELECT TOP 1 ElectionDayDate FROM [dbo].[ElectionDay] WITH(NOLOCK)),112) - Convert(Char(8),V.[DateOfBirth],112))/10000) between [From] AND ISNULL([To],150)),
		I.PollingStationId,
		(SELECT TOP 1 ps.RegionId FROM [dbo].[PollingStation] as ps WITH(NOLOCK) WHERE ps.PollingStationId = I.PollingStationId),
		[dbo].[fn_GetParentRegion]((SELECT TOP 1 ps.RegionId FROM [dbo].[PollingStation] as ps WITH(NOLOCK) WHERE ps.PollingStationId = I.PollingStationId))
    FROM 
	Inserted I WITH (NOLOCK)
	INNER JOIN  [dbo].[Voter] as V WITH(NOLOCK) ON I.VoterId = V.VoterId
	WHERE
	I.Status >=5000 and I.Status <=5004
END
