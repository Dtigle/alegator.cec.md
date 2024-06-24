CREATE TABLE [schematmp].[AuditEventTypes] (
    [auditEventTypeId] BIGINT         NOT NULL,
    [code]             NVARCHAR (50)  NOT NULL,
    [auditStrategy]    SMALLINT       NOT NULL,
    [name]             NVARCHAR (50)  NOT NULL,
    [description]      NVARCHAR (500) NULL,
    [EditUserId]       BIGINT         NOT NULL,
    [EditDate]         DATETIME       NOT NULL,
    [Version]          INT            NOT NULL,
    CONSTRAINT [PK_AuditEventTypes] PRIMARY KEY CLUSTERED ([auditEventTypeId] ASC) WITH (FILLFACTOR = 80)
);

