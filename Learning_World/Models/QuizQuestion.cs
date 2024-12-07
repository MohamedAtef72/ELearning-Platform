using System;
using System.Collections.Generic;

namespace Learning_World.Models;

public partial class QuizQuestion
{
    public int QuestionId { get; set; }

    public int? LessonId { get; set; }

    public string QuestionText { get; set; } = null!;

    public virtual LessonQuiz? Lesson { get; set; }

    public virtual ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
}
