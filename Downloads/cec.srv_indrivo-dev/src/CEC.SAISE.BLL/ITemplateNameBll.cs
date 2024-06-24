using CEC.SAISE.BLL.Dto.TemplateManager;
using CEC.SAISE.Domain.TemplateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL
{
    public interface ITemplateNameBLL
    {
        /// <summary>
        /// Retrieves all template names.
        /// </summary>
        /// <returns>A list of all template names.</returns>
        Task<List<TemplateNameDto>> GetAllTemplateNamesAsync();

        /// <summary>
        /// Retrieves a specific template name by its ID.
        /// </summary>
        /// <param name="id">The ID of the template name to retrieve.</param>
        /// <returns>The template name if found; otherwise, null.</returns>
        Task<TemplateName> GetTemplateNameAsync(long id);

        /// <summary>
        /// Adds or updates a template name.
        /// </summary>
        /// <param name="templateName">The template name to add or update.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        Task<TemplateNameDto> AddOrUpdateTemplateNameAsync(TemplateNameDto templateName);

        /// <summary>
        /// Deletes a specific template name by its ID.
        /// </summary>
        /// <param name="id">The ID of the template name to delete.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        Task<bool> DeleteTemplateNameAsync(long id);
    }
}
