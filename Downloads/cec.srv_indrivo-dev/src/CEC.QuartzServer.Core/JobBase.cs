using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.NHibernateProvider;
using Quartz;

namespace CEC.QuartzServer.Core
{
    public abstract class JobBase : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (var uow = new NhUnitOfWork())
                {
                    Execute();
                    uow.Complete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected abstract void Execute();
    }
}
