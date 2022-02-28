using System.Collections.Generic;

namespace TodoApi.Models
{
    // NOTE This is the DTO for listing tags with todo items
    public class TagDTO
    {
        public string TagId { get; set; }
        public int TodoCount { get; set; }
        public ICollection<TagTodoItemDTO> TodoItems { get; set; }
    }
}
