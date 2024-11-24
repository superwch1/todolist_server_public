
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using todolist_server.Models;

namespace todolist_server.Data
{
    public class Repository : IRepository
    {
        private ToDoListDbContext _context { get; set; }
        public Repository(ToDoListDbContext context)
        {
            _context = context;
        }


        public async Task<TaskInfo> CreateTask(TaskInfo task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.Logs.AddAsync(new LogInfo() { UserId = task.UserId, Instruction = $"{task.UserId} has added a task on {DateTime.Today.ToString("dd-MM-yyyy HH:mm")}" });
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<string> DeleteAllTask(string userId)
        {
            var tasks = _context.Tasks
                .Where(x => x.UserId == userId)
                .ToList();     

            if (tasks != null)
            {
                _context.Tasks.RemoveRange(tasks);
                await _context.SaveChangesAsync();
                return "Done";
            }
            return "Fail";
        }

        public async Task<string> DeleteTask(int taskId, string userId)
        {
            TaskInfo? selectedTask = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);

            if (selectedTask != null)
            {
                if (selectedTask.UserId == userId)
                {
                    _context.Tasks.Remove(selectedTask);
                    await _context.Logs.AddAsync(new LogInfo() { UserId = userId, Instruction = $"{userId} has deleted a task on {DateTime.Today.ToString("dd-MM-yyyy HH:mm")}" });
                    await _context.SaveChangesAsync();
                    return "Done";
                }
            }
            return "Fail";
        }

        public async Task<List<TaskInfo>> ReadTasks(string userId, int year, int month)
        {
            var tasks = await _context.Tasks
                .Where(x => x.UserId == userId && x.DueDate.Month == month && x.DueDate.Year == year)
                .ToListAsync();

            return tasks;
        }

        public async Task<List<TaskInfo>> ReadTasksKeyword(string userId, string keyword)
        {
            var tasks = await _context.Tasks
                .Where(x => x.UserId == userId && (x.Topic.Contains(keyword) || (x.Content != null && x.Content.Contains(keyword))))
                .ToListAsync();

            return tasks;
        }

        public async Task<TaskInfo?> UpdateTask(TaskInfo task)
        {
            TaskInfo? selectedTask = await _context.Tasks
                .FirstOrDefaultAsync(x => x.Id == task.Id && x.UserId == task.UserId);

            if (selectedTask != null)
            {
                selectedTask.Id = task.Id;
                selectedTask.Topic = task.Topic;
                selectedTask.Content = task.Content;
                selectedTask.IntType = task.IntType;
                selectedTask.IntSymbol = task.IntSymbol;
                selectedTask.CreatedDate = task.CreatedDate;
                selectedTask.DueDate = task.DueDate;
                selectedTask.UserId = task.UserId;

                await _context.Logs.AddAsync(new LogInfo() { UserId = task.UserId, Instruction = $"Someone has modified a task on {DateTime.Today.ToString("dd-MM-yyyy HH:mm")}" });
                await _context.SaveChangesAsync();

                return selectedTask;
            }
            else
            {
                var newTask = await CreateTask(new TaskInfo()
                {
                    IntType = task.IntType,
                    Topic = task.Topic,
                    Content = task.Content,
                    DueDate = task.DueDate,
                    IntSymbol = task.IntSymbol,
                    CreatedDate = DateTime.Now,
                    UserId = task.UserId
                });
                return newTask;
            }
        }
    }
}
