﻿using System.ComponentModel.DataAnnotations;

namespace todolist_server.Models
{
    public class ResetPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password length is between 6 - 20 characters")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetToken { get; set; }
    }
}
