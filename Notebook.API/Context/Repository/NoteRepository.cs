using Microsoft.EntityFrameworkCore;

namespace Notebook.API.Context.Repository
{

    public class NoteRepository : Repository<NotebookEntity>, IRepository<NotebookEntity>
    {
        public NoteRepository(NotebookContext dbContext) : base(dbContext)
        {
        }
    }
}
