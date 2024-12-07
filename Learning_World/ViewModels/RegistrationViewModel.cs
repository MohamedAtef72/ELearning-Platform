using System.ComponentModel.DataAnnotations;

namespace Learning_World.ViewModels
{
    public class RegistrationViewModel
    {

        [Required]
        public string? UserName { get; set; }
        [Required]
        [DataType(dataType: DataType.Password)]
        public string? Password { get; set; }
        [Required]
        [Compare("Password")]
        [DataType(dataType: DataType.Password)]
        public string? ConfirmPassword { get; set; }
        [Required]
        [DataType(dataType: DataType.EmailAddress)]
        public string? Email { get; set; }

    }
}
