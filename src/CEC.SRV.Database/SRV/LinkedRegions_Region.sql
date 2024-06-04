create table SRV.LinkedRegions_Region (
        linkedRegionId BIGINT not null,
       regionId BIGINT not null,
	   constraint FK_LinkedRegions_Region_Regions_regionId foreign key (regionId) references Lookup.Regions,
	   constraint FK_LinkedRegions_Region_LinkedRegions_linkedRegionId foreign key (linkedRegionId) references SRV.LinkedRegions
)