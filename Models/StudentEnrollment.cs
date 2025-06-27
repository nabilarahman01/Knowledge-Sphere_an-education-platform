using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knowledgeSphere.Models
{
    public class EnrollStudents
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student Mail is required.")]
        public string StudentEmail{ get; set; }

        public string? TeacherMail { get; set; }

        [Required(ErrorMessage = "Please select a course.")]
        //[ForeignKey("Id")]
        public int CourseId { get; set; }

        //[ForeignKey("StudentEmail")]
        //public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
        [NotMapped]
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
    }
}
