using Notebook.Shared.Contact;
using Notebook.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Service
{
    public class LoginService : ILoginService
    {
        private readonly HttpRestClient client;
        private readonly string serviceName = "Login";

        public LoginService(HttpRestClient client)
        {
            this.client = client;
        }

        public async Task<ApiResponse<UserDto>> Login(UserDto user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Login";
            request.Parameter = user;
            return await client.ExecuteAsync<UserDto>(request);
        }

        public async Task<ApiResponse> Register(UserDto user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Register";
            request.Parameter = user;
            return await client.ExecuteAsync(request);
        }
    }
}
