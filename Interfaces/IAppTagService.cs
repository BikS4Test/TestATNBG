
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectName.Types;

namespace ProjectName.Interfaces
{
    /// <summary>
    /// Interface for managing application tags.
    /// </summary>
    public interface IAppTagService
    {
        /// <summary>
        /// Creates a new application tag.
        /// </summary>
        /// <param name="createAppTagDto">Data transfer object for creating an application tag.</param>
        /// <returns>A string representing the result of the creation operation.</returns>
        Task<string> CreateAppTag(CreateAppTagDto createAppTagDto);

        /// <summary>
        /// Retrieves an application tag based on the provided request data.
        /// </summary>
        /// <param name="requestDto">Data transfer object containing request details for retrieving an application tag.</param>
        /// <returns>An instance of AppTag representing the retrieved tag.</returns>
        Task<AppTag> GetAppTag(AppTagRequestDto requestDto);

        /// <summary>
        /// Updates an existing application tag.
        /// </summary>
        /// <param name="updateAppTagDto">Data transfer object for updating an application tag.</param>
        /// <returns>A string representing the result of the update operation.</returns>
        Task<string> UpdateAppTag(UpdateAppTagDto updateAppTagDto);

        /// <summary>
        /// Deletes an application tag.
        /// </summary>
        /// <param name="deleteAppTagDto">Data transfer object for deleting an application tag.</param>
        /// <returns>A boolean indicating the success of the deletion operation.</returns>
        Task<bool> DeleteAppTag(DeleteAppTagDto deleteAppTagDto);

        /// <summary>
        /// Retrieves a list of application tags based on the provided request data.
        /// </summary>
        /// <param name="requestDto">Data transfer object containing request details for retrieving a list of application tags.</param>
        /// <returns>A list of AppTag instances representing the retrieved tags.</returns>
        Task<List<AppTag>> GetListAppTag(ListAppTagRequestDto requestDto);
    }
}
