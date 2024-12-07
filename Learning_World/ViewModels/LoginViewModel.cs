using System.ComponentModel.DataAnnotations;

namespace Learning_World.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public String Name { get; set; }
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = null!;
        [Display(Name = "Remeber Me")]
        public bool Remember { get; set; }
    }
}
