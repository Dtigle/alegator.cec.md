CREATE TABLE [dbo].[ReportParams] (
    [ReportParamId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [Code]          NVARCHAR (100) NOT NULL,
    [Description]   NVARCHAR (255) NULL,
    CONSTRAINT [PK_ReportParams] PRIMARY KEY CLUSTERED ([ReportParamId] ASC),
    CONSTRAINT [UK_ReportParams_code] UNIQUE NONCLUSTERED ([Code] ASC)
);

