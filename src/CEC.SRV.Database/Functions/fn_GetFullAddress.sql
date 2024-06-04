CREATE FUNCTION [SRV].[fn_GetFullAddress] 
(
	@fullStreetName nvarchar(50),
	@houseNumber int,
	@houseSuffix nvarchar(10)
)
RETURNS nvarchar(100)
AS
BEGIN
	DECLARE @Result nvarchar(100)
	declare @buildingNumber nvarchar(50)

	set @Result = @fullStreetName

	if @fullStreetName is not null
	begin
		set @buildingNumber = [SRV].[fn_BuildingNumberConcat](@houseNumber, @houseSuffix)
		if @buildingNumber is not null
			set @Result = @fullStreetName + ', ' + @buildingNumber
	end

	RETURN @Result

END
