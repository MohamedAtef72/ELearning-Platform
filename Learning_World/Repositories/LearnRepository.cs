using Learning_World.Data;
using Learning_World.Models;
using Microsoft.EntityFrameworkCore;

namespace Learning_World.Repositories
{
    public class LearnRepository
    {
        private readonly ElearningPlatformContext _db ;
        public LearnRepository(ElearningPlatformContext context) 
        {
            _db = context;
        }

        public Course GetCourse(int id)
        {
            return _db.Courses.FirstOrDefault(e => e.CourseId == id);
        }

        public IQueryable<Module> GetModulesByCourseId(int id)
        {
            return _db.Modules.Where(e => e.CourseId == id);    
        }


        public IQueryable<Lesson>  GetLessonsByModuleId(int moduleId)
        {
            return _db.Lessons.Include(e => e.Part).Where(e => e.Part.ModuleId == moduleId);
        }

        public IQueryable<LessonCompletion> GetCompletedLessonsByModuleId(int moduleId , int userId)
        {
            return _db.LessonCompletions.Where(e => e.ModuleId == moduleId && e.UserId == userId);
        }


        public IQueryable<Part> GetPartByModuleId(int moduleId)
        {
            return _db.Parts.Include(e => e.Lessons).ThenInclude(e => e.LessonType).Where(e => e.ModuleId == moduleId);
        }

        public int GetCourseIdByModuleId(int moduleId)
        {
            return (int)_db.Modules.Include(e => e.Course).FirstOrDefault(e => e.ModuleId == moduleId).CourseId;
        }

        public Lesson GetLessonVideoByLessonId(int lessonId)
        {
            return _db.Lessons.Include(e => e.LessonVideo).Include(e => e.Part).FirstOrDefault(e => e.LessonId == lessonId);
        }
        public Lesson GetLessonTextByLessonId(int lessonId)
        {
            return _db.Lessons.Include(e => e.LessonText).Include(e => e.Part).FirstOrDefault(e => e.LessonId == lessonId);
        }
        public Lesson GetLessonQuizByLessonId(int lessonId)
        {
            return _db.Lessons.Include(e => e.LessonQuiz).ThenInclude(e => e.QuizQuestions).ThenInclude(e => e.QuizAnswers).FirstOrDefault(l => l.LessonId == lessonId);
        }


        public int GetModuleIdByLessonId(int lessonId)
        {
            return (int)_db.Lessons.Include(e => e.Part).FirstOrDefault(e => e.LessonId == lessonId).Part.ModuleId;
        }
        public void AddLessonCompletetions(int lessonId, int ModuleId, int UserId)
        {
            _db.LessonCompletions.Add(new LessonCompletion() { LessonID = lessonId, UserId = UserId, ModuleId = ModuleId, CompletionDate = DateTime.Now });

        }

        public LessonQuiz GetLessonQuiz(int lessonId)
        {
            return _db.LessonQuizzes
                .Include(q => q.QuizQuestions)
                .ThenInclude(qq => qq.QuizAnswers)
                .FirstOrDefault(q => q.LessonId == lessonId);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
