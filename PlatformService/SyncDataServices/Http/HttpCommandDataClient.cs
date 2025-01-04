using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _congfiguration;
        public HttpCommandDataClient(HttpClient httpClient, IConfiguration congfiguration)
        {
            _httpClient = httpClient;
            _congfiguration = congfiguration;
        }

      

        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
           var httpContent = new StringContent(
              JsonSerializer.Serialize(plat),
              Encoding.UTF8, "application/json"
           );

           var reponse = await _httpClient.PostAsync($"{_congfiguration["CommandService"]}", httpContent);

           if(reponse.IsSuccessStatusCode) 
           {
            Console.WriteLine("--> sync Post # command service was OK!");
           }
           else {
                Console.WriteLine("--> sync Post # command service failed");
           }
        }
    }
}