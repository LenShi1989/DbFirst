using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DbFirst.Models;

public partial class TodoList
{
    public Guid todoId { get; set; }

    public string name { get; set; }

    public DateTime? insertTime { get; set; }

    public DateTime? updateTime { get; set; }

    public bool? enable { get; set; }

    public int? orders { get; set; }

    public string insertEmployeeName { get; set; }

    public string updateEmployeeName { get; set; }

    public string uploadFiles { get; set; }
}
