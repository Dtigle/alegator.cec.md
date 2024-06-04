CREATE TABLE [dbo].[PollingStation] (
    [PollingStationId]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [Type]                    INT            CONSTRAINT [DF_PollingStation_Type] DEFAULT ((1)) NOT NULL,
    [Number]                  INT            CONSTRAINT [DF_PollingStation_Number] DEFAULT ((0)) NOT NULL,
    [SubNumber]               NVARCHAR (50)  NULL,
    [OldName]                 NVARCHAR (MAX) NULL,
    [NameRo]                  NVARCHAR (MAX) NULL,
    [NameRu]                  NVARCHAR (MAX) NULL,
    [Address]                 NVARCHAR (500) NULL,
    [RegionId]                BIGINT         CONSTRAINT [DF_PollingStation_RegionId] DEFAULT ((0)) NOT NULL,
    [StreetId]                BIGINT         NULL,
    [StreetNumber]            INT            NULL,
    [StreetSubNumber]         NVARCHAR (50)  NULL,
    [EditUserId]              BIGINT         CONSTRAINT [DF_PollingStation_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]                DATETIME       CONSTRAINT [DF_PollingStation_EditDate_2] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                 INT            CONSTRAINT [DF_PollingStation_Version] DEFAULT ((1)) NOT NULL,
    [LocationLatitude]        FLOAT (53)     NULL,
    [LocationLongitude]       FLOAT (53)     NULL,
    [ExcludeInLocalElections] BIT            CONSTRAINT [DF_PollingStation_ExcludeInLocalElection] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PollingUnit] PRIMARY KEY CLUSTERED ([PollingStationId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_PollingStation_Region] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([RegionId])
);


GO
CREATE NONCLUSTERED INDEX [IX_PollingStationRegion]
    ON [dbo].[PollingStation]([RegionId] ASC);

