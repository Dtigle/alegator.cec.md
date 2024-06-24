/*
Post-Deployment Script Template							
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
print 'Inserting users data' 
--:r .\Access\IdentityRoles.sql
--:r .\Access\IdentityUsers.sql
--:r .\Access\UserRoles.sql

/* Lookup */
print 'Inserting lookup data' 
--:r .\Lookup\StreetTypes.sql
--:r .\Lookup\RegionTypes.sql
--:r .\Lookup\PersonStatusTypes.sql
:r .\Lookup\PersonAddressTypes.sql
--:r .\Lookup\ManagerTypes.sql
:r .\Lookup\DocumentTypes.sql
--:r .\Lookup\ElectionTypes.sql
--:r .\Lookup\Genders.sql

--:r .\Lookup\DisableConstraints.sql
--:r .\Lookup\Regions_Country.sql
--:r .\Lookup\Regions_Chisinau.sql
--:r .\Lookup\Streets_Chisinau.sql
--:r .\Lookup\Regions.sql
--:r .\Lookup\EnableConstraints.sql

/* SRV */
--:r .\SRV\PollingStations.sql
--:r .\SRV\Addresses.sql
--:r .\SRV\PollingStations.sql
--:r .\SRV\UpdateAddresses.sql
--:r .\SRV\People.sql
--:r .\SRV\PersonStatus.sql
--:r .\SRV\UpdatePeopleWithStatus.sql
--:r .\SRV\PersonAddresses.sql
--:r .\SRV\Events.sql
--:r .\SRV\Notifications.sql
--:r .\SRV\NotificationsReceivers.sql
:r .\SRV\DefaultSettingValues.sql

/* RSA Users */
print 'Inserting RSA users'
--:r .\RSAUsers\AdditionalUserInfos.sql
--:r .\RSAUsers\IdentityUsers.sql
--:r .\RSAUsers\UserRoles.sql
--:r .\RSAUsers\SRVIdentityUsersRegions.sql


print 'Insert RSP lookups'
:r .\RSP\StreetCode.sql
:r .\RSP\RegTypeCode.sql
:r .\RSP\DocTypeCode.sql
:r .\RSP\SexCode.sql