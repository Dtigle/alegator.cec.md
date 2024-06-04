SET IDENTITY_INSERT SRV.People ON

DECLARE @SRV_People TABLE
(
	[personId] BIGINT NOT NULL,
    [idnp] NVARCHAR(13) not null unique,
    [firstName] NVARCHAR(255) not null,
    [surname] NVARCHAR(255) not null,
    [middleName] NVARCHAR(255) null,
	[dateOfBirth] DATETIME2 NOT NULL, 
    [genderId] BIGINT not null,
    [comments] NVARCHAR(255) null,
    [created] DATETIMEOFFSET null,
    [modified] DATETIMEOFFSET null,
    [deleted] DATETIMEOFFSET null,
    [createdById] NVARCHAR(255) null,
    [modifiedById] NVARCHAR(255) null,
    [deletedById] NVARCHAR(255) null
)

INSERT INTO @SRV_People ([personId], [idnp], [firstName], [surname], [middleName], [dateOfBirth], [genderId], [comments], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	( 1,  N'0934567890123',     N'Iurie', N'Furculiță',     N'Filip', '1973-02-24 00:00:00.000', 2, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 2,  N'2003447799113',       N'Ion',     N'Dînga',   N'Dumitru', '1985-05-21 00:00:00.000', 2, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 3,  N'2000554321123',    N'Vasile',     N'Grosu',    N'Andrei', '1958-04-04 00:00:00.000', 2, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 4,  N'2001987654321',      N'Inga',   N'Veleșcu',     N'Maxim', '1961-02-02 00:00:00.000', 3, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 5,  N'0956567890128',  N'Gheorghe',    N'Mînzat', N'Paraschiv', '1973-01-14 00:00:00.000', 2, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 6,  N'0933447799117',    N'Cornel',  N'Bohanțov',    N'Andrei', '1965-06-26 00:00:00.000', 2, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 7,  N'2002234554324',    N'Andrei',  N'Anghelov',     N'Mihai', '1960-07-01 00:00:00.000', 2, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 8,  N'0911254321896',       N'Ana',  N'Anghelov',    N'Vasile', '1981-08-03 00:00:00.000', 3, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 9,  N'2003456780129', N'Alexandru',  N'Anghelov',    N'Andrei', '1982-09-15 00:00:00.000', 2, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 10, N'0943447799153',   N'Tatiana',  N'Anghelov',    N'Andrei', '1979-10-30 00:00:00.000', 3, null, SYSDATETIMEOFFSET(), null, null, 1, null, null)
	
MERGE SRV.People AS target
USING
	(
		SELECT [personId], [idnp], [firstName], [surname], [middleName], [dateOfBirth], [genderId], [comments], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @SRV_People
	) AS source
ON source.[personId] = target.[personId]
WHEN MATCHED AND (source.[idnp] <> target.[idnp]
OR source.[firstName] <> target.[firstName]
OR source.[surname] <> target.[surname]
OR source.[middleName] <> target.[middleName]
OR source.[dateOfBirth] <> target.[dateOfBirth]
OR source.[genderId] <> target.[genderId]
OR source.[comments] <> target.[comments]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[idnp] = source.[idnp],
			target.[firstName] = source.[firstName],
			target.[surname] = source.[surname],
			target.[middleName] = source.[middleName],
			target.[dateOfBirth] = source.[dateOfBirth],
			target.[genderId] = source.[genderId],
			target.[comments] = source.[comments],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([personId], [idnp], [firstName], [surname], [middleName], [dateOfBirth], [genderId], [comments], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[personId], source.[idnp], source.[firstName], source.[surname], source.[middleName], source.[dateOfBirth], 
		source.[genderId], source.[comments], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);


SET IDENTITY_INSERT SRV.People OFF