using PlatformService.Dtos;

namespace PlatformService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        public Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto);

        public Task ConnectAsync();
    }
}