using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using todolist_server.Models;

namespace todolist_server.Data
{
    public class ToDoListDbContext : IdentityDbContext
    {
        public DbSet<TaskInfo> Tasks { get; set; }
        public DbSet<LogInfo> Logs { get; set; }

        public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options) : base(options) { }
    }
}
