using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace todolist_server.Models
{
    public class LogInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; } = string.Empty;
        public virtual IdentityUser? User { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Instruction { get; set; } = string.Empty;
    }
}
