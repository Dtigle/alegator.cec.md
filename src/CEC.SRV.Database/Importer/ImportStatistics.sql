CREATE TABLE [Importer].[ImportStatistics] (
        importStatisticId BIGINT IDENTITY NOT NULL,
       [new] BIGINT null,
	   [date] DATETIME not null,
       conflicted BIGINT null,
       updated BIGINT null,
	   error BIGINT null,
       total BIGINT null,
	   region BIGINT null,
       changedStatus BIGINT null,
       residenceChnaged BIGINT null,

       PRIMARY KEY (importStatisticId)
	   
    )