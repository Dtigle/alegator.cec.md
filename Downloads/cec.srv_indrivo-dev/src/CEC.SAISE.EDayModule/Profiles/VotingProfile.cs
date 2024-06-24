using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.Domain;
using CEC.SAISE.EDayModule.Models.VoterCertificate;
using CEC.SAISE.EDayModule.Models.Voting;

namespace CEC.SAISE.EDayModule.Profiles
{
    public class VotingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<StatisticsDto, PollingStationStatisticsModel>();

            Mapper.CreateMap<UpdateVoterModel, VoterUpdateData>();

            Mapper.CreateMap<UpdateVoterResult, UpdateVoterResultModel>();





           
        }
    }
}