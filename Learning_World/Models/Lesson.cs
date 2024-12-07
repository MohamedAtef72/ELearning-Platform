using System;
using System.Collections.Generic;

namespace Learning_World.Models;

public partial class Lesson
{
    public int LessonId { get; set; }

    public int? PartId { get; set; }

    public int? LessonTypeId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int OrderInPart { get; set; }

    public virtual LessonQuiz? LessonQuiz { get; set; }

    public virtual LessonText? LessonText { get; set; }

    public virtual LessonType? LessonType { get; set; }

    public virtual LessonVideo? LessonVideo { get; set; }

    public virtual Part? Part { get; set; }
}
