﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;

namespace CEC.SRV.BLL.Repositories
{
    public interface IAuditerRepository
    {
        PageResponse<T> Get<T>(PageRequest request, long enityId) where T : AuditedEntity<IdentityUser>;
    }
}