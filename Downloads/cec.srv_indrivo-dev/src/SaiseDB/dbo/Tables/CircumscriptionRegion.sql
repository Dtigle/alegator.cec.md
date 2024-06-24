CREATE TABLE [dbo].[CircumscriptionRegion] (
    [CircumscriptionRegionId]   BIGINT IDENTITY (1, 1) NOT NULL,
    [AssignedCircumscriptionId] BIGINT NOT NULL,
    [ElectionRoundId]           BIGINT NOT NULL,
    [RegionId]                  BIGINT NOT NULL,
    CONSTRAINT [PK_CircumscriptionRegion] PRIMARY KEY CLUSTERED ([CircumscriptionRegionId] ASC, [AssignedCircumscriptionId] ASC),
    CONSTRAINT [FK_CircumscriptionRegion_AssignedCircumscription] FOREIGN KEY ([AssignedCircumscriptionId]) REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId]),
    CONSTRAINT [FK_CircumscriptionRegion_ElectionRoundId] FOREIGN KEY ([ElectionRoundId]) REFERENCES [dbo].[ElectionRound] ([ElectionRoundId]),
    CONSTRAINT [FK_CircumscriptionRegion_Region] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([RegionId])
);

