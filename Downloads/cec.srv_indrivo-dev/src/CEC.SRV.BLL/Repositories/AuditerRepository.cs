using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using NHibernate;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using NHibernate.Envers.Query.Criteria;

namespace CEC.SRV.BLL.Repositories
{
    public class AuditerRepository : IAuditerRepository
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly ISession _session;

        public AuditerRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _session = _sessionFactory.OpenSession();
        }

        public PageResponse<T> Get<T>(PageRequest request, long enityId) where T : AuditedEntity<IdentityUser>
        {
            var result = ApplyPaging<T>(request, AuditEntity.Id().Eq(enityId));

            return result;
        }

        private PageResponse<T> ApplyPaging<T>(PageRequest request, IAuditCriterion criterion) where T : AuditedEntity<IdentityUser>
        {
            var startIndex = (request.PageNumber - 1)*request.PageSize;
            var totalCount = _session.Auditer()
                .CreateQuery()
                .ForRevisionsOfEntity(typeof(T), false, true)
                .AddProjection(AuditEntity.RevisionNumber().Count())
                .Add(criterion)
                .GetSingleResult();

            var queryResult = _session.Auditer()
                .CreateQuery()
                .ForRevisionsOf<T>(true)
                .Add(criterion)
                .SetFirstResult(startIndex)
                .SetMaxResults(request.PageSize)
                .Results();

            return new PageResponse<T>
            {
                Items = queryResult.ToList(),
                PageSize = request.PageSize,
                StartIndex = request.PageNumber,
                Total = Convert.ToInt32(totalCount)
            };
        }
    }
}
