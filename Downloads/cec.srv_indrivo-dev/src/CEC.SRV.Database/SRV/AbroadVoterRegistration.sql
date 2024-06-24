CREATE TABLE [SRV].[AbroadVoterRegistrations]
(
	[abroadVoterRegistrationId] bigint identity NOT NULL, 
	[personId] BIGINT NOT NULL, 
    [abroadAddress] NVARCHAR(255) NOT NULL, 
	[abroadAddressCountry] NVARCHAR(255), 
	[abroadAddressLat] FLOAT , 
	[abroadAddressLong] FLOAT , 
    [residenceAddress] NCHAR(255) NOT NULL, 
	[email] NVARCHAR(255) NOT NULL,
	[ipAddress] NVARCHAR(255),    
    [created] DATETIMEOFFSET NOT NULL, 
	CONSTRAINT [PK_AbroadVoterRegistrations] primary key ([abroadVoterRegistrationId]),
    CONSTRAINT [FK_AbroadVoterRegistrations_People_personid] FOREIGN KEY ([personId]) REFERENCES [SRV].[People]
)
