using Learning_World.Data;
using Learning_World.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Learning_World.Repositories
{
    public class CoursesRepository
    {
        private readonly ElearningPlatformContext _context;

        public CoursesRepository(ElearningPlatformContext context)
        {
            _context = context;
        }
        public IQueryable<Course> GetAllCourses()
        {
            return _context.Courses;
        }

        public Course GetById(int id)
        {
            return _context.Courses.Find(id);
        }

        public List<Course> GetMostPopularCourses(int n)
        {
            return _context.Courses.OrderByDescending(c => c.MaxEnrollment).Take(n).ToList();
        }
        public List<Module> GetCourseModules(int courseId)
        {
            return _context.Modules.Where(e => e.CourseId == courseId).ToList();
        }
        public List<Part> GetCourseParts()
        {
            return _context.Parts.ToList();
        }
        public List<Lesson> GetCourseLessons()
        {
            return _context.Lessons.ToList();
        }
    }
}
