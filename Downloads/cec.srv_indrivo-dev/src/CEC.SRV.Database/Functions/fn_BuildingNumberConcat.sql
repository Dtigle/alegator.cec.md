CREATE FUNCTION [SRV].[fn_BuildingNumberConcat] 
(
	@houseNumber int,
	@houseSuffix nvarchar(10)
)
RETURNS nvarchar(50)
WITH SCHEMABINDING
AS
BEGIN
	DECLARE @Result nvarchar(50)
	declare @suffix nvarchar(10)

	set @suffix = LTRIM(RTRIM(@houseSuffix))

	if @houseNumber is not null and @houseNumber > 0
	begin
		set @Result = cast(@houseNumber as nvarchar(10))
		if @houseSuffix is not null
		begin
			if ISNUMERIC(SUBSTRING(@suffix,1,1)) <> 0
				set @Result = @Result + '/' + @suffix
			else
				set @Result = @Result + @suffix
		end
	end

	RETURN @Result

END
