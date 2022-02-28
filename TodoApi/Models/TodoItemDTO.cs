using System.Collections.Generic;

namespace TodoApi.Models
{
    // NOTE This is the DTO for todo items with tags
    public class TodoItemDTO
    {
        public long TodoItemId { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public ICollection<TodoTagDTO> Tags { get; set; }
    }
}