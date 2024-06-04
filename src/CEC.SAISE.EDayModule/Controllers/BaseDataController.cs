using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amdaris.NHibernateProvider.Web;

namespace CEC.SAISE.EDayModule.Controllers
{
    [NHibernateSession]
    public class BaseDataController : BaseController
    {
    }
}