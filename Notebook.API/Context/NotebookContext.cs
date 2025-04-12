using Microsoft.EntityFrameworkCore;

namespace Notebook.API.Context
{
    public class NotebookContext : DbContext
    {
        public NotebookContext(DbContextOptions<NotebookContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<NotebookEntity> Notebooks { get; set; }
        public DbSet<Memo> Memos { get; set; }
    }
}
