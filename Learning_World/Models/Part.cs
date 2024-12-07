using System;
using System.Collections.Generic;

namespace Learning_World.Models;

public partial class Part
{
    public int PartId { get; set; }

    public int? ModuleId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int OrderInModule { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual Module? Module { get; set; }
}
