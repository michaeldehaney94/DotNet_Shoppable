using System.ComponentModel.DataAnnotations;

namespace DotNet_Shoppable.Models
{
    public class PasswordDto
    {
        [Required(ErrorMessage = "The current password field is required"), MaxLength(100)]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "The new password field is required"), MaxLength(100)]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "The confirmed password field is required")]
        [Compare("NewPassword", ErrorMessage = "Confirmed password and/or password does not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}


// This model is used to update the user's profile password