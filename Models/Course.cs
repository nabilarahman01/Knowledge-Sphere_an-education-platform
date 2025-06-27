using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace knowledgeSphere.Models
{
    public partial class Course
    {
        public int Id { get; set; }
        public string CourseNo { get; set; }
        public string CourseTitle { get; set; }
        public string Description { get; set; }
        public string? Mail { get; set; }

        // Collection of posts related to the course
        public ICollection<CoursePost> Posts { get; set; } = new List<CoursePost>();

        //public ICollection<EnrollStudents> EnrolledStudents { get; set; } = new List<EnrollStudents>();
    }

    public class CoursePost
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key to associate the post with a course
        public int CourseId { get; set; }

        // Navigation property for the associated course
        public Course? Course { get; set; }

        public string? FileName { get; set; }
        public byte[]? FileData { get; set; }
        public string? ContentType { get; set; }
    }
}