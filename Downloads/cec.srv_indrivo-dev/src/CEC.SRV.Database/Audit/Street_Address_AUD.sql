CREATE TABLE [Audit].[Street_Address_AUD]
(
	 REV INT not null,
       addressId BIGINT not null,
       REVTYPE TINYINT not null,
       CONSTRAINT [PK_Street_Address_AUD] primary key (REV, addressId),
	   constraint FKF1B7B397AAE62361   foreign key (REV) references Audit.REVINFO
)
