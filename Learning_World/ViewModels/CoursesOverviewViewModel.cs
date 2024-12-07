using Learning_World.Extentions;
using Learning_World.Models;

namespace Learning_World.ViewModels
{
    public class CoursesOverviewViewModel
    {
        public string Search { get; set; }
        public string Sort { get; set; }
        public List<string> Levels { get; set; }
        public List<string> Prices { get; set; }
        public List<string> Rates { get; set; }
        public PaginatedList<Course> Page { get; set; }
    }
}
