﻿using Notebook.Shared;
using Notebook.Shared.Contact;
using Notebook.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Service
{
    public interface ILoginService
    {
        Task<ApiResponse<UserDto>> Login(UserDto user);

        Task<ApiResponse> Register(UserDto user);
    }
}
