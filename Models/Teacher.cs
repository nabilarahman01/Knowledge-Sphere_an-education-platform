using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace knowledgeSphere.Models
{
    [Index(nameof(Email),IsUnique = true)]
    public partial class Teacher
    {
        public int TeacherId { get; set; }
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters")]
        public string Password { get; set; } = null!;
    }
}
