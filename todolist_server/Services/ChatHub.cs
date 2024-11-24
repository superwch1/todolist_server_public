using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using System.Security.Claims;
using todolist_server.Data;
using todolist_server.Models;

namespace todolist_server.Services
{
    [Authorize(AuthenticationSchemes = AuthSchemes)]
    public class ChatHub : Hub
    {
        private const string AuthSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
        private IRepository _repository { get; set; }

        public ChatHub(IRepository repository)
        {
            _repository = repository;
        }


        public async Task<string> CreateTask(int intType, string topic, string content, string dueDate, int intSymbol)
        {
            var userId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var task = await _repository.CreateTask(new TaskInfo()
                {
                    IntType = intType,
                    Topic = topic,
                    Content = content,
                    DueDate = DateTime.ParseExact(dueDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                    IntSymbol = intSymbol,
                    CreatedDate = DateTime.Now,
                    UserId = userId
                });

                await Clients.User(userId).SendAsync("DeleteThenCreateTask", task.Id, task.IntType, task.Topic, task.Content, task.DueDate.ToString("yyyy-MM-dd"), task.IntSymbol);
                return "Done";
            }
            return "Fail";
        }


        //Create new task if the id of UpdateTask is not found (deleted by other user)
        public async Task<string> UpdateTask(int id, int intType, string topic, string content, string dueDate, int intSymbol)
        {
            var userId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var task = await _repository.UpdateTask(new TaskInfo()
                {
                    Id = id,
                    IntType = intType,
                    Topic = topic,
                    Content = content,
                    DueDate = DateTime.ParseExact(dueDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                    IntSymbol = intSymbol,
                    CreatedDate = DateTime.Now,
                    UserId = userId
                });

                if (task != null)
                {
                    await Clients.User(userId).SendAsync("DeleteThenCreateTask", task.Id, task.IntType, task.Topic, task.Content, task.DueDate.ToString("yyyy-MM-dd"), task.IntSymbol);
                }
                return "Done";
            }
            return "Fail";
        }


        public async Task<string> DeleteTask(int id)
        {
            var userId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var response = await _repository.DeleteTask(id, userId);
                if (response == "Done")
                {
                    await Clients.User(userId).SendAsync("DeleteTask", id);
                    return "Done";
                }
            }
            return "Fail";
        }
    }
}
