using System;
using System.Linq.Expressions;
using CEC.SRV.BLL.Responses;

namespace CEC.SRV.BLL
{
    public interface IService<TInterface> where TInterface : IBll
    {
        Responser<TResult> Exec<TResult>(Expression<Func<TInterface, TResult>> expression) where TResult : class;
        Responser<TResult> ExecUow<TResult>(Expression<Func<TInterface, TResult>> expression) where TResult : class;

    }
}