/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

USE [IAPWEM_Core]
GO
	
IF OBJECT_ID ( 'GetAppServiceAccountsByPartitionkeyAndRowkey', 'P' ) IS NOT NULL 
	DROP PROCEDURE GetAppServiceAccountsByPartitionkeyAndRowkey;
GO
	
CREATE PROCEDURE GetAppServiceAccountsByPartitionkeyAndRowkey

@partitionKey nvarchar(50),
@rowKey nvarchar(50)
AS
BEGIN
 
			
SELECT TOP 1 
[CipherKeyIndex], 
[CompanyId], 
[CreatedOn], 
[Description], 
[Domain], 
[IsActive], 
[LastModifiedOn], 
[PartitionKey], 
[RowKey], 
[ServiceId], 
[ServicePassword], 
[Timestamp] 
FROM AppServiceAccounts
WHERE 
[PartitionKey] = @partitionKey AND 
[RowKey] = @rowKey
			
END

GO

-- End of Procedure
	
IF OBJECT_ID ( 'GetAllFromAppServiceAccounts', 'P' ) IS NOT NULL 
	DROP PROCEDURE GetAllFromAppServiceAccounts;
GO
	
CREATE PROCEDURE GetAllFromAppServiceAccounts

AS
BEGIN
SELECT
[CipherKeyIndex], 
[CompanyId], 
[CreatedOn], 
[Description], 
[Domain], 
[IsActive], 
[LastModifiedOn], 
[PartitionKey], 
[RowKey], 
[ServiceId], 
[ServicePassword], 
[Timestamp] 
FROM AppServiceAccounts

			
END

GO

-- End of Procedure
	
IF OBJECT_ID ( 'InsertAppServiceAccounts', 'P' ) IS NOT NULL 
	DROP PROCEDURE InsertAppServiceAccounts;
GO
	
CREATE PROCEDURE InsertAppServiceAccounts

@cipherKeyIndex int,
@companyId uniqueidentifier,
@createdOn datetime,
@description nvarchar(500),
@domain nvarchar(50),
@isActive bit,
@lastModifiedOn datetime,
@partitionKey nvarchar(50),
@rowKey nvarchar(50),
@serviceId nvarchar(50),
@servicePassword nvarchar(50)
AS
BEGIN

INSERT INTO AppServiceAccounts
(
[CipherKeyIndex], 
[CompanyId], 
[CreatedOn], 
[Description], 
[Domain], 
[IsActive], 
[LastModifiedOn], 
[PartitionKey], 
[RowKey], 
[ServiceId], 
[ServicePassword]
)
VALUES
(
@cipherKeyIndex, 
@companyId, 
@createdOn, 
@description, 
@domain, 
@isActive, 
@lastModifiedOn, 
@partitionKey, 
@rowKey, 
@serviceId, 
@servicePassword
)

		
END

GO

-- End of Procedure
	
IF OBJECT_ID ( 'UpdateAppServiceAccounts', 'P' ) IS NOT NULL 
	DROP PROCEDURE UpdateAppServiceAccounts;
GO
	
CREATE PROCEDURE UpdateAppServiceAccounts

@cipherKeyIndex int,
@companyId uniqueidentifier,
@createdOn datetime,
@description nvarchar(500),
@domain nvarchar(50),
@isActive bit,
@lastModifiedOn datetime,
@partitionKey nvarchar(50),
@rowKey nvarchar(50),
@serviceId nvarchar(50),
@servicePassword nvarchar(50)
AS
BEGIN

UPDATE AppServiceAccounts
SET 
[CipherKeyIndex] = @cipherKeyIndex, 
[CompanyId] = @companyId, 
[CreatedOn] = @createdOn, 
[Description] = @description, 
[Domain] = @domain, 
[IsActive] = @isActive, 
[LastModifiedOn] = @lastModifiedOn, 
[ServiceId] = @serviceId, 
[ServicePassword] = @servicePassword
WHERE 
[PartitionKey] = @partitionKey AND 
[RowKey] = @rowKey

END

GO

-- End of Procedure
	
IF OBJECT_ID ( 'DeleteAppServiceAccounts', 'P' ) IS NOT NULL 
	DROP PROCEDURE DeleteAppServiceAccounts;
GO
	
CREATE PROCEDURE DeleteAppServiceAccounts

@partitionKey nvarchar(50),
@rowKey nvarchar(50)
AS
BEGIN
			
DELETE FROM AppServiceAccounts
WHERE 
[PartitionKey] = @partitionKey AND 
[RowKey] = @rowKey
			
END

GO

-- End of Procedure
	
IF OBJECT_ID ( 'GetAppServiceAccountsByPartitionKey', 'P' ) IS NOT NULL 
	DROP PROCEDURE GetAppServiceAccountsByPartitionKey;
GO
	
CREATE PROCEDURE GetAppServiceAccountsByPartitionKey

@partitionKey nvarchar(50)
AS
BEGIN
 

SELECT 
[CipherKeyIndex], 
[CompanyId], 
[CreatedOn], 
[Description], 
[Domain], 
[IsActive], 
[LastModifiedOn], 
[PartitionKey], 
[RowKey], 
[ServiceId], 
[ServicePassword], 
[Timestamp] 
FROM AppServiceAccounts
WHERE 
[PartitionKey] = @partitionKey

			
END

GO

-- End of Procedure
