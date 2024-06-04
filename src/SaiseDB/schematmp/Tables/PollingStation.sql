CREATE TABLE [schematmp].[PollingStation] (
    [PollingStationId]        BIGINT         NOT NULL,
    [Type]                    INT            NOT NULL,
    [Number]                  INT            NOT NULL,
    [SubNumber]               NVARCHAR (50)  NULL,
    [OldName]                 NVARCHAR (MAX) NULL,
    [NameRo]                  NVARCHAR (MAX) NULL,
    [NameRu]                  NVARCHAR (MAX) NULL,
    [Address]                 NVARCHAR (500) NULL,
    [RegionId]                BIGINT         NULL,
    [StreetId]                BIGINT         NULL,
    [StreetNumber]            INT            NULL,
    [StreetSubNumber]         NVARCHAR (50)  NULL,
    [EditUserId]              BIGINT         NOT NULL,
    [EditDate]                DATETIME       NOT NULL,
    [Version]                 INT            NOT NULL,
    [LocationLatitude]        FLOAT (53)     NULL,
    [LocationLongitude]       FLOAT (53)     NULL,
    [ExcludeInLocalElections] BIT            NOT NULL,
    CONSTRAINT [PK_PollingUnit] PRIMARY KEY CLUSTERED ([PollingStationId] ASC) WITH (FILLFACTOR = 80)
);

