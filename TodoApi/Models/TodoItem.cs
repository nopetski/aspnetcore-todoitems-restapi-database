using System.Collections.Generic;

namespace TodoApi.Models
{
    // This is the model for todo items
    public class TodoItem
    {
        public long TodoItemId { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public string User { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}