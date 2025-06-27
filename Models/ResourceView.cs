using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knowledgeSphere.Models
{
    public class ResourceView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Resource Name is required")]
        public string ResourceName { get; set; }

        [Required(ErrorMessage = "Topic/Description is required")]
        public string TopicDescription { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; set; }

        public byte[]? FileData { get; set; }
        public string? FileName { get; set; }
    }

}
