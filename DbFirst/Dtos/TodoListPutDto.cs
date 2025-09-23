using DbFirst.ValidationAttributes;

namespace DbFirst.Dtos
{
    public class TodoListPutDto
    {
        public Guid TodoId { get; set; }
        
        [TodoName]
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
        public List<UploadFilePostDto> UploadFiles { get; set; }

        public TodoListPutDto()
        {
            UploadFiles = new List<UploadFilePostDto>(); 
        }
    }
}
