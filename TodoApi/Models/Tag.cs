using System.Collections.Generic;

namespace TodoApi.Models
{
    // This is the models for tags
    public class Tag
    {
        public string TagId { get; set; }
        public int TodoCount { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
