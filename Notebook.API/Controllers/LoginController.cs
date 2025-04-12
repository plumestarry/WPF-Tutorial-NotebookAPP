using Microsoft.AspNetCore.Mvc;
using Notebook.API.Context;
using Notebook.API.Service;
using Notebook.Shared.Dtos;
using Notebook.Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notebook.Api.Controllers
{
    /// <summary>
    /// 账户控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService service;

        public LoginController(ILoginService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<ApiResponse> Login([FromBody] UserDto param) => await service.LoginAsync(param.Account, param.PassWord);

        [HttpPost]
        public async Task<ApiResponse> Register([FromBody] UserDto param) => await service.Register(param);

    }
}
