using AutoMapper;
using DbFirst.Dtos;
using DbFirst.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DbFirst.Controllers
{
    [Route("api/Todo/{TodoId}/UploadFile")]
    [ApiController]
    public class TodoUploadFileController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly IMapper _mapper;

        public TodoUploadFileController(TodoContext todoContext, IMapper mapper)
        {
            _todoContext = todoContext;
            _mapper = mapper;
        }


        // GET: api/<TodoUploadFileController>
        [HttpGet]
        public ActionResult<IEnumerable<UploadFileDto>> Get(Guid TodoId)
        {
            if (!_todoContext.TodoList.Any(a => a.TodoId == TodoId))
            {
                return NotFound("找不到該事項");
            }

            var result = from a in _todoContext.UploadFile
                         where a.TodoId == TodoId
                         select new UploadFileDto
                         {
                             Name = a.Name,
                             Src = a.Src,
                             TodoId = a.TodoId,
                             UploadFileId = a.UploadFileId
                         };

            //var result = _todoContext.UploadFile
            //    .Where(a => a.TodoId == TodoId)
            //    .Select(a => new UploadFileDto
            //    {
            //        Name = a.Name,
            //        Src = a.Src,
            //        TodoId = a.TodoId,
            //        UploadFileId = a.UploadFileId
            //    });

            if (result == null || result.Count() == 0)
            {
                return NotFound("找不到檔案");
            }

            return Ok(result);
        }

        // GET api/<TodoUploadFileController>/5
        [HttpGet("{UploadFileId}")]
        public ActionResult<UploadFileDto> Get(Guid TodoId, Guid UploadFileId)
        {
            if (!_todoContext.TodoList.Any(a => a.TodoId == TodoId))
            {
                return NotFound("找不到該事項");
            }

            var result = (from a in _todoContext.UploadFile
                         where a.TodoId == TodoId
                         && a.UploadFileId == UploadFileId
                         select new UploadFileDto
                         {
                             Name = a.Name,
                             Src = a.Src,
                             TodoId = a.TodoId,
                             UploadFileId = a.UploadFileId
                         }).SingleOrDefault();

            if (result == null)
            {
                return NotFound("找不到檔案");
            }

            return result;
        }

        // POST api/<TodoUploadFileController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TodoUploadFileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoUploadFileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
