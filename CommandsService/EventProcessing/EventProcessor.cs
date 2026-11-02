using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    //todo
                    break;
                default:
                    //todo
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            System.Console.WriteLine("--> Determining Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDTO>(notificationMessage);

            switch (eventType.Event)
            {
                case "PlatformPublished":
                    System.Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    System.Console.WriteLine("--> Couldn`t determine event type");
                    return EventType.Undetermined;
            }
        }
        private void AddPlatform(string platformPublishedMessage)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);
            try
            {
                var plat = _mapper.Map<Platform>(platformPublishedDTO);
                if (!repo.ExternalPlatformExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                }
                else
                {
                    System.Console.WriteLine($"--> Platform already exists... ");
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine($"--> could not add Platform to DB {ex.Message}");
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}