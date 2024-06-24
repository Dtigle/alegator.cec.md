
	SET IDENTITY_INSERT [dbo].[TemplateTypes] ON;
	INSERT INTO [dbo].[TemplateTypes] (TemplateTypeId, Title, EditUserId, EditDate, [Version]) 
		VALUES (1, 'Document', -1, GETDATE(), 1);
	SET IDENTITY_INSERT [dbo].[TemplateTypes] OFF;