using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using CEC.SRV.BLL.Repositories;

namespace CEC.SRV.BLL.Impl
{
    public class AuditerBll : IAuditerBll
    {
        private readonly IAuditerRepository _auditerRepository;

        public AuditerBll(IAuditerRepository auditerRepository)
        {
            _auditerRepository = auditerRepository;
        }

        public PageResponse<T> Get<T>(PageRequest request, long enityId) where T : AuditedEntity<IdentityUser>
        {
            return _auditerRepository.Get<T>(request, enityId);
        }
    }
}
