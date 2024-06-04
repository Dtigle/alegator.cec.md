using NHibernate;

namespace CEC.SRV.BLL.Repositories
{
    public class WebApiSrvRepository : SrvRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public WebApiSrvRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        
        public override ISession Session
        {
            get
            {
                return _sessionFactory.GetCurrentSession();
            }
        }
    }
}
