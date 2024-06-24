CREATE TABLE Importer.[RspRegistrationDatas]
(
	rspRegistrationDataId BIGINT IDENTITY NOT NULL,
	dateOfExpiration datetime2 null,
	regTypeCode INT null,
	region NVARCHAR(255) null,
	locality NVARCHAR(255) null,
	administrativecode INT null,
	streetName NVARCHAR(255) null,
	streetcode INT null,
	houseNr INT null,
	houseSuffix NVARCHAR(255) null,
	apNr INT null,
	apSuffix NVARCHAR(255) null,
	isInConflict BIT,
	dateOfRegistration datetime2 not null DEFAULT '0001-01-01',
	rspModificationDataId BIGINT null,
	constraint [PK_RspRegistrationDatas] primary key clustered (rspRegistrationDataId asc)
)

GO

CREATE NONCLUSTERED INDEX [IX_RspRegistrationDatas_rspModificationDataId]
ON [Importer].[RspRegistrationDatas] ([rspModificationDataId])

GO

CREATE NONCLUSTERED INDEX [IX_RspRegistrationDatas_administrativecode]
ON [Importer].[RspRegistrationDatas] ([administrativecode])

