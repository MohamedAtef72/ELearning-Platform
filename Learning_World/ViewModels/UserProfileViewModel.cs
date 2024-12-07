using Learning_World.Models;
using System.ComponentModel.DataAnnotations;

namespace Learning_World.ViewModels
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }
        public string? Image {  get; set; }
        public IFormFile? ImageFile { get; set; } // File upload for image
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")] public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<CertificateViewModel> Certificates { get; set; } = new List<CertificateViewModel>();
    }
    public class CertificateViewModel
    {
        public string CourseName { get; set; } = null!;
        public DateTime IssueDate { get; set; }
    }
}
