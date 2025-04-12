using Notebook.Shared.Dtos;

namespace Notebook.API.Service
{
    public interface INotebookService : IBaseService<NotebookDto>
    {
        Task<ApiResponse> GetAllAsync(NotebookParameter query);

        Task<ApiResponse> Summary();
    }
}
