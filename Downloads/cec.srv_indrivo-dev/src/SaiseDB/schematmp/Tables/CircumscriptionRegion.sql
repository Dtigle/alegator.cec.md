CREATE TABLE [schematmp].[CircumscriptionRegion] (
    [CircumscriptionRegionId]   BIGINT NOT NULL,
    [AssignedCircumscriptionId] BIGINT NOT NULL,
    [ElectionRoundId]           BIGINT NOT NULL,
    [RegionId]                  BIGINT NOT NULL,
    CONSTRAINT [PK_CircumscriptionRegion] PRIMARY KEY CLUSTERED ([CircumscriptionRegionId] ASC, [AssignedCircumscriptionId] ASC) WITH (FILLFACTOR = 80)
);

