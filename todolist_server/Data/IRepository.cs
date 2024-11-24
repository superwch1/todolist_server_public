using todolist_server.Models;

namespace todolist_server.Data
{
    public interface IRepository
    {
        public Task<List<TaskInfo>> ReadTasks(string userId, int year, int month);
        public Task<List<TaskInfo>> ReadTasksKeyword(string userId, string keyword);
        public Task<TaskInfo> CreateTask(TaskInfo task);
        public Task<TaskInfo?> UpdateTask(TaskInfo task);
        public Task<string> DeleteTask(int taskId, string userId);
        public Task<string> DeleteAllTask(string userId);
    }
}
