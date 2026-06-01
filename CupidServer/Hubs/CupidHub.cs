using CupidServer.Data;
using Microsoft.AspNetCore.SignalR;

namespace CupidServer.Hubs
{
    public class CupidHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");

            var person = InMemoryStore.Persons.FirstOrDefault(p =>
                p.ConnectionId == Context.ConnectionId);

            if (person != null)
            {
                InMemoryStore.Persons.Remove(person);

                Console.WriteLine($"{person.Username} removed from active users.");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}