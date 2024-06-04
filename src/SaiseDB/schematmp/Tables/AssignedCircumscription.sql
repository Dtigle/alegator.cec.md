CREATE TABLE [schematmp].[AssignedCircumscription] (
    [AssignedCircumscriptionId] BIGINT         NOT NULL,
    [ElectionRoundId]           BIGINT         NOT NULL,
    [CircumscriptionId]         BIGINT         NOT NULL,
    [RegionId]                  BIGINT         NOT NULL,
    [Number]                    NVARCHAR (32)  NOT NULL,
    [NameRo]                    NVARCHAR (255) NOT NULL,
    [isFromUtan]                BIT            CONSTRAINT [DF_AssignedCircumscription_isFromUtan_1] DEFAULT ((0)) NOT NULL,
    [EditUserId]                BIGINT         NOT NULL,
    [EditDate]                  DATETIME       NOT NULL,
    [Version]                   INT            NOT NULL,
    CONSTRAINT [PK_AssignedCircumscription] PRIMARY KEY CLUSTERED ([AssignedCircumscriptionId] ASC) WITH (FILLFACTOR = 80)
);

