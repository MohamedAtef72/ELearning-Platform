using Learning_World.Models;

namespace Learning_World.ViewModels
{
    public class MyLearningCoursesViewModel
    {
        public int UserId { get; set; }
        public Course Course { get; set; }
        public decimal CompletionPercentage { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
