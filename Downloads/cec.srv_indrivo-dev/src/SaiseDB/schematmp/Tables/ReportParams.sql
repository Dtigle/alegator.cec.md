CREATE TABLE [schematmp].[ReportParams] (
    [ReportParamId] BIGINT         NOT NULL,
    [Code]          NVARCHAR (100) NOT NULL,
    [Description]   NVARCHAR (255) NULL,
    CONSTRAINT [PK_ReportParam] PRIMARY KEY CLUSTERED ([ReportParamId] ASC) WITH (FILLFACTOR = 80)
);

