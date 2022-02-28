using System.Collections.Generic;

namespace TodoApi.Models
{
    public static class Helpers
    {
        public static TodoItemDTO TodoToDTO (this TodoItem item)
        {
            var todoItem = new TodoItemDTO
            {
                TodoItemId = item.TodoItemId,
                IsComplete = item.IsComplete,
                Name = item.Name,
                Tags = ConvertTags(item.Tags)
            };

            return todoItem;
        }

        // TODO Saiskohan t‰m‰n ja toisen metodin yhdistetty‰ jotenkin?
        public static List<TodoTagDTO> ConvertTags(ICollection<Tag> source)
        {
            if (source == null) return null;

            List<TodoTagDTO> list = new List<TodoTagDTO>();

            foreach (Tag tag in source) {
                TodoTagDTO t = new TodoTagDTO { TagId = tag.TagId };
                list.Add(t);
            }

            return list;
        }

        public static TagDTO TagToDTO(this Tag item)
        {
            var tag = new TagDTO
            {
                TagId = item.TagId,
                TodoCount = item.TodoItems.Count,
                TodoItems = ConvertTodos(item.TodoItems)
            };

            return tag;
        }

        // TODO katso ylemp‰‰
        public static List<TagTodoItemDTO> ConvertTodos(ICollection<TodoItem> source)
        {
            if (source == null) return null;

            List<TagTodoItemDTO> list = new List<TagTodoItemDTO>();

            foreach (TodoItem todo in source)
            {
                TagTodoItemDTO i = new TagTodoItemDTO
                {
                    TodoItemId = todo.TodoItemId,
                    Name = todo.Name,
                    IsComplete = todo.IsComplete,
                    User = todo.User
                };

                list.Add(i);
            }

            return list;
        }
    }
}