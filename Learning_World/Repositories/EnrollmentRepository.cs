using Learning_World.Data;
using Learning_World.Models;

namespace Learning_World.Repositories
{
    public class EnrollmentRepository
    {
        private readonly ElearningPlatformContext _context;

        public EnrollmentRepository(ElearningPlatformContext context)
        {
            _context = context;
        }

        public bool IsEnrolled(int courseId, int userId)
        {
            return _context.Enrollments.FirstOrDefault(e => e.CourseId == courseId && e.UserId == userId) == null ? false : true;
        }

        public void AddEnroll(Enrollment enrollment, Payment payment)
        {
            _context.Payments.Add(payment);
            _context.Enrollments.Add(enrollment);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
