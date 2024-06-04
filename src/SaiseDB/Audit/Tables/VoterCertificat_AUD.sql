CREATE TABLE [Audit].[VoterCertificat_AUD] (
    [VoterCertificatId] BIGINT         NOT NULL,
    [REV]               INT            NOT NULL,
    [REVTYPE]           TINYINT        NOT NULL,
    [AssignedVoterId]   BIGINT         NULL,
    [ReleaseDate]       DATETIME       NULL,
    [CertificatNr]      NVARCHAR (255) NULL,
    [PollingStationId]  BIGINT         NULL,
    [EditUserID]        BIGINT         NULL,
    [EditDate]          DATETIME       NULL,
    [Version]           INT            NULL,
    [Deleted]           DATETIME       NULL,
    CONSTRAINT [PK_VoterCertificat_AUD] PRIMARY KEY CLUSTERED ([VoterCertificatId] ASC, [REV] ASC),
    CONSTRAINT [FK6271F260AAE62361] FOREIGN KEY ([REV]) REFERENCES [Audit].[REVINFO] ([REV])
);

