using System;
using System.Collections.Generic;

namespace Learning_World.Models;

public partial class LessonQuiz
{
    public int LessonId { get; set; }

    public int PassingScore { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
}
