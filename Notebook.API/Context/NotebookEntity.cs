namespace Notebook.API.Context
{
    public class NotebookEntity : BaseEntity
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int Status { get; set; }
    }
}
