CREATE TABLE [dbo].[Election] (
    [ElectionId]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Type]                    BIGINT         CONSTRAINT [DF_Election_Type] DEFAULT ((0)) NOT NULL,
    [Status]                  INT            CONSTRAINT [DF_Election_Status] DEFAULT ((0)) NOT NULL,
    [DateOfElection]          DATETIME       CONSTRAINT [DF_Election_DateOfElection] DEFAULT (sysdatetime()) NOT NULL,
    [Comments]                NVARCHAR (MAX) CONSTRAINT [DF_Election_Comments] DEFAULT ('TBD') NOT NULL,
    [EditUserId]              BIGINT         CONSTRAINT [DF_Election_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]                DATETIME       CONSTRAINT [DF_Election_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                 INT            CONSTRAINT [DF_Election_Version] DEFAULT ((1)) NOT NULL,
    [ReportsPath]             VARCHAR (MAX)  CONSTRAINT [DF_Election_ReportsPath] DEFAULT ('') NOT NULL,
    [BuletinDateOfElectionRo] NVARCHAR (20)  NULL,
    [BuletinDateOfElectionRu] NVARCHAR (20)  NULL,
    CONSTRAINT [PK_Election] PRIMARY KEY CLUSTERED ([ElectionId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Election_ElectionType] FOREIGN KEY ([Type]) REFERENCES [dbo].[ElectionType] ([ElectionTypeId])
);

