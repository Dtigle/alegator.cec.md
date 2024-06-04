 create table RSP.RegTypeCodes (
       regTypeCodeId BIGINT IDENTITY NOT NULL,
       name NVARCHAR(255) null,
       namerus NVARCHAR(255) null,
       primary key (regTypeCodeId)
    )