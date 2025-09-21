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




        // GET api/Todo/Len/1f3012b6-71ae-4e74-88fd-018ed53ed2d3
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



        // GET api/Todo/1f3012b6-71ae-4e74-88fd-018ed53ed2d3
        [HttpGet("{TodoId}")]
        public ActionResult<TodoListDto> GetOne(Guid TodoId)
        {
            var result = (from a in _todoContext.TodoList
                          where a.TodoId == TodoId
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

        // POST api/Todo/Len
        [HttpPost("Len")]
        public IActionResult Post([FromBody] TodoList value)
        {
            TodoList insert = new TodoList
            {
                Name = value.Name,
                Enable = value.Enable,
                Orders = value.Orders,
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UploadFile = value.UploadFile
            };

            _todoContext.TodoList.Add(insert);
            _todoContext.SaveChanges();

            return Ok("已傳送");
        }

        // POST api/Todo/Len2
        [HttpPost("Len2")]
        public IActionResult Len2([FromBody] TodoList value)
        {
            List<UploadFile> upl = new List<UploadFile>();

            foreach (var temp in value.UploadFile)
            {
                UploadFile up = new UploadFile
                {
                    Name = temp.Name,
                    Src = temp.Src
                };
                upl.Add(up);
            }


            TodoList insert = new TodoList
            {
                Name = value.Name,
                Enable = value.Enable,
                Orders = value.Orders,
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UploadFile = value.UploadFile
            };

            _todoContext.TodoList.Add(insert);
            _todoContext.SaveChanges();

            return Ok("已傳送");
        }


        // POST api/Todo/nofk
        [HttpPost("nofk")]
        public IActionResult Postnofk([FromBody] TodoList value)
        {
            TodoList insert = new TodoList
            {
                Name = value.Name,
                Enable = value.Enable,
                Orders = value.Orders,
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };

            _todoContext.TodoList.Add(insert);
            _todoContext.SaveChanges();

            foreach (var temp in value.UploadFile)
            {
                UploadFile insert2 = new UploadFile
                {
                    Name = temp.Name,
                    Src = temp.Src,
                    TodoId = insert.TodoId
                };

                _todoContext.UploadFile.Add(insert2);
                _todoContext.SaveChanges();
            }
            return Ok("已傳送");

        }


        // POST api/Todo/1f3012b6-71ae-4e74-88fd-018ed53ed2d3/UploadFile
        [HttpPost]
        public string Post(Guid TodoId, [FromBody] UploadFile value)
        {
            if (!_todoContext.TodoList.Any(a => a.TodoId == TodoId))
            {
                return "找不到該事項";
            }

            UploadFile insert = new UploadFile
            {
                Name = value.Name,
                Src = value.Src,
                TodoId = TodoId
            };

            _todoContext.UploadFile.Add(insert);
            _todoContext.SaveChanges();

            return "ok";
        }


        // POST api/Todo/AutoMapper
        [HttpPost("AutoMapper")]
        public void PostAutoMapper([FromBody] TodoListPostDto value)
        {
            var map = _mapper.Map<TodoList>(value);

            map.InsertTime = DateTime.Now;
            map.UpdateTime = DateTime.Now;
            map.InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            map.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");


            _todoContext.TodoList.Add(map);
            _todoContext.SaveChanges();

        }



        // POST api/Todo/1f3012b6-71ae-4e74-88fd-018ed53ed2d3/AutoMapper2/UploadFile
        [HttpPost("AutoMapper2")]
        public string PostAutoMapper2(Guid TodoId, [FromBody] UploadFilePostDto value)
        {
            if (!_todoContext.TodoList.Any(a => a.TodoId == TodoId))
            {
                return "找不到該事項";
            }

            var map = _mapper.Map<UploadFile>(value);
            map.TodoId = TodoId;

            _todoContext.UploadFile.Add(map);
            _todoContext.SaveChanges();

            return "ok";
        }

        // POST api/Todo/Len3
        [HttpPost("Len3")]
        public void Len3([FromBody] TodoListPostDto value)
        {
            TodoList insert = new TodoList
            {
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };

            _todoContext.TodoList.Add(insert).CurrentValues.SetValues(value);
            _todoContext.SaveChanges();

            foreach (var temp in value.UploadFile)
            {
                _todoContext.UploadFile.Add(new UploadFile()
                {
                    TodoId = insert.TodoId
                }).CurrentValues.SetValues(temp);
            }

            _todoContext.SaveChanges();

        }



        // POST api/Todo/Len4
        [HttpPost("Len4")]
        public IActionResult Len4([FromBody] TodoListPostDto value)
        {
            TodoList insert = new TodoList
            {
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };

            _todoContext.TodoList.Add(insert).CurrentValues.SetValues(value);
            _todoContext.SaveChanges();

            foreach (var temp in value.UploadFile)
            {
                _todoContext.UploadFile.Add(new UploadFile()
                {
                    TodoId = insert.TodoId
                }).CurrentValues.SetValues(temp);
            }

            _todoContext.SaveChanges();

            return CreatedAtAction(nameof(GetOne), new { TodoId = insert.TodoId }, insert);

        }



        //POST api/Todo/postSQL
        [HttpPost("postSQL")]
        public void PostSQL([FromBody] TodoListPostDto value)
        {
            int enableValue = value.Enable ? 1 : 0;
            string sql = @"INSERT INTO [dbo].[TodoList]
           ([Name]
           ,[InsertTime]
           ,[UpdateTime]
           ,[Enable]
           ,[Orders]
           ,[InsertEmployeeId]
           ,[UpdateEmployeeId])
     VALUES
          (N'" + value.Name + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + enableValue + "," + value.Orders + "," + "'00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001')";
            //(N'" + value.Name + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + value.Enable + "," + value.Orders + "," + "'00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001')";

            _todoContext.Database.ExecuteSqlRaw(sql);

        }


        // PUT api/Todo/:id
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] TodoList value)
        {
            //_todoContext.TodoList.Update(value);
            //_todoContext.SaveChanges(); 


            ///<summary>
            /// 寫法一
            /// </summary>
            var update = _todoContext.TodoList.Find(id);
            update.InsertTime = DateTime.Now;
            update.UpdateTime = DateTime.Now;
            update.InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            update.Name = value.Name;
            update.Orders = value.Orders;
            update.Enable = value.Enable;
            _todoContext.SaveChanges();


            ///<summary>
            /// 寫法二
            /// </summary>
            //var update = (from a in _todoContext.TodoList
            //              where a.TodoId == id
            //              select a).SingleOrDefault();

            //if (update != null)
            //{
            //    update.InsertTime = DateTime.Now;
            //    update.UpdateTime = DateTime.Now;
            //    update.InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            //    update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            //    update.Name = value.Name;
            //    update.Orders = value.Orders;
            //    update.Enable = value.Enable;
            //    _todoContext.SaveChanges();
            //}
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
