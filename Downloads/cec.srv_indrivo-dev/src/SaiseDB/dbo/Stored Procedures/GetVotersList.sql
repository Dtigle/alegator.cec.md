/****** Object:  StoredProcedure [dbo].[GetVotersList]    Script Date: 10/18/2023 6:15:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetVotersList]
(
    @ElectionRoundId	  bigint,
	@PollingStationId bigint,
	@AssignedCircumscriptionId bigint,
	@ElectionId bigint,
	@VoterStatus bigint
)
AS
SELECT 
	ROW_NUMBER() OVER(ORDER BY AV.[AssignedVoterId]) AS RowNumber
    , AV.[AssignedVoterId]
    , AV.[RegionId]
	, ISNULL('str. ' + V.StreetName, '') 
		+ ISNULL(' ' + CONVERT(VARCHAR, V.StreetNumber), '') 		
		+ ISNULL('/' + CONVERT(VARCHAR, V.StreetSubNumber), '') 
		+ ISNULL(' ap. ' + CONVERT(VARCHAR, V.BlockNumber), '')
		+ ISNULL(CONVERT(VARCHAR, V.BlockSubNumber), '') 
		+ ', ' + R.PersonAddress as 'PersonFullAdress'
    , PS1.NameRo AS 'RequestingPollingStationName'
    , PS1.Number AS 'RequestingPollingStationNumber'
	, PS2.Number AS 'PollingStationNumber'
    , AV.[PollingStationId]
	, AV.[RequestingPollingStationId]
    , PS2.NameRo AS 'PollingStationName' 
	, PS2.[Address] AS 'PSAdress'
	, R1.PollingStationAddress AS 'PollingStationAddress'
    , AV.[VoterId]
    , V.Idnp
    , V.LastNameRo +' '+V.NameRo +' '+ V.PatronymicRo AS 'VoterFullName'
    , V.DateOfBirth
    , V.PlaceOfResidence
    , V.DocumentNumber
    , AVS.[Signature]
    , AV.[Category]
    , AV.[Status]
    , AV.[Comment]
    , AV.[ElectionListNr]
    , AV.[EditUserId]
    , SU.UserName
    , AV.[EditDate]
    , AV.[Version]
	,E.[Type]
	,ET.[TypeName]

FROM 
    [dbo].[AssignedVoter] AS AV
LEFT JOIN 
    (
    SELECT 
        r1.RegionId,
        ISNULL(rt1.[Name] + ' ' + r1.[Name], '') + ', ' +
        ISNULL(CASE WHEN r2.RegionId = 1 THEN null ELSE rt2.[Name] + ' ' + r2.[Name] END, '') + ', ' +
        ISNULL(CASE WHEN r3.RegionId = 1 THEN null ELSE rt3.[Name] + ' ' + r3.[Name] END, '') AS PersonAddress

    FROM 
        [dbo].[Region] r1
    LEFT JOIN 
        [dbo].[Region] r2 ON r1.ParentId = r2.RegionId
    LEFT JOIN 
        [dbo].[Region] r3 ON r2.ParentId = r3.RegionId
    LEFT JOIN 
        [dbo].[RegionType] rt1 ON r1.RegionTypeId = rt1.RegionTypeId
    LEFT JOIN 
        [dbo].[RegionType] rt2 ON r2.RegionTypeId = rt2.RegionTypeId
    LEFT JOIN 
        [dbo].[RegionType] rt3 ON r3.RegionTypeId = rt3.RegionTypeId
    ) AS R ON R.RegionId = AV.RegionId
LEFT JOIN 
    [dbo].[PollingStation] AS PS1 ON PS1.PollingStationId = AV.RequestingPollingStationId
LEFT JOIN 
    [dbo].[PollingStation] AS PS2 ON PS2.PollingStationId = AV.PollingStationId
LEFT JOIN 
    [dbo].[Voter] AS V ON V.VoterId = AV.VoterId
LEFT JOIN 
    [dbo].[SystemUser] AS SU ON SU.SystemUserId = AV.EditUserId
LEFT JOIN 
    [dbo].[AssignedVoterStatistics] AS AVS ON AVS.AssignedVoterId = AV.AssignedVoterId
LEFT JOIN 
    [dbo].[AssignedPollingStation] AS APS ON APS.PollingStationId = AV.[PollingStationId]
LEFT JOIN 
	[dbo].[ElectionRound] AS ER ON ER.ElectionRoundId = APS.ElectionRoundId
LEFT JOIN [dbo].[Election] as E on E.ElectionId = ER.ElectionId
LEFT JOIN [dbo].[ElectionType] as ET on ET.ElectionTypeId = E.[Type]
LEFT JOIN 
    (
    SELECT 
        r1.RegionId,
        ISNULL(rt1.[Name] + ' ' + r1.[Name], '') + ', ' +
        ISNULL(CASE WHEN r2.RegionId = 1 THEN null ELSE rt2.[Name] + ' ' + r2.[Name] END, '') --+ ', ' +
        --ISNULL(CASE WHEN r3.RegionId = 1 THEN null ELSE rt3.[Name] + ' ' + r3.[Name] END, '') 
		AS PollingStationAddress

    FROM 
        [dbo].[Region] r1
    LEFT JOIN 
        [dbo].[Region] r2 ON r1.ParentId = r2.RegionId
    LEFT JOIN 
        [dbo].[Region] r3 ON r2.ParentId = r3.RegionId
    LEFT JOIN 
        [dbo].[RegionType] rt1 ON r1.RegionTypeId = rt1.RegionTypeId
    LEFT JOIN 
        [dbo].[RegionType] rt2 ON r2.RegionTypeId = rt2.RegionTypeId
    LEFT JOIN 
        [dbo].[RegionType] rt3 ON r3.RegionTypeId = rt3.RegionTypeId
    ) AS R1 ON R1.RegionId = PS2.RegionId
WHERE E.ElectionId = @ElectionId
AND APS.ElectionRoundId = @ElectionRoundId
AND APS.AssignedCircumscriptionId = @AssignedCircumscriptionId
AND AV.[PollingStationId] = @PollingStationId
AND AV.[Status] = @VoterStatus;