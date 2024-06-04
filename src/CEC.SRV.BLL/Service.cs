using System;
using System.Linq.Expressions;
using Amdaris.NHibernateProvider;
using CEC.SRV.BLL.Responses;

namespace CEC.SRV.BLL
{
    public class Service<TInterface> : IService<TInterface> where TInterface : IBll
    {
        private readonly TInterface _instance;

        public Service(TInterface instance)
        {
            _instance = instance;
        }

        public Responser<TResult> Exec<TResult>(Expression<Func<TInterface, TResult>> expression) where TResult : class
        {
            try
            {
                var method = expression.Compile();

                var result = method(_instance);
                
                return new Responser<TResult>()
                    .WithStatusCode(StatusCode.OK).WithModel(result);
            }
            catch (Exception ex)
            {
                return new Responser<TResult>().AddError(ex.Message);
            }
        }

        public Responser<TResult> ExecUow<TResult>(Expression<Func<TInterface, TResult>> expression) where TResult : class
        {
            using (var uow = new NhUnitOfWork())
            {
                var result = Exec(expression);

                if (result.Status.Succeeded)
                {
                    uow.Complete();
                }
               
                return result;
            }
        }
    }
}