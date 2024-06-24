using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.Web.Api.Profiles;

namespace CEC.Web.Api.App_Start
{
    /// <summary>
    /// Provides the bootstrapping for <see cref="AutoMapper"/>
    /// </summary>
    public static class AutoMapperConfig
    {
        /// <summary>
        /// Initialize AutoMapper
        /// </summary>
        public static void Initialize()
        {
            Mapper.Initialize(arg =>
            {
                arg.AddProfile<PeopleProfile>();
            });
        }
    }
}