using System;
using System.Collections.Generic;

namespace DbFirst.Models;

public partial class JobTitle
{
    public Guid JobTitleId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Employee> Employee { get; set; } = new List<Employee>();
}
