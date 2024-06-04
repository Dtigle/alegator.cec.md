SET XACT_ABORT ON
SET IDENTITY_INSERT Lookup.Regions ON

BEGIN TRAN

declare @streetId bigint

INSERT INTO [Lookup].[Regions]
           ([regionId]
			,[name]
           ,[description]
           ,[parentId]
           ,[regionTypeId]
           ,[saiseId]
           ,[registruId]
           ,[statisticCode]
           ,[statisticIdentifier]
           ,[hasStreets]
           ,[geolatitude]
           ,[geolongitude]
           ,[created]
           ,[modified]
           ,[deleted]
           ,[createdById]
           ,[modifiedById]
           ,[deletedById]
           ,[publicAdministrationId]
           ,[circumscription])
     VALUES (-1,N'Fără Reședință',NULL,1,2,NULL,NULL,NULL,NULL,0,NULL,NULL,SYSDATETIMEOFFSET(),NULL,NULL,N'1',NULL,NULL,NULL,NULL)


INSERT INTO [Lookup].[Streets]
           ([name]
           ,[description]
           ,[regionId]
           ,[streetTypeId]
           ,[ropId]
           ,[saiseId]
           ,[created]
           ,[modified]
           ,[deleted]
           ,[createdById]
           ,[modifiedById]
           ,[deletedById])
     VALUES (N'>',null,-1,1,9999,null,SYSDATETIMEOFFSET(),null,null,N'1',null,null)


set @streetId = SCOPE_IDENTITY()

INSERT INTO [SRV].[Addresses]
           ([houseNumber]
           ,[suffix]
           ,[pollingStationId]
           ,[created]
           ,[modified]
           ,[deleted]
           ,[streetId]
           ,[createdById]
           ,[modifiedById]
           ,[deletedById]
           ,[buildingType]
           ,[geolatitude]
           ,[geolongitude])
     VALUES
           (null, null, null, SYSDATETIMEOFFSET(), null, null, @streetId, N'1', null, null,0, null, null )


COMMIT


SET IDENTITY_INSERT Lookup.Regions OFF