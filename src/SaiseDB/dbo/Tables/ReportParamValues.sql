CREATE TABLE [dbo].[ReportParamValues] (
    [ReportParamValueId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [ReportParamId]      BIGINT         NOT NULL,
    [ElectionTypeId]     BIGINT         NOT NULL,
    [Value]              NVARCHAR (512) NULL,
    CONSTRAINT [PK_ReportParamValues] PRIMARY KEY CLUSTERED ([ReportParamValueId] ASC),
    CONSTRAINT [FK_ReportParamValues_ElectionType] FOREIGN KEY ([ElectionTypeId]) REFERENCES [dbo].[ElectionType] ([ElectionTypeId]),
    CONSTRAINT [FK_ReportParamValues_ReportParams] FOREIGN KEY ([ReportParamId]) REFERENCES [dbo].[ReportParams] ([ReportParamId])
);

