CREATE TABLE [dbo].[AssignedVoterStatistics] (
    [AssignedVoterStatisticId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [AssignedVoterId]          BIGINT   NOT NULL,
    [AssignedVoterStatus]      INT      NOT NULL,
    [Gender]                   INT      NOT NULL,
    [AgeCategoryId]            INT      NOT NULL,
    [PollingStationId]         BIGINT   NOT NULL,
    [RegionId]                 BIGINT   NOT NULL,
    [ParentRegionId]           BIGINT   NOT NULL,
    [CreationDate]             DATETIME CONSTRAINT [DF_AssignedVoterStatistics_CreationDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_AssignedVoterStatistics] PRIMARY KEY CLUSTERED ([AssignedVoterStatisticId] ASC),
    CONSTRAINT [FK_AssignedVoterStatistics_AgeCategories] FOREIGN KEY ([AgeCategoryId]) REFERENCES [dbo].[AgeCategories] ([AgeCategoryId]),
    CONSTRAINT [FK_AssignedVoterStatistics_AssignedVoter] FOREIGN KEY ([AssignedVoterId]) REFERENCES [dbo].[AssignedVoter] ([AssignedVoterId]),
    CONSTRAINT [FK_AssignedVoterStatistics_PollingStation] FOREIGN KEY ([PollingStationId]) REFERENCES [dbo].[PollingStation] ([PollingStationId]),
    CONSTRAINT [FK_AssignedVoterStatistics_Region] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([RegionId]),
    CONSTRAINT [FK_AssignedVoterStatistics_RegionParent] FOREIGN KEY ([ParentRegionId]) REFERENCES [dbo].[Region] ([RegionId])
);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_AgeCategory]
    ON [dbo].[AssignedVoterStatistics]([AgeCategoryId] ASC, [RegionId] ASC, [ParentRegionId] ASC, [PollingStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_CreationDate]
    ON [dbo].[AssignedVoterStatistics]([CreationDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_Gender]
    ON [dbo].[AssignedVoterStatistics]([Gender] ASC, [RegionId] ASC, [ParentRegionId] ASC, [PollingStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_ParentRegionId]
    ON [dbo].[AssignedVoterStatistics]([ParentRegionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_PollingStation]
    ON [dbo].[AssignedVoterStatistics]([PollingStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_RegionId]
    ON [dbo].[AssignedVoterStatistics]([RegionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatisticsPSAgeCategory]
    ON [dbo].[AssignedVoterStatistics]([AgeCategoryId] ASC, [PollingStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatisticsPSGender]
    ON [dbo].[AssignedVoterStatistics]([Gender] ASC, [PollingStationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatisticsRegAgeCategory]
    ON [dbo].[AssignedVoterStatistics]([AgeCategoryId] ASC, [RegionId] ASC, [ParentRegionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatisticsRegGender]
    ON [dbo].[AssignedVoterStatistics]([Gender] ASC, [RegionId] ASC, [ParentRegionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_Status]
    ON [dbo].[AssignedVoterStatistics]([AssignedVoterStatus] ASC, [PollingStationId] ASC, [RegionId] ASC, [ParentRegionId] ASC);

