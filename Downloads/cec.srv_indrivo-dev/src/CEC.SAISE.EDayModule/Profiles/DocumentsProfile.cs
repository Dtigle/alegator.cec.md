using AutoMapper;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.EDayModule.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.SAISE.EDayModule.Profiles
{
    public class DocumentsProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<DocumentDto, DocumentDataModel>()
                        .ForMember(dest => dest.ReportParameterValues,
                                   opt => opt.MapFrom(src => src.ReportParameterValues));

            CreateMap<ReportParameterValueDto, ReportParameterValueModel>();

            CreateMap<DocumentDataModel, DocumentDto>();
            CreateMap<ReportParameterValueModel, ReportParameterValueDto>();
        }
    }
}