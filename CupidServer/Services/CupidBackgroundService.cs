using CupidServer.Data;
using CupidServer.Hubs;
using CupidServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace CupidServer.Services
{
    public class CupidBackgroundService : BackgroundService
    {
        private readonly IHubContext<CupidHub> _hubContext;
        private readonly CupidService _cupidService;

        public CupidBackgroundService(
            IHubContext<CupidHub> hubContext,
            CupidService cupidService)
        {
            _hubContext = hubContext;
            _cupidService = cupidService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                foreach (var receiver in InMemoryStore.Persons.ToList())
                {
                    if (receiver.WaitingForConfirmation)
                        continue;

                    var sender = _cupidService.FindBestMatch(receiver);

                    if (sender == null)
                        continue;

                    var letter = _cupidService.CreateLoveLetter(sender);

                    receiver.WaitingForConfirmation = true;

                    await _hubContext.Clients
                        .Client(receiver.ConnectionId)
                        .SendAsync("ReceiveLetter", letter, stoppingToken);

                    Console.WriteLine($"Letter sent to {receiver.Username} from {sender.Username}");
                }
            }
        }
    }
}