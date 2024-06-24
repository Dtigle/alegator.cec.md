using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amdaris.Domain.Paging;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.Domain;
using CEC.SAISE.Domain.TemplateManager;

namespace CEC.SAISE.BLL
{
    public interface IUserBll
    {
        SystemUser GetById(long userId);
        Task<SystemUser> GetByIdAsync(long userId);
        Task<UserDataDto> GetCurrentUserData();
        Task<IList<Election>> GetAccessibleElections();
        Task<PageResponse<Election>> GetAccessibleElectionsAsync(PageRequest request);
        PageResponse<AssignedCircumscription> GetAccessibleCircumscriptions(PageRequest request, long? electionId);
        PageResponse<Region> GetAccessibleRegions(PageRequest request, long electionId, long? regionId);
		IList<Region> GetAccessibleCircumscription(long electionId);
		IList<Region> GetAccessibleRegions(long electionId, long regionId);
        PageResponse<PollingStation> GetAccessiblePollingStations(PageRequest request, long electionId, long? circumscriptionId, long? regionId);
        PageResponse<TemplateNameDto> GetAccessibleTemplateNames(PageRequest request, long electionId);
        PageResponse<TemplateNameDto> GetAccessibleTemplateNamesGrid(PageRequest request, long electionId);
    }
}
