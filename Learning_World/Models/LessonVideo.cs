using System;
using System.Collections.Generic;

namespace Learning_World.Models;

public partial class LessonVideo
{
    public int LessonId { get; set; }

    public string VideoUrl { get; set; } = null!;

    public int Duration { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;
}
