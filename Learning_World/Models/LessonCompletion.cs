using System.ComponentModel.DataAnnotations.Schema;

namespace Learning_World.Models
{
	public class LessonCompletion
	{
		[ForeignKey(nameof(User))]
        public int UserId { get; set; }

		[ForeignKey(nameof(Module))]
		public int ModuleId { get; set; }

		[ForeignKey(nameof(Lesson))]
        public int LessonID { get; set; }
        public DateTime CompletionDate { get; set; }

        public User User { get; set; }
		public Lesson Lesson { get; set; }
		public Module Module { get; set; }	


    }
}
