﻿CREATE TABLE [schematmp].[AssignedPollingStation] (
    [AssignedPollingStationId]  BIGINT    NOT NULL,
    [ElectionRoundId]           BIGINT    NOT NULL,
    [AssignedCircumscriptionId] BIGINT    NOT NULL,
    [PollingStationId]          BIGINT    NOT NULL,
    [Type]                      INT       NOT NULL,
    [Status]                    BIGINT    NOT NULL,
    [IsOpen]                    BIT       NOT NULL,
    [OpeningVoters]             BIGINT    NOT NULL,
    [EstimatedNumberOfVoters]   INT       NOT NULL,
    [NumberOfRoBallotPapers]    INT       NOT NULL,
    [NumberOfRuBallotPapers]    INT       NOT NULL,
    [ImplementsEVR]             BIT       NOT NULL,
    [EditUserId]                BIGINT    NOT NULL,
    [EditDate]                  DATETIME  NOT NULL,
    [Version]                   INT       NOT NULL,
    [isOpeningEnabled]          BIT       NOT NULL,
    [isTurnoutEnabled]          BIT       NOT NULL,
    [isElectionResultEnabled]   BIT       NOT NULL,
    [numberPerElection]         CHAR (10) NULL,
    [RegionId]                  BIGINT    NULL,
    [ParentRegionId]            BIGINT    NULL,
    [CirculationRo]             INT       NULL,
    [CirculationRu]             INT       NULL,
    CONSTRAINT [PK_AssignedPollingStation] PRIMARY KEY NONCLUSTERED ([AssignedPollingStationId] ASC) WITH (FILLFACTOR = 80)
);
