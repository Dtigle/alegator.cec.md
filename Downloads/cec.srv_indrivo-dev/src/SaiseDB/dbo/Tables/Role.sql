CREATE TABLE [dbo].[Role] (
    [RoleId]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (MAX) NOT NULL,
    [Level]      INT            CONSTRAINT [DF_Role_Level] DEFAULT ((1)) NOT NULL,
    [EditUserId] BIGINT         CONSTRAINT [DF_Role_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]   DATETIME       CONSTRAINT [DF_Role_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]    INT            CONSTRAINT [DF_Role_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId] ASC) WITH (FILLFACTOR = 80)
);

