using System;
using System.Collections.Generic;

namespace DbFirst.Models;

public partial class Employee
{
    public Guid EmployeeId { get; set; }

    public string Name { get; set; }

    public string Account { get; set; }

    public string Password { get; set; }

    public Guid JobTitleId { get; set; }

    public Guid DivisionId { get; set; }

    public virtual Division Division { get; set; }

    public virtual JobTitle JobTitle { get; set; }

    public virtual ICollection<TodoList> TodoListInsertEmployee { get; set; } = new List<TodoList>();

    public virtual ICollection<TodoList> TodoListUpdateEmployee { get; set; } = new List<TodoList>();
}
