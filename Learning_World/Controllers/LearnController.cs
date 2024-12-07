using System.Security.Claims;
using Learning_World.Data;
using Learning_World.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Learning_World.Controllers
{
    [Authorize]
    public class LearnController : Controller
    {
        LearnRepository _learnRepository;

        public LearnController(ElearningPlatformContext _DbContext, LearnRepository repo)
        {
            _learnRepository = repo;
        }

        [CourseEnrollmentAuthorization]
        public IActionResult Index(int id, int moduleId = 1)
        {
            var course = _learnRepository.GetCourse(id);
            ViewBag.CourseTilte = course.Title;
            var modules = _learnRepository.GetModulesByCourseId(id).ToList();
            ViewBag.Modules = modules;
            ViewBag.SelectedModuleId = moduleId;
            Dictionary<int, List<int>> keyValuePairs = new Dictionary<int, List<int>>();
            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            foreach (var module in modules)
            {
                keyValuePairs.Add(module.ModuleId,
                    new List<int>() { _learnRepository.GetLessonsByModuleId(module.ModuleId).ToList().Count(),
                   _learnRepository.GetCompletedLessonsByModuleId(module.ModuleId, userId).ToList().Count() });
            }
            ViewBag.KeyValuePairs = keyValuePairs;
            return View(modules);
        }

     
        public IActionResult PartsPartialView(int id)
        {
            var partsWithLessons = _learnRepository.GetPartByModuleId(id).ToList();
            return PartialView(partsWithLessons);

        }
        [LessonsAuthorizationFilter]
        [Route("Learn/lesson/{moduleId?}/{lessonType}/{lessonId}")]

        public IActionResult Lessons(int moduleId, int lessonId)
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var partsWithLessons = _learnRepository.GetPartByModuleId(moduleId).ToList();
            int cid = _learnRepository.GetCourseIdByModuleId(moduleId);
            ViewBag.C_Id = cid;
            ViewBag.isCompleted = _learnRepository.GetCompletedLessonsByModuleId(moduleId, userId).ToList();
            ViewBag.ModuleName = partsWithLessons[0].Module.Title;
            return View(partsWithLessons);
        }


        [Route("Learn/LessonDisplayPartialView/{moduleId?}/{lessonType}/{lessonId}")]
        public IActionResult LessonDisplayPartialView(int lessonId, string lessonType, int moduleId)
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            ViewBag.isCompleted = _learnRepository.GetCompletedLessonsByModuleId(moduleId, userId).ToList();
            ViewBag.cousreId = _learnRepository.GetCourseIdByModuleId(moduleId);
            switch (lessonType)
            {
                case "Video":
                    var lesson1 = _learnRepository.GetLessonVideoByLessonId(lessonId);
                    return PartialView("LessonVideoDisplay", lesson1);

                case "Text":
                    var lesson2 = _learnRepository.GetLessonTextByLessonId(lessonId);
                    return PartialView("LessonTextDisplay", lesson2);

                case "Quiz":
                    return RedirectToAction("GetQuiz", new { lessonId = lessonId });

                default:
                    return View("NotFound404");
            }

        }

        [HttpPost]
        [Route("Learn/CompleteLesson/{ModuleId}/{LessonId}")]
        public void CompleteLesson(int lessonId, int ModuleId)
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            _learnRepository.AddLessonCompletetions(lessonId, ModuleId, userId);
            _learnRepository.Save();
        }


        [HttpGet]
        public IActionResult GetQuiz(int lessonId)
        {
            var lesson = _learnRepository.GetLessonQuizByLessonId(lessonId);
            return PartialView("LessonQuizDisplay", lesson);
        }

        [HttpPost]
        public IActionResult QuizResult(IFormCollection form, int lessonId)
        {
            var quiz = _learnRepository.GetLessonQuiz(lessonId);

            if (quiz == null)
            {
                return View("NotFound404");
            }

            int correctAnswers = 0;
            var results = new List<object>();

            foreach (var question in quiz.QuizQuestions)
            {
                var selectedAnswerIdStr = form[$"answer_{question.QuestionId}"];
                if (int.TryParse(selectedAnswerIdStr, out int selectedAnswerId))
                {
                    var correctAnswer = question.QuizAnswers.FirstOrDefault(a => a.IsCorrect);

                    bool isCorrect = correctAnswer != null && selectedAnswerId == correctAnswer.AnswerId;
                    if (isCorrect) correctAnswers++;

                    results.Add(new
                    {
                        questionId = question.QuestionId,
                        selectedAnswerId = selectedAnswerId,
                        correctAnswerId = correctAnswer.AnswerId,
                        isCorrect = isCorrect
                    });
                }
            }

            double percentage = (double)correctAnswers / quiz.QuizQuestions.Count * 100;
            bool passed = percentage >= quiz.PassingScore;
            if (passed)
            {
                CompleteLesson(lessonId, _learnRepository.GetModuleIdByLessonId(lessonId));
            }

            // Return the percentage as part of the response
            return Json(new { passed = passed, percentage = percentage, results = results, lessonId = lessonId });
        }
    }
}
