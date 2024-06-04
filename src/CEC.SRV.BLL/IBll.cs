using System.Collections.Generic;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;

namespace CEC.SRV.BLL
{
    public interface IBll
    {
        T Get<T>(long id) where T : Entity;
        IList<Region> GetAllLocalities(int? nodeId);
        IList<Region> GetLocalities(int? nodeId);
        PageResponse<T> Get<T>(PageRequest pageRequest) where T : Entity;
        PageResponse<T> SearchEntity<T>(PageRequest pageRequest) where T : Entity;
        IList<T> GetAll<T>() where T : Entity;
		IList<RegionType> GetRegionTypesByFilter(long regionId);
        void Delete<T>(long id) where T : Entity;
        void UnDelete<T>(long entityId) where T : SoftEntity<IdentityUser>;
        void SaveOrUpdate<T>(T entity) where T : Entity;
        PublicAdministration GetPublicAdministration(long regionId);
        IList<Region> GetRegion(string name, long regionTypeId, long? parentId);
        bool IsRegionAccessibleToCurrentUser(long currentRegionId);
		void VerificationIsRegionDeleted(long regionId);
		void VerificationIsDeletedLookup<T>(long id) where T : Lookup;
		void VerificationIsDeletedSrv<T>(long id) where T : SRVBaseEntity;
    }
}