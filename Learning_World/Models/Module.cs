using System;
using System.Collections.Generic;

namespace Learning_World.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public int? CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int OrderInCourse { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}
