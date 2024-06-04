using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using CEC.SRV.BLL;
using CEC.Web.Api.Dtos.People;

namespace CEC.Web.Api.Controllers
{
    [RoutePrefix("People")]
    [NHibernateSession]
    public class PeopleController : ApiController
    {
        private readonly IVotersBll _votersBll;

        public PeopleController(IVotersBll votersBll)
        {
            _votersBll = votersBll;
        }

        [Route("{idnp:regex(^(09|20){1}[0-9]{11}$)}")]
        public PersonData GetByIdnp(string idnp)
        {
            var person = _votersBll.GetByIdnp(idnp);
            var result = Mapper.Map<PersonData>(person);

            return result;
        }
    }
}
