using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.SAISE.EDayModule.Profiles;

namespace CEC.SAISE.EDayModule.App_Start
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
                arg.AddProfile<UserDataProfile>();
                arg.AddProfile<VotingProfile>();
                arg.AddProfile<ElectionResultsProfile>();
                arg.AddProfile<CandidatesProfile>();
				arg.AddProfile<PollingStationEnablerProfile>();
				arg.AddProfile<VoterCertificatProfile>();
            });
        }
    }
}