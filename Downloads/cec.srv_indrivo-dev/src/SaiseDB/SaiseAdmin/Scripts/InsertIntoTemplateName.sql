﻿use [SAISE.Admin]

--SET IDENTITY_INSERT [SAISE].[TemplateNames] ON;
	INSERT INTO [SAISE].[TemplateNames] ([TemplateNameId], [TemplateTypeId], Title, EditUserId, EditDate, [Version]) VALUES
	(1, 1, N'Raportul BESV privind pregătirea SV', -1, GETDATE(), 1),
	(2, 1, N'Raportul BESV privind pregătirea SV în a 2-a zi', -1, GETDATE(), 1),
	(3, 1, N'Raport intermediar BESV (ora 14.00)', -1, GETDATE(), 1),
	(4, 1, N'Raport intermediar BESV (ora 14.00)', -1, GETDATE(), 1),
	(5, 1, N'Raport intermediar BESV (ora 21.00)', -1, GETDATE(), 1),
	(6, 1, N'Proces verbal BESV privind rezultatele numărării voturilor', -1, GETDATE(), 1),
	(7, 1, N'Proces verbal BESV privind rezultatele numărării voturilor', -1, GETDATE(), 1),
	(8, 1, N'Proces verbal BESV privind rezultatele numărării voturilor', -1, GETDATE(), 1),
	(9, 1, N'Proces verbal BESV privind rezultatele numărării voturilor', -1, GETDATE(), 1),
	(10, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(11, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(12, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(13, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(14, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(15, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(16, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(17, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(18, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(19, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(20, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(21, 1, N'Raportul final BESV', -1, GETDATE(), 1),
	(22, 1, N'Raportul intermediar CECE (ora 14.00)', -1, GETDATE(), 1),
	(23, 1, N'Raportul intermediar CECE (ora 14.00)', -1, GETDATE(), 1),
	(24, 1, N'Proces-verbal CECE', -1, GETDATE(), 1),
	(25, 1, N'Proces-verbal CECE', -1, GETDATE(), 1),
	(26, 1, N'Proces-verbal CECE', -1, GETDATE(), 1),
	(27, 1, N'Proces-verbal CECE', -1, GETDATE(), 1),
	(28, 1, N'Proces-verbal CECE', -1, GETDATE(), 1),
	(29, 1, N'Proces-verbal CECE', -1, GETDATE(), 1),
	(30, 1, N'Proces-verbal CECE', -1, GETDATE(), 1),
	(31, 1, N'Proces-verbal CECE', -1, GETDATE(), 1),
	(32, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(33, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(34, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(35, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(36, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(37, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(38, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(39, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(40, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(41, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(42, 1, N'Raportul final CECE', -1, GETDATE(), 1),
	(43, 1, N'Raportul final CECE', -1, GETDATE(), 1);
--SET IDENTITY_INSERT [dbo].[TemplateNames] OFF;

Use [SAISE.Admin]
delete from [SAISE].[TemplateNames]
