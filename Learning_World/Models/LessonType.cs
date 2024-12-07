using System;
using System.Collections.Generic;

namespace Learning_World.Models;

public partial class LessonType
{
    public int LessonTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
