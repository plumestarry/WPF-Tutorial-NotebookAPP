namespace Notebook.API.Context.Repository
{
    public class UserRepository : Repository<User>, IRepository<User>
    {
        public UserRepository(NotebookContext dbContext) : base(dbContext)
        {
        }
    }
}
