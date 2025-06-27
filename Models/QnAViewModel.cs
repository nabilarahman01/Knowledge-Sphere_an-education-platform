using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knowledgeSphere.Models
{
    public class QnAViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Author { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
    }

    public class AnswerViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string? Author { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public QnAViewModel? Question { get; set; }
    }
}
