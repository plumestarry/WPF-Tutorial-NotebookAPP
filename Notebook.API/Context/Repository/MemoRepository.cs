using Microsoft.EntityFrameworkCore;

namespace Notebook.API.Context.Repository
{
    public class MemoRepository : Repository<Memo>, IRepository<Memo>
    {
        public MemoRepository(NotebookContext dbContext) : base(dbContext)
        {
        }
    }
}
