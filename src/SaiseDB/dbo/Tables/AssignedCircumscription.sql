CREATE TABLE [dbo].[AssignedCircumscription] (
    [AssignedCircumscriptionId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [ElectionRoundId]           BIGINT         NOT NULL,
    [CircumscriptionId]         BIGINT         NOT NULL,
    [RegionId]                  BIGINT         NOT NULL,
    [Number]                    NVARCHAR (32)  NOT NULL,
    [NameRo]                    NVARCHAR (255) NOT NULL,
    [isFromUtan]                BIT            CONSTRAINT [DF_AssignedCircumscription_isFromUtan] DEFAULT ((0)) NOT NULL,
    [EditUserId]                BIGINT         CONSTRAINT [DF_AssignedCircumscription_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]                  DATETIME       CONSTRAINT [DF_AssignedCircumscription_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                   INT            CONSTRAINT [DF_AssignedCircumscription_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_AssignedCircumscription] PRIMARY KEY CLUSTERED ([AssignedCircumscriptionId] ASC),
    CONSTRAINT [FK_AssignedCircumscription_ElectionRound_ElectionRoundId] FOREIGN KEY ([ElectionRoundId]) REFERENCES [dbo].[ElectionRound] ([ElectionRoundId]),
    CONSTRAINT [FK_AssignedCircumscription_Region] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([RegionId])
);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedCircumscription_ElectionRound]
    ON [dbo].[AssignedCircumscription]([ElectionRoundId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AssignedCircumscription_Region]
    ON [dbo].[AssignedCircumscription]([RegionId] ASC);

