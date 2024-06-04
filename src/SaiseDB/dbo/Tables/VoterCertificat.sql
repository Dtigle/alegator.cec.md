CREATE TABLE [dbo].[VoterCertificat] (
    [VoterCertificatId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [AssignedVoterId]   BIGINT         NOT NULL,
    [ReleaseDate]       DATETIME       NOT NULL,
    [CertificatNr]      NVARCHAR (255) NOT NULL,
    [PollingStationId]  BIGINT         NOT NULL,
    [EditUserID]        BIGINT         NOT NULL,
    [EditDate]          DATETIME       NOT NULL,
    [Version]           INT            NOT NULL,
    [Deleted]           DATETIME       NULL,
    CONSTRAINT [PK_VoterCertificat] PRIMARY KEY CLUSTERED ([VoterCertificatId] ASC),
    CONSTRAINT [FK_VoterCertificat_AssignedVoter] FOREIGN KEY ([AssignedVoterId]) REFERENCES [dbo].[AssignedVoter] ([AssignedVoterId]),
    CONSTRAINT [FK_VoterCertificat_PollingStation] FOREIGN KEY ([PollingStationId]) REFERENCES [dbo].[PollingStation] ([PollingStationId]),
    CONSTRAINT [FK_VoterCertificat_VoterCertificat] FOREIGN KEY ([VoterCertificatId]) REFERENCES [dbo].[VoterCertificat] ([VoterCertificatId])
);


GO
CREATE NONCLUSTERED INDEX [IX_VoterCertificat_AssignedVoterId]
    ON [dbo].[VoterCertificat]([AssignedVoterId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_VoterCertificat_PollingStationId]
    ON [dbo].[VoterCertificat]([PollingStationId] ASC);

