CREATE PROCEDURE [SRV].[sp_GenerateVotersList]
	@pollingStationId bigint,
	@electionDate as datetime2
AS
BEGIN
declare @isNoStreetsRegion int
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    --check if region uses Streets
	set @isNoStreetsRegion = (select count(r.hasStreets) from SRV.PollingStations ps
								inner join Lookup.Regions r on ps.regionId = r.regionId
								where ps.pollingStationId = @pollingStationId and r.hasStreets = 0)

	if @isNoStreetsRegion = 0
	begin
		--select for regions with streets
		SELECT    row_number() over(order by RegionName, StreetName, isnull(houseNumber,0), isnull(houseSuffix,''), isnull(apNumber,0), isnull(apSuffix,''), surname, firstName, middleName, dateOfBirth) AS rowNr,
					pollingStationId, personId, idnp, DocSeria, DocNumber, firstName, surname, 
					middleName, dbo.TitleCaseNet4(surname + ' ' + firstName) as fullName, dateOfBirth, RegionName, StreetName, houseNumber, houseSuffix, apNumber, 
					apSuffix, EligibleAddress, fullApNumber, [Status], AddressType, residenceExpirationDate
		from
		(
			select  ps.pollingStationId, ps.number as PollingStationNumber,
					p.personId, p.idnp, p.firstName, p.surname, p.middleName, p.dateOfBirth, 
					g.name as Gender,
					vb.RegionName, vb.StreetName, vb.houseNumber, vb.suffix as houseSuffix, vb.fullBuildingAddress as EligibleAddress,
					pa.apNumber, pa.apSuffix, SRV.fn_BuildingNumberConcat(pa.apNumber, pa.apSuffix) as fullApNumber,
					pat.name as AddressType,
					pa.dateOfExpiration as residenceExpirationDate,
					p.doc_seria as DocSeria, p.doc_number as DocNumber, 
					pst.name as [Status]
			from SRV.People p
			inner join Lookup.Genders g on p.genderId = g.genderId
			inner join SRV.PersonAddresses pa on p.personId = pa.personId
			inner join Lookup.PersonAddressTypes pat on pa.personAddressTypeId = pat.personAddressTypeId
			inner join SRV.PersonStatuses stat on p.personId = stat.personId
			inner join Lookup.PersonStatusTypes pst on stat.statusTypeId = pst.personStatusTypeId
			inner join SRV.v_Buildings vb on pa.addressId = vb.addressId
			inner join SRV.PollingStations ps on vb.pollingStationId = ps.pollingStationId
			where 
				pa.isEligible = 1 and (pa.dateOfExpiration is null or pa.dateOfExpiration > @electionDate)
				and pst.isExcludable = 0
				and stat.isCurrent = 1
				and p.deleted is null and pa.deleted is null and ps.deleted is null
		) t
		WHERE (t.pollingStationId = @pollingStationId) and (dateOfBirth <= DATEADD(year, -18, @electionDate))
	end
	else
	begin
		--select for regions without streets/addresses
		SELECT    row_number() over(order by surname, firstName, middleName, dateOfBirth) AS rowNr,
					pollingStationId, personId, idnp, DocSeria, DocNumber, firstName, surname, 
					middleName, dbo.TitleCaseNet4(surname + ' ' + firstName) as fullName, dateOfBirth, RegionName, StreetName, houseNumber, houseSuffix, apNumber, 
					apSuffix, EligibleAddress, fullApNumber, [Status], AddressType, residenceExpirationDate
		from
		(
			select  ps.pollingStationId, ps.number as PollingStationNumber,
					p.personId, p.idnp, p.firstName, p.surname, p.middleName, p.dateOfBirth, 
					g.name as Gender,
					vb.RegionName, N'< str.' as StreetName, 0 as houseNumber, '' as houseSuffix, '' as EligibleAddress,
					0 as apNumber, '' as apSuffix, 0 as fullApNumber,
					pat.name as AddressType,
					pa.dateOfExpiration as residenceExpirationDate,
					p.doc_seria as DocSeria, p.doc_number as DocNumber, 
					pst.name as [Status]
			from SRV.People p
			inner join Lookup.Genders g on p.genderId = g.genderId
			inner join SRV.PersonAddresses pa on p.personId = pa.personId
			inner join Lookup.PersonAddressTypes pat on pa.personAddressTypeId = pat.personAddressTypeId
			inner join SRV.PersonStatuses stat on p.personId = stat.personId
			inner join Lookup.PersonStatusTypes pst on stat.statusTypeId = pst.personStatusTypeId
			inner join SRV.v_Buildings vb on pa.addressId = vb.addressId
			inner join SRV.PollingStations ps on vb.pollingStationId = ps.pollingStationId
			where 
				pa.isEligible = 1 and (pa.dateOfExpiration is null or pa.dateOfExpiration > @electionDate)
				and pst.isExcludable = 0
				and stat.isCurrent = 1
				and p.deleted is null and pa.deleted is null and ps.deleted is null
		) t
		WHERE (t.pollingStationId = @pollingStationId) and (dateOfBirth <= DATEADD(year, -18, @electionDate))
	end
END

