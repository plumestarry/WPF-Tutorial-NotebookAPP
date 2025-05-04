using Notebook.Common.Models;
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
    public class NotebookService : BaseService<NotebookDto>, INotebookService
    {
        private readonly HttpRestClient client;

        public NotebookService(HttpRestClient client) : base(client, "Notebook")
        {
            this.client = client;
        }

        public async Task<ApiResponse<PagedList<NotebookDto>>> GetAllFilterAsync(NotebookParameter parameter)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/Notebook/GetAll?pageIndex={parameter.PageIndex}" +
                $"&pageSize={parameter.PageSize}" +
                $"&search={parameter.Search}" +
                $"&status={parameter.Status}";
            return await client.ExecuteAsync<PagedList<NotebookDto>>(request);
        }

        public async Task<ApiResponse<SummaryDto>> SummaryAsync()
        {
            BaseRequest request = new BaseRequest();
            request.Route = "api/Notebook/Summary";
            return await client.ExecuteAsync<SummaryDto>(request);
        }
    }
}
