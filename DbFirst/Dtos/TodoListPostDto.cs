namespace DbFirst.Dtos
{
    public class TodoListPostDto
    {
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
        public List<UploadFilePostDto> UploadFile { get; set; }
    }
}
