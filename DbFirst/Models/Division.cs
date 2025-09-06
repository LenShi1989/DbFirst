using System;
using System.Collections.Generic;

namespace DbFirst.Models;

public partial class Division
{
    public Guid DivisionId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Employee> Employee { get; set; } = new List<Employee>();
}
