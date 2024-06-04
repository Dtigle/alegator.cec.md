CREATE TABLE [Audit].[AdditionalUserInfos_AUD]
(
	additionalUserInfoId BIGINT not null,
       REV INT not null,
       REVTYPE TINYINT not null,
       firstName NVARCHAR(255) null,
       lastName NVARCHAR(255) null,
       dateOfBirth DATETIME null,
       email NVARCHAR(255) null,
       landlinePhone NVARCHAR(255) null,
       mobPhone NVARCHAR(255) null,
       workInfo NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
	[genderId] BIGINT NULL, 
    CONSTRAINT [PK_AdditionalUserInfos_AUD] PRIMARY KEY (additionalUserInfoId, REV),
	constraint [FK69AE9A4FAAE62361] foreign key (REV) references Audit.REVINFO
)
