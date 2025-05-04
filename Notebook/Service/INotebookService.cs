using Notebook.Shared;
using Notebook.Shared.Contact;
using Notebook.Shared.Dtos;
using Notebook.Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Service
{
    public interface INotebookService : IBaseService<NotebookDto>
    {
        Task<ApiResponse<PagedList<NotebookDto>>> GetAllFilterAsync(NotebookParameter parameter);

        Task<ApiResponse<SummaryDto>> SummaryAsync();
    }
}
