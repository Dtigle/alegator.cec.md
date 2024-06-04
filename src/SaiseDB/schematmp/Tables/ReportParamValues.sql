CREATE TABLE [schematmp].[ReportParamValues] (
    [ReportParamValueId] BIGINT         NOT NULL,
    [ReportParamId]      BIGINT         NOT NULL,
    [ElectionTypeId]     BIGINT         NOT NULL,
    [Value]              NVARCHAR (512) NULL,
    CONSTRAINT [PK_ReportParamValue] PRIMARY KEY CLUSTERED ([ReportParamValueId] ASC) WITH (FILLFACTOR = 80)
);

