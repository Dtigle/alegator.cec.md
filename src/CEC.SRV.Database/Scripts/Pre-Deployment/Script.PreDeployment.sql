/*
Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--print 'Post steps for $(BuildConfiguration)' 

/* User and Roles */
print 'Migrating Person Documents' 
:r .\SRV\RemovePersonDocumentTable.sql
print 'Migrating Person Documents succeeded.' 