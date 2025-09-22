using System.ComponentModel.DataAnnotations;

namespace DbFirst.Dtos
{
    public class TodoListPostDto
    {
        //加入資料驗證功能 https://learn.microsoft.com/zh-tw/dotnet/api/system.componentmodel.dataannotations?view=net-9.0
        [Required]
        //[EmailAddress(ErrorMessage ="請輸入電子信箱")]
        //[StringLength(3)]
        //[RegularExpression("[a-z]")]
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
        public List<UploadFilePostDto> UploadFile { get; set; }

        public TodoListPostDto()
        {
            UploadFile = new List<UploadFilePostDto>();
        }
    }
}
