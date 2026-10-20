using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace labo.signalr.api.Hubs
{
    public class UselessTaskHub : Hub
    {
        // static: shared across all connections (a new Hub is created per call)
        private static int _userCount = 0;

        private readonly ApplicationDbContext _context;

        public UselessTaskHub(ApplicationDbContext context)
        {
            _context = context;
        }

        // Connection: send the list to the client who just connected
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            Interlocked.Increment(ref _userCount);
            await Clients.All.SendAsync("UserCount", _userCount);

            await Clients.Caller.SendAsync("TaskList", await _context.UselessTasks.ToListAsync());
        }

        // Add a task: save it, then send the list to everyone
        public async Task AddTask(string taskText)
        {
            _context.UselessTasks.Add(new UselessTask
            {
                Text = taskText,
                Completed = false
            });
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("TaskList", await _context.UselessTasks.ToListAsync());
        }

        // Complete a task: update it, then send the list to everyone
        public async Task CompleteTask(int id)
        {
            UselessTask? task = await _context.UselessTasks.FindAsync(id);
            if (task != null)
            {
                task.Completed = true;
                await _context.SaveChangesAsync();
            }

            await Clients.All.SendAsync("TaskList", await _context.UselessTasks.ToListAsync());
        }

        // Disconnection: decrement and notify everyone
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Interlocked.Decrement(ref _userCount);
            await Clients.All.SendAsync("UserCount", _userCount);

            await base.OnDisconnectedAsync(exception);
        }
    }
}