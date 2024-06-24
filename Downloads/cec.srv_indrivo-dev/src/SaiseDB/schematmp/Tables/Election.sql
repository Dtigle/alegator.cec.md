CREATE TABLE [schematmp].[Election] (
    [ElectionId]              BIGINT         NOT NULL,
    [Type]                    BIGINT         NOT NULL,
    [Status]                  INT            NOT NULL,
    [DateOfElection]          DATETIME       NOT NULL,
    [Comments]                NVARCHAR (MAX) NOT NULL,
    [EditUserId]              BIGINT         NOT NULL,
    [EditDate]                DATETIME       NOT NULL,
    [Version]                 INT            NOT NULL,
    [ReportsPath]             VARCHAR (MAX)  NOT NULL,
    [BuletinDateOfElectionRo] NVARCHAR (20)  NULL,
    [BuletinDateOfElectionRu] NVARCHAR (20)  NULL,
    CONSTRAINT [PK_Election] PRIMARY KEY CLUSTERED ([ElectionId] ASC) WITH (FILLFACTOR = 80)
);

