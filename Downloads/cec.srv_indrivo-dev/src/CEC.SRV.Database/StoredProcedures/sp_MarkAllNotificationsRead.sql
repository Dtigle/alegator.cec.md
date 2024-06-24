CREATE PROCEDURE [SRV].[sp_MarkAllNotificationsRead]
AS
	update SRV.NotificationReceivers 
	set notificationIsRead = 1, modified = SYSDATETIMEOFFSET(), modifiedById = 1 
	where notificationIsRead = 0

