using Notebook.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Service
{
    public class MemoService : BaseService<MemoDto>, IMemoService
    {
        public MemoService(HttpRestClient client) : base(client, "Memo")
        {

        }
    }
}
