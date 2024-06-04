CREATE TABLE [Audit].[StayStatements_AUD]
(
	stayStatementId BIGINT not null,
	REV INT not null,
    REVTYPE TINYINT not null,
    created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    personId BIGINT null,
    baseAddressId BIGINT null,
    declaredStayAddressId BIGINT null,
    electionInstanceId BIGINT null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null,
    CONSTRAINT [PK_StayStatements_AUD] primary key (stayStatementId, REV),
	constraint FK79EE0E8CAAE62361 foreign key (REV) references Audit.REVINFO
)
