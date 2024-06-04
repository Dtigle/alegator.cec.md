CREATE TABLE [dbo].[AuditEvents] (
    [auditEventId]     BIGINT          IDENTITY (1, 1) NOT NULL,
    [auditEventTypeId] BIGINT          NOT NULL,
    [level]            TINYINT         NOT NULL,
    [generatedAt]      DATETIME        NOT NULL,
    [message]          NVARCHAR (1000) NULL,
    [userId]           NVARCHAR (128)  NULL,
    [userMachineIp]    NVARCHAR (50)   NULL,
    [EditUserID]       BIGINT          NOT NULL,
    [EditDate]         DATETIME        NOT NULL,
    [Version]          INT             NOT NULL,
    CONSTRAINT [PK_AuditEvents] PRIMARY KEY CLUSTERED ([auditEventId] ASC),
    CONSTRAINT [FK_AuditEvents_AuditEventTypes] FOREIGN KEY ([auditEventTypeId]) REFERENCES [dbo].[AuditEventTypes] ([auditEventTypeId])
);

