﻿CREATE TABLE [schematmp].[SystemUser] (
    [SystemUserId]            BIGINT         NOT NULL,
    [UserName]                NVARCHAR (50)  NOT NULL,
    [Password]                NVARCHAR (MAX) NOT NULL,
    [Email]                   NVARCHAR (50)  NOT NULL,
    [Level]                   INT            NOT NULL,
    [Comments]                NVARCHAR (MAX) NULL,
    [Idnp]                    BIGINT         NOT NULL,
    [FirstName]               NVARCHAR (100) NOT NULL,
    [Surname]                 NVARCHAR (100) NOT NULL,
    [MiddleName]              NVARCHAR (100) NULL,
    [DateOfBirth]             DATETIME       NOT NULL,
    [Gender]                  INT            NOT NULL,
    [PasswordQuestion]        NVARCHAR (100) NULL,
    [PasswordAnswer]          NVARCHAR (100) NULL,
    [IsApproved]              BIT            NOT NULL,
    [IsOnLine]                BIT            NOT NULL,
    [IsLockedOut]             BIT            NOT NULL,
    [CreationDate]            DATETIME       NOT NULL,
    [LastActivityDate]        DATETIME       NOT NULL,
    [LastPasswordChangedDate] DATETIME       NOT NULL,
    [LastLockoutDate]         DATETIME       NOT NULL,
    [FailedAttemptStart]      DATETIME       NOT NULL,
    [FailedAnswerStart]       DATETIME       NOT NULL,
    [FailedAttemptCount]      INT            NOT NULL,
    [FailedAnswerCount]       INT            NOT NULL,
    [LastLoginDate]           DATETIME       NOT NULL,
    [LastUpdateDate]          DATETIME       NOT NULL,
    [Language]                NVARCHAR (100) NULL,
    [MobileNumber]            NVARCHAR (20)  NULL,
    [ContactName]             NVARCHAR (100) NULL,
    [ContactMobileNumber]     NVARCHAR (20)  NULL,
    [StreetAddress]           NVARCHAR (100) NULL,
    [ElectionId]              BIGINT         NULL,
    [RegionId]                BIGINT         NULL,
    [PollingStationId]        BIGINT         NULL,
    [CircumscriptionId]       BIGINT         NULL,
    [EditUserId]              BIGINT         NOT NULL,
    [EditDate]                DATETIME       NOT NULL,
    [Version]                 INT            NOT NULL,
    [IsDeleted]               BIT            NOT NULL,
    CONSTRAINT [PK_SystemUser_1] PRIMARY KEY CLUSTERED ([SystemUserId] ASC) WITH (FILLFACTOR = 80)
);

