﻿CREATE TABLE [schematmp].[ElectionCompetitorMember] (
    [ElectionCompetitorMemberId] BIGINT          NOT NULL,
    [AssignedCircumscriptionId]  BIGINT          NULL,
    [ElectionRoundId]            BIGINT          NULL,
    [LastNameRo]                 NVARCHAR (100)  NOT NULL,
    [LastNameRu]                 NVARCHAR (100)  NULL,
    [NameRo]                     NVARCHAR (100)  NOT NULL,
    [NameRu]                     NVARCHAR (100)  NULL,
    [PatronymicRo]               NVARCHAR (100)  NULL,
    [PatronymicRu]               NVARCHAR (100)  NULL,
    [DateOfBirth]                DATETIME        NOT NULL,
    [PlaceOfBirth]               NVARCHAR (100)  NOT NULL,
    [Gender]                     INT             NOT NULL,
    [Occupation]                 NVARCHAR (100)  NULL,
    [OccupationRu]               NVARCHAR (100)  NULL,
    [Designation]                NVARCHAR (100)  NULL,
    [DesignationRu]              NVARCHAR (100)  NULL,
    [Workplace]                  NVARCHAR (200)  NULL,
    [WorkplaceRu]                NVARCHAR (200)  NULL,
    [Idnp]                       BIGINT          NOT NULL,
    [ElectionCompetitorId]       BIGINT          NULL,
    [DateOfRegistration]         DATE            NULL,
    [CompetitorMemberOrder]      INT             NOT NULL,
    [ColorLogo]                  VARBINARY (MAX) NULL,
    [BlackWhiteLogo]             VARBINARY (MAX) NULL,
    [Picture]                    VARBINARY (MAX) NULL,
    [Status]                     INT             NOT NULL,
    [EditUserId]                 BIGINT          NOT NULL,
    [EditDate]                   DATETIME        NOT NULL,
    [Version]                    INT             NOT NULL,
    CONSTRAINT [PK_ElectionCompetitorMember] PRIMARY KEY NONCLUSTERED ([ElectionCompetitorMemberId] ASC) WITH (FILLFACTOR = 80)
);
