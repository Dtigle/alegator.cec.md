CREATE TABLE Importer.RsaUsers (
	rsaUserId BIGINT IDENTITY NOT NULL,
	loginName NVARCHAR(255) not null,
	[password] NVARCHAR(255) not null,
	[source] NVARCHAR(255) null,
	status NVARCHAR(255) null,
	statusMessage NVARCHAR(255) null,
	created DATETIMEOFFSET null,
	statusDate DATETIMEOFFSET null,
	comments NVARCHAR(255) null,
	regionId BIGINT not null,
	PRIMARY KEY (rsaUserId),

	CONSTRAINT [FK_RsaUsers_Regions_regionId] FOREIGN KEY (regionId) REFERENCES Lookup.Regions
)

