using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace todolist_server.Models
{
    public class TaskInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "The type of task can't be omitted")]
        public int IntType { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        [MaxLength(255, ErrorMessage = "Topic length can't be more than 255 words.")]
        public string Topic { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(4000)")]
        [MaxLength(4000, ErrorMessage = "Content length can't be more than 4000 words.")]
        public string? Content { get; set; }

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, 1, ErrorMessage = "The status of task can't be omitted")]
        public int IntSymbol { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("User")]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; } = string.Empty;
        public virtual IdentityUser? User { get; set; }
    }
}
