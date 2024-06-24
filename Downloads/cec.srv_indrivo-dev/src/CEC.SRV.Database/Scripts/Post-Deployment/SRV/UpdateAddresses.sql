
UPDATE SRV.Addresses
	SET [pollingStationId] = 1
	WHERE [addressId] >= 1
	  AND [addressId] <= 5;

UPDATE SRV.Addresses
	SET [pollingStationId] = 2
	WHERE [addressId] = 6;
		
