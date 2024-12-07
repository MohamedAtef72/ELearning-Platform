using Learning_World.Data;
using Learning_World.Models;
using Learning_World.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Learning_World.Repositories
{
    public class MyLearningRepository
    {
        private readonly ElearningPlatformContext _context;

        public MyLearningRepository(ElearningPlatformContext context)
        {
            _context = context;
        }

        public Dictionary<int?, int> FilterCourses(int userId)
        {
            var userCourses = _context.Enrollments.Include(e => e.Course)
                .Where(e => e.UserId == userId).ToList();
            Dictionary<int?, int> keyValuePairs = new();
            foreach (var courseData in userCourses)
            {
                var modules = _context.Modules.Where(e => e.CourseId == courseData.Course.CourseId).ToList();
                int lessons = 0;
                int completedLessons = 0;
                foreach (var module in modules)
                {
                    lessons += _context.Lessons.Include(e => e.Part).Where(e => e.Part.ModuleId == module.ModuleId).Count();
                    completedLessons += _context.LessonCompletions.Where(e => e.ModuleId == module.ModuleId && e.UserId == userId).Count();
                }
                int progress = lessons > 0 ? (int)((double)completedLessons / lessons * 100) : 0;
                keyValuePairs.Add(courseData.CourseId, progress);
            }
            return keyValuePairs;
        }

        public List<MyLearningCoursesViewModel> GetMyLearningCourses(Dictionary<int?, int> filteredCourses, int userId)
        {
            var courses = filteredCourses
                .Select(c => new
                {
                    CourseName = c.Key,
                    CompletionPercentage = c.Value,
                    IsCompleted = c.Value == 100
                }).ToList();


            var myLearningCoursesViewModels = courses.Select(course => new MyLearningCoursesViewModel
            {
                UserId = userId,  // Assign UserId from your controller context
                Course = _context.Courses.FirstOrDefault(e => e.CourseId == course.CourseName),
                CompletionPercentage = course.CompletionPercentage,
                CompletionDate = (from lc in _context.LessonCompletions
                                  join module in _context.Modules on lc.ModuleId equals module.ModuleId
                                  where module.CourseId == course.CourseName && lc.UserId == userId
                                  orderby lc.CompletionDate descending
                                  select lc.CompletionDate).FirstOrDefault(), // Get 
                IsCompleted = course.IsCompleted
            }).ToList();
            return myLearningCoursesViewModels;
        }
    }
}
