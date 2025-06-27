using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace knowledgeSphere.Models
{
    public partial class Student
    {
        public int Id { get; set; }
        [Required]
        public int? StudentId { get; set; }
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string Gender { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = null!;

        //public ICollection<EnrollStudents> EnrolledCourses { get; set; } = new List<EnrollStudents>();
    }
}
