CREATE TABLE [dbo].[ElectionType] (
    [ElectionTypeId]          BIGINT         NOT NULL,
    [Code]                    INT            NULL,
    [TypeName]                NVARCHAR (50)  NOT NULL,
    [Description]             NVARCHAR (100) NULL,
    [ElectionArea]            TINYINT        NULL,
    [ElectionCompetitorType]  TINYINT        NULL,
    [ElectionRoundsNo]        TINYINT        NULL,
    [AcceptResidenceDoc]      BIT            NULL,
    [AcceptVotingCert]        BIT            NULL,
    [AcceptAbroadDeclaration] BIT            NULL,
    CONSTRAINT [PK_ElectionType] PRIMARY KEY CLUSTERED ([ElectionTypeId] ASC) WITH (FILLFACTOR = 80)
);

