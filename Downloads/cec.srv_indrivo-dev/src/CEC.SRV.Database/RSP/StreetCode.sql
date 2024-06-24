create table RSP.StreetTypeCodes (
       streetTypeCodeId BIGINT IDENTITY NOT NULL,
       docprint NVARCHAR(255) null,
       name NVARCHAR(255) null,
       namerus NVARCHAR(255) null,
       [rspStreetTypeCodeId] BIGINT NOT NULL, 
    primary key (streetTypeCodeId)
    )
