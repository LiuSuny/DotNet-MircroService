using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
         private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, 
        ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
           _commandDataClient = commandDataClient;
           _messageBusClient = messageBusClient;
            _repository = repository;
            _mapper = mapper;
        }

       [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatform()
        {
            Console.WriteLine("--> Get all platform...");

             var plateformItem = _repository.GetAllPlatforms();
             return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(plateformItem));
        }

        [HttpGet("{id}", Name ="GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine("--> Getting platform by id...");

             var plateformItem = _repository.GetPlatformsById(id);

             if(plateformItem != null) 
             {
               return Ok(_mapper.Map<PlatformReadDto>(plateformItem));
             }
             
             return NotFound();  
        }

         [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("--> creating a platform...");
             
             var plateformModel = _mapper.Map<Platform>(platformCreateDto);

              _repository.CreatePlatform(plateformModel);
              _repository.SaveChanges();

           
               var platformReadDto = _mapper.Map<PlatformReadDto>(plateformModel);
               
               //making a call to send  sync message our command service
               try
               {
                 await _commandDataClient.SendPlatformToCommand(platformReadDto);

               }
               catch (Exception ex)
               {
                 Console.WriteLine($"--> could not send synchronuously: {ex.Message}");            
               }

                //sending async messages
                try
                {
                  var platformPublsihedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                   platformPublsihedDto.Event = "Platform_Published";
                   _messageBusClient.PublishNewPlatform(platformPublsihedDto);
                }
                catch (Exception ex)
                {
                   Console.WriteLine($"--> could not send asynchronuously: {ex.Message}");            
                }


               return CreatedAtRoute(nameof(GetPlatformById), 
               new {Id = platformReadDto.Id}, platformReadDto);
             
        }
    }
}