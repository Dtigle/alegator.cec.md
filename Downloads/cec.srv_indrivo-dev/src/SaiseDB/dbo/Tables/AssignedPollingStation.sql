CREATE TABLE [dbo].[AssignedPollingStation] (
    [AssignedPollingStationId]  BIGINT    IDENTITY (1, 1) NOT NULL,
    [ElectionRoundId]           BIGINT    NOT NULL,
    [AssignedCircumscriptionId] BIGINT    NOT NULL,
    [PollingStationId]          BIGINT    NOT NULL,
    [Type]                      INT       CONSTRAINT [DF_AssignedPollingStation_Type] DEFAULT ((1)) NOT NULL,
    [Status]                    BIGINT    CONSTRAINT [DF_AssignedPollingStation_Status] DEFAULT ((0)) NOT NULL,
    [IsOpen]                    BIT       CONSTRAINT [DF_AssignedPollingStation_IsOpen] DEFAULT ((0)) NOT NULL,
    [OpeningVoters]             BIGINT    CONSTRAINT [DF_AssignedPollingStation_OpeningVoters] DEFAULT ((0)) NOT NULL,
    [EstimatedNumberOfVoters]   INT       CONSTRAINT [DF_AssignedPollingStation_EstimatedNumberOfVoters] DEFAULT ((0)) NOT NULL,
    [NumberOfRoBallotPapers]    INT       CONSTRAINT [DF_AssignedPollingStation_NumberOfRoBallotPapers] DEFAULT ((0)) NOT NULL,
    [NumberOfRuBallotPapers]    INT       CONSTRAINT [DF_AssignedPollingStation_NumberOfRuBallotPapers] DEFAULT ((0)) NOT NULL,
    [ImplementsEVR]             BIT       CONSTRAINT [DF_AssignedPollingStation_ImplementsEVR] DEFAULT ((0)) NOT NULL,
    [EditUserId]                BIGINT    CONSTRAINT [DF_AssignedPollingStation_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]                  DATETIME  CONSTRAINT [DF_AssignedPollingStation_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                   INT       CONSTRAINT [DF_AssignedPollingStation_Version] DEFAULT ((1)) NOT NULL,
    [isOpeningEnabled]          BIT       CONSTRAINT [DF__AssignedP__isOpe__7E37BEF6] DEFAULT ((0)) NOT NULL,
    [isTurnoutEnabled]          BIT       CONSTRAINT [DF__AssignedP__isTur__7F2BE32F] DEFAULT ((0)) NOT NULL,
    [isElectionResultEnabled]   BIT       CONSTRAINT [DF__AssignedP__isEle__00200768] DEFAULT ((0)) NOT NULL,
    [numberPerElection]         CHAR (10) NULL,
    [RegionId]                  BIGINT    NULL,
    [ParentRegionId]            BIGINT    NULL,
    [CirculationRo]             INT       NULL,
    [CirculationRu]             INT       NULL,
    CONSTRAINT [PK_AssignedPollingStation] PRIMARY KEY NONCLUSTERED ([AssignedPollingStationId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_AssignedPollingStation_AssignedCircumscription] FOREIGN KEY ([AssignedCircumscriptionId]) REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId]),
    CONSTRAINT [FK_AssignedPollingStation_ElectionRound] FOREIGN KEY ([ElectionRoundId]) REFERENCES [dbo].[ElectionRound] ([ElectionRoundId]),
    CONSTRAINT [FK_AssignedPollingStation_PollingStation] FOREIGN KEY ([PollingStationId]) REFERENCES [dbo].[PollingStation] ([PollingStationId]),
    CONSTRAINT [FK_AssignedPollingStation_Region] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([RegionId]),
    CONSTRAINT [FK_AssignedPollingStation_RegionParent] FOREIGN KEY ([ParentRegionId]) REFERENCES [dbo].[Region] ([RegionId])
);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationCircumscription]
    ON [dbo].[AssignedPollingStation]([AssignedCircumscriptionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationElectionRound]
    ON [dbo].[AssignedPollingStation]([ElectionRoundId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationParentRegion]
    ON [dbo].[AssignedPollingStation]([ParentRegionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationPollingStation]
    ON [dbo].[AssignedPollingStation]([PollingStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationRegion]
    ON [dbo].[AssignedPollingStation]([RegionId] ASC);

