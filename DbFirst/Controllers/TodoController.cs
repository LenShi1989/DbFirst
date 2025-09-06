using AutoMapper;
using DbFirst.Dots;
using DbFirst.Models;
using DbFirst.Parameters;
using DbFirst.Profiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;




// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DbFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly IMapper _mapper;

        public TodoController(TodoContext todoContext, IMapper mapper)
        {
            _todoContext = todoContext;
            _mapper = mapper;
        }


        //// GET api/Todo/From/1
        //[HttpGet("From/{id}")]
        //public dynamic GetFrom([FromRoute] string id,
        //    [FromQuery] string id2,
        //    [FromBody] string id3,
        //    [FromForm] string id4)
        //{
        //    List<dynamic> result = new List<dynamic>();

        //    result.Add(id);
        //    result.Add(id2);
        //    result.Add(id3);
        //    result.Add(id4);

        //    return result;
        //}


        // GET api/Todo/Len
        [HttpGet("Len")]
        public IEnumerable<TodoListDto> GetTodoList([FromQuery] TodoSelectParameter value)
        {
            var result = _todoContext.TodoList
                .Include(a => a.InsertEmployee)
                .Include(a => a.UpdateEmployee)
                .Include(a => a.UploadFile)
                //.AsEnumerable()
                .Select(a => ItemToDto(a));

            //.Select(a => new TodoListSelectDto
            //{
            //    Enable = a.enable,
            //    InsertEmployeeName = a.insertEmployeeName,
            //    InsertTime = a.insertTime,
            //    Name = a.name,
            //    Orders = a.orders,
            //    TodoId = a.todoId,
            //    UpdateEmployeeName = a.updateEmployeeName,
            //    UpdateTime = a.updateTime
            //});

            if (!string.IsNullOrWhiteSpace(value.name))
            {
                result = result.Where(a => a.Name.Contains(value.name));
            }

            if (value.enable != null)
            {
                result = result.Where(a => a.Enable == value.enable);
            }

            if (value.InsertTime != null)
            {
                result = result.Where(a => a.InsertTime == value.InsertTime);
            }

            if (value.minOrder != null && value.maxOrder != null)
            {
                result = result.Where(a => a.Orders >= value.minOrder && a.Orders <= value.maxOrder);
            }

            return result;
        }


        // GET api/Todo/1f3012b6-71ae-4e74-88fd-018ed53ed2d3
        [HttpGet("Len/{id}")]
        public TodoListDto Get(Guid id)
        {
            var result = (from a in _todoContext.TodoList
                          where a.TodoId == id
                          select new TodoListDto
                          {
                              Enable = a.Enable,
                              InsertEmployeeName = a.InsertEmployee.Name,
                              InsertTime = a.InsertTime,
                              Name = a.Name,
                              Orders = a.Orders,
                              TodoId = a.TodoId,
                              UpdateEmployeeName = a.UpdateEmployee.Name,
                              UpdateTime = a.UpdateTime,
                              UploadFiles = (from b in _todoContext.UploadFile
                                             where a.TodoId == b.TodoId
                                             select new UploadFileDto
                                             {
                                                 Name = b.Name,
                                                 Src = b.Src,
                                                 TodoId = b.TodoId,
                                                 UploadFileId = b.UploadFileId
                                             }).ToList()
                          }).SingleOrDefault();
            return result;
        }

        // GET: api/Todo/AutoMapper
        [HttpGet("AutoMapper")]
        public IEnumerable<TodoListDto> GetAutoMapper([FromQuery] TodoSelectParameter value)
        {
            var result = _todoContext.TodoList
                //.AsEnumerable()
                .Include(a => a.UpdateEmployee)
                .Include(a => a.InsertEmployee)
                .Include(a => a.UploadFile)
                .Select(a => a);

            //.Select(a => new TodoListSelectDto
            //{
            //    Enable = a.enable,
            //    InsertEmployeeName = a.insertEmployeeName,
            //    InsertTime = a.insertTime,
            //    Name = a.name,
            //    Orders = a.orders,
            //    TodoId = a.todoId,
            //    UpdateEmployeeName = a.updateEmployeeName,
            //    UpdateTime = a.updateTime
            //});

            if (!string.IsNullOrWhiteSpace(value.name))
            {
                result = result.Where(a => a.Name.Contains(value.name));
            }

            if (value.enable != null)
            {
                result = result.Where(a => a.Enable == value.enable);
            }

            if (value.InsertTime != null)
            {
                result = result.Where(a => a.InsertTime == value.InsertTime);
            }

            if (value.minOrder != null && value.maxOrder != null)
            {
                result = result.Where(a => a.Orders >= value.minOrder && a.Orders <= value.maxOrder);
            }

            var map = _mapper.Map<IEnumerable<TodoListDto>>(result);

            return map;
        }


        // GET: api/<TodoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TodoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TodoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private static TodoListDto ItemToDto(TodoList a)
        {
            List<UploadFileDto> updto = new List<UploadFileDto>();

            foreach (var temp in a.UploadFile)
            {
                UploadFileDto up = new UploadFileDto
                {
                    Name = temp.Name,
                    Src = temp.Src,
                    TodoId = temp.TodoId,
                    UploadFileId = temp.UploadFileId
                };
                updto.Add(up);
            }


            return new TodoListDto
            {
                Enable = a.Enable,
                InsertEmployeeName = a.InsertEmployee.Name,
                InsertTime = a.InsertTime,
                Name = a.Name,
                Orders = a.Orders,
                TodoId = a.TodoId,
                UpdateEmployeeName = a.UpdateEmployee.Name,
                UpdateTime = a.UpdateTime,
                UploadFiles = updto
            };
        }
    }
}
