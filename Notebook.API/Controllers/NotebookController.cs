using Microsoft.AspNetCore.Mvc;
using Notebook.API.Context;
using Notebook.API.Service;
using Notebook.Shared.Dtos;

namespace Notebook.API.Controllers
{
    /// <summary>
    /// 待办事项控制器s
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NotebookController : ControllerBase
    {
        private readonly INotebookService service;

        public NotebookController(INotebookService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<ApiResponse> Get(int id) => await service.GetSingleAsync(id);

        [HttpGet]
        public async Task<ApiResponse> GetAll([FromQuery] NotebookParameter param) => await service.GetAllAsync(param);

        [HttpGet]
        public async Task<ApiResponse> Summary() => await service.Summary();

        [HttpPost]
        public async Task<ApiResponse> Add([FromBody] NotebookDto model) => await service.AddAsync(model);

        [HttpPost]
        public async Task<ApiResponse> Update([FromBody] NotebookDto model) => await service.UpdateAsync(model);

        [HttpDelete]
        public async Task<ApiResponse> Delete(int id) => await service.DeleteAsync(id);

    }
}
