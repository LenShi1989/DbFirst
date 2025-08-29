using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DbFirst.Models;
using DbFirst.Dots;
using DbFirst.Parameters;
using System.Linq;
using System;
using AutoMapper;
using DbFirst.Profiles;



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
        public IEnumerable<TodoListSelectDto> GetTodoList([FromQuery] TodoSelectParameter value)
        {
            var result = _todoContext.TodoList
                .AsEnumerable()
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


        // GET: api/Todo/AutoMapper
        [HttpGet("AutoMapper")]
        public IEnumerable<TodoListSelectDto> GetTodoList2([FromQuery] TodoSelectParameter value)
        {
            var result = _todoContext.TodoList
                //.AsEnumerable()
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

            var map = _mapper.Map< IEnumerable<TodoList>, IEnumerable<TodoListSelectDto>>(result);

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

        private static TodoListSelectDto ItemToDto(TodoList a)
        {
            return new TodoListSelectDto
            {
                Enable = a.enable,
                InsertEmployeeName = a.insertEmployeeName + "(" + a.todoId + ")",
                InsertTime = a.insertTime,
                Name = a.Name,
                Orders = a.orders,
                TodoId = a.todoId,
                UpdateEmployeeName = a.updateEmployeeName + "(" + a.todoId + ")",
                UpdateTime = a.updateTime
            };
        }
    }
}
