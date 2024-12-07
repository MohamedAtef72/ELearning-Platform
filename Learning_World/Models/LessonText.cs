using System;
using System.Collections.Generic;

namespace Learning_World.Models;

public partial class LessonText
{
    public int LessonId { get; set; }

    public string Content { get; set; } = null!;

    public virtual Lesson Lesson { get; set; } = null!;
}
