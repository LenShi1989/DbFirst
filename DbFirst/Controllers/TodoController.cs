using AutoMapper;
using DbFirst.Dtos;
using DbFirst.Models;
using DbFirst.Parameters;
using DbFirst.Profiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;



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
        public ActionResult<IEnumerable<TodoListDto>> GetTodoList([FromQuery] TodoSelectParameter value)
        {
            var result = _todoContext.TodoList
                .Include(a => a.InsertEmployee)
                .Include(a => a.UpdateEmployee)
                .Include(a => a.UploadFile)
                .Select(a => a); // 这里可以省略，因为默认就是这样

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

            var dtoResult = result.ToList().Select(a => ItemToDto(a)).ToList();

            if (dtoResult == null || dtoResult.Count <= 0)
            {
                return NotFound("找不到資源");
            }

            return Ok(dtoResult);
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

        [HttpGet("GetSQL")]
        public IEnumerable<TodoList> GetSQL(string name)
        {
            string sql = "select * from todolist where 1=1";

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql = sql + " and name like N'%" + name + "%'";
            }

            var result = _todoContext.TodoList.FromSqlRaw(sql);

            return result;
        }


        [HttpGet("GetSQLDto")]
        public IEnumerable<TodoListDto> GetSQLDto(string name)
        {
            string sql = @"SELECT [TodoId]
      ,a.[Name]
      ,[InsertTime]
      ,[UpdateTime]
      ,[Enable]
      ,[Orders]
      ,b.Name as InsertEmployeeName
      ,c.Name as UpdateEmployeeName
  FROM [TodoList] a
  join Employee b on a.InsertEmployeeId=b.EmployeeId
  join Employee c on a.UpdateEmployeeId=c.EmployeeId where 1=1";

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql = sql + " and name like N'%" + name + "%'";
            }

            var result = _todoContext.ExecSQL<TodoListDto>(sql);

            return result;
        }


        [HttpGet("GetSQLDto2")]
        public IEnumerable<TodoListDto> GetSQLDto2(string name)
        {
            using (var command = _todoContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
            SELECT 
                [TodoId],
                a.[Name],
                [InsertTime],
                [UpdateTime],
                [Enable],
                [Orders],
                b.Name as InsertEmployeeName,
                c.Name as UpdateEmployeeName
            FROM 
                [TodoList] a
            JOIN 
                Employee b ON a.InsertEmployeeId = b.EmployeeId
            JOIN 
                Employee c ON a.UpdateEmployeeId = c.EmployeeId
            WHERE 
                1 = 1";

                if (!string.IsNullOrWhiteSpace(name))
                {
                    command.CommandText += " AND a.Name LIKE @Name";
                    command.Parameters.Add(new SqlParameter("@Name", $"%{name}%"));
                }

                _todoContext.Database.OpenConnection();
                var result = command.ExecuteReader()
                    .Cast<DbDataRecord>()
                    .Select(record => new TodoListDto
                    {
                        TodoId = (Guid)record["TodoId"], // 修改这里为 Guid 类型
                        Name = record["Name"] as string,
                        InsertTime = (DateTime)record["InsertTime"],
                        UpdateTime = (DateTime)record["UpdateTime"],
                        Enable = (bool)record["Enable"],
                        Orders = (int)record["Orders"],
                        InsertEmployeeName = record["InsertEmployeeName"] as string,
                        UpdateEmployeeName = record["UpdateEmployeeName"] as string
                    }).ToList();

                return result;
            }
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
        public void Post([FromBody] TodoList value)
        {
            _todoContext.Add(value);
            _todoContext.SaveChanges();
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
