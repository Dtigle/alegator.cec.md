using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using CEC.SAISE.Domain.TemplateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Impl
{
    public class TemplateNameBLL : ITemplateNameBLL
    {
        private readonly ISaiseRepository _saiseRepository;

        public TemplateNameBLL(ISaiseRepository saiseRepository)
        {
            _saiseRepository = saiseRepository;
        }

        public async Task<TemplateName> GetTemplateNameAsync(long templateNameId)
        {
            var templateName = await _saiseRepository.QueryAsync<TemplateName>(z =>
                z.FirstOrDefault(x => x.Id == templateNameId));
            return templateName;
        }

        public async Task<List<TemplateNameDto>> GetAllTemplateNamesAsync()
        {
            var templateNames = await _saiseRepository.QueryAsync<TemplateName>(z => z.ToList());
            var listTemplateNameDto = new List<TemplateNameDto>();
            foreach (var templateName in templateNames)
            {
                listTemplateNameDto.Add(new TemplateNameDto { Id = templateName.Id, Title = templateName.Title, Description = templateName.Description });
            }
            return listTemplateNameDto;
        }

        public async Task<bool> DeleteTemplateNameAsync(long templateNameId)
        {
            try
            {
                var templateName = await GetTemplateNameAsync(templateNameId);
                if (templateName != null)
                {
                    await _saiseRepository.DeleteAsync(templateName);
                }
                return true;
            }
            catch (Exception e)
            {
                // Logging info if needed.
                return false;
            }
        }

        public async Task<TemplateNameDto> AddOrUpdateTemplateNameAsync(TemplateNameDto model)
        {
            if (model == null) throw new ArgumentNullException("model");

            var userId = SecurityHelper.GetLoggedUserId();
            var user = _saiseRepository.GetAsync<SystemUser>(userId).Result;

            TemplateName templateName = null;
            var newCreatedTemplatename = model.Id == 0;
            templateName = newCreatedTemplatename ? new TemplateName() : _saiseRepository.Get<TemplateName>(model.Id);

            templateName.Title = model.Title;
            templateName.Description = model.Description;
            templateName.EditDate = DateTime.Now;
            templateName.EditUser = user;

            _saiseRepository.SaveOrUpdate(templateName);

            if (newCreatedTemplatename)
            {
                _saiseRepository.SaveOrUpdate(templateName);
            }

            return new TemplateNameDto
            {
                Id = templateName.Id,
                Title = templateName.Title,
                Description = templateName.Description
            };
        }
    }
}
