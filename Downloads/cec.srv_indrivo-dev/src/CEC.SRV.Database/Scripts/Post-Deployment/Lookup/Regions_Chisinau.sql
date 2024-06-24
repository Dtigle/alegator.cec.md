SET IDENTITY_INSERT Lookup.Regions ON

INSERT INTO @Lookup_Regions ([regionId], [name], [description], [parentId], [regionTypeId],[hasStreets], [saiseId], [registruId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById],[publicAdministrationId],[circumscription])
VALUES
	(2,		N'CHIȘINĂU',			null,  1, 4, 0, 44, 2801000, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, 1),
	--(39,	N'CHIȘINĂU',			null,  2, 6, 1,  44, 2801000, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(40,	N'BOTANICA',			null, 2, 5, 0, 4765, 2801142, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(41,	N'BUIUCANI',			null, 2, 5, 0, 4773, 2801103, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(42,	N'CENTRU',				null, 2, 5, 0, 4780, 2801101, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(43,	N'CIOCANA',				null, 2, 5, 0, 4788, 2801148, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(44,	N'RÎŞCANI',				null, 2, 5, 0, 4802, 2801102, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(45,	N'BĂCIOI',				null,  2, 8, 0, 4767, 2863707, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(46,	N'BĂCIOI',				null, 45, 9, 1, 4767, 2863707, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(47,	N'BRĂILA',			    null, 45, 9, 1, 4766, 2863706, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(48,	N'FRUMUȘICA',		    null, 45, 9, 1, 4769, 2863709, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(49,	N'STRĂISTENI',			null, 45, 9, 1, 4771, 2863708, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(50,	N'SÎNGERA',				null,  2, 7, 1, 4772, 2801004, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(51,	N'DOBROGEA',			null, 50, 9, 1, 4768, 2801005, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(52,	N'REVACA',				null, 50, 9, 1, 4770, 2801009, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(53,	N'CONDRIȚA',			null,  2, 9, 1, 4774, 2801012, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(54,	N'TRUȘENI',				null,  2, 8, 0, 4778, 2886792, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(55,	N'TRUȘENI',				null, 54, 9, 1, 4778, 2886792, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(56,	N'DUMBRAVA',			null, 54, 9, 1, 4775, 2801015, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(57,	N'DURLEȘTI',			null,  2, 7, 1, 4776, 2801002, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(58,	N'GHIDIGHICI',			null,  2, 9, 1, 4777, 2801013, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(59,	N'VATRA',				null,  2, 7, 1, 4779, 2801003, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(60,	N'CODRU',		        null,  2, 7, 1,4781, 2801001, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(61,	N'BUBUIECI',			null,  2, 8, 0, 4782, 2801007, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(62,	N'BUBUIECI',			null, 61, 9, 1, 4782, 2801007, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(63,	N'BÎC',					null, 61, 9, 1, 4785, 2801027, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(64,	N'HUMULEȘTI',			null, 61, 9, 1, 4791, 2871746, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(65,	N'BUDEȘTI',				null,  2, 9, 1, 4783, 2861711, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(66,	N'TOHATIN',				null,  2, 8, 0, 4792, 2801016, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(67,	N'TOHATIN',				null, 66, 9, 1, 4792, 2801016, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(68,	N'BUNEȚI',				null, 66, 9, 1, 4784, 2801025, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(69,	N'CHELTUITORI',			null, 66, 9, 1, 4787, 2801026, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(70,	N'CRUZEȘTI',			null,  2, 8, 0, 4790, 2801017, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(71,	N'CRUZEȘTI',			null, 70, 9, 1, 4790, 2801017, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(72,	N'CEROBORTA',			null, 70, 9, 1, 4786, 2801023, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(73,	N'COLONIȚA',			null,  2, 9, 1, 4789, 2801022, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(74,	N'VADUL LUI VODĂ',		null,  2, 7, 1, 4793, 2801006, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(75,	N'VĂDULENI',			null, 74, 9, 1, 4794, 2861712, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(76,	N'CIORESCU',			null,  2, 8, 0, 4795, 2801028, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(77,	N'CIORESCU',			null, 76, 9, 1, 4795, 2801028, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(78,	N'FĂUREȘTI',			null, 76, 9, 1, 4797, 2801014, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(79,	N'GOIAN',			    null, 76, 9, 1, 4798, 2801018, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(80,	N'CRICOVA',				null,  2, 7, 1, 4796, 2801010, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(81,	N'GRĂTIEȘTI',			null,  2, 8, 0, 4800, 2801020, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(82,	N'GRĂTIEȘTI',			null, 81, 9, 1, 4800, 2801020, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(83,	N'HULBOACA',			null, 81, 9, 1, 4801, 2801024, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),

	(84,	N'STĂUCENI',			null,  2, 8, 0, 4803, 2861454, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(85,	N'STĂUCENI',			null, 84, 9, 1, 4803, 2861454, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null),
	(86,	N'GOIANUL NOU',			null, 84, 9, 1, 4799, 2861455, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null)	

MERGE Lookup.Regions AS target
USING
	(
		SELECT [regionId], [name], [description], [parentId], [regionTypeId],[hasStreets], [saiseId], [registruId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById], [publicAdministrationId], [circumscription]
		FROM @Lookup_Regions
	) AS source
ON source.[regionId] = target.[regionId]
WHEN MATCHED AND (source.[name] <> target.[name]
OR source.[description] <> target.[description]
OR source.[parentId] <> target.[parentId]
OR source.[regionTypeId] <> target.[regionTypeId]
OR source.[hasStreets] <> target.[hasStreets]
OR source.[saiseId] <> target.[saiseId]
OR source.[registruId] <> target.[registruId]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById]
OR source.[publicAdministrationId] <> target.[publicAdministrationId]
OR source.[circumscription] <> target.[circumscription])
	THEN 
		UPDATE SET 
			target.[name] = source.[name],
			target.[description] = source.[description],
			target.[parentId] = source.[parentId],
			target.[regionTypeId] = source.[regionTypeId],
			target.[saiseId] = source.[saiseId],
			target.[registruId] = source.[registruId],
			target.[hasStreets] = source.[hasStreets],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById],
			target.[publicAdministrationId] = source.[publicAdministrationId],
			target.[circumscription] = source.[circumscription]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([regionId], [name], [description], [parentId], [regionTypeId], [hasStreets], [saiseId], [registruId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById], [publicAdministrationId], [circumscription])
		VALUES (source.[regionId], source.[name], source.[description], source.[parentId], source.[regionTypeId], source.[hasStreets], source.[saiseId], source.[registruId], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById], source.[publicAdministrationId], source.[circumscription]);

SET IDENTITY_INSERT Lookup.Regions OFF