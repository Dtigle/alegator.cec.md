CREATE TABLE [Importer].[AlegatorDatas]
(
	[alegatorDataId] BIGINT IDENTITY NOT NULL,
	alegatorId BIGINT NULL,
    alegatorPollingStationId BIGINT NULL,
    docSeria NVARCHAR(255) NULL,
    docNumber NVARCHAR(255) NULL,
    streetName NVARCHAR(255) NULL,
    houseNr NVARCHAR(255) NULL,
    apartment NVARCHAR(255) NULL,
    idnp NVARCHAR(255) NULL,
    firstName NVARCHAR(255) NULL,
    middleName NVARCHAR(255) NULL,
    surname NVARCHAR(255) NULL,
    dateOfBirth DATETIME NULL,
    gender NVARCHAR(255) NULL,
    source NVARCHAR(255) NULL,
	statusMessage NVARCHAR(MAX) NULL,
    status NVARCHAR(255) NULL,
    created DATETIMEOFFSET NULL,
    statusDate DATETIMEOFFSET NULL,
    comments NVARCHAR(255) NULL,
	personStatus INT,
		
	constraint [PK_AlegatorDatas] primary key clustered (alegatorDataId asc),
	
)
