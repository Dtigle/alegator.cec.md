update Importer.RspModificationDatas set statusConflictCode=64 where statusConflictCode=16

update Importer.RspModificationDatas  set statusConflictCode=16
where StatusConflictCode = 2 and ((status <> 'End')) and statusMessage not like '%bloc%'

update Importer.RspModificationDatas  set statusConflictCode=24
where StatusConflictCode = 10 and ((status <> 'End')) and statusMessage not like '%bloc%' and statusMessage like '%StreetCode%'