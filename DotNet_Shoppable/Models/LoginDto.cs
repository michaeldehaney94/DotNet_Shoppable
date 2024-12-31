﻿using System.ComponentModel.DataAnnotations;

namespace DotNet_Shoppable.Models
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; } // use to remember user login session data
    }
}

