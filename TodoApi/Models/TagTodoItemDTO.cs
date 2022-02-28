namespace TodoApi.Models
{
    // NOTE This is the DTO for todo items in tags
    public class TagTodoItemDTO
    {
        public long TodoItemId { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public string User { get; set; }
    }
}