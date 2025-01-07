using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

         [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
           Console.WriteLine($"--> Getting commands for platforms {platformId}");
           
           if(!_repo.PlatformExist(platformId))
           {
              return NotFound();
           }
           var commands = _repo.GetCommandForPlatform(platformId);
           return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }
        
         [HttpGet("{commandId}", Name ="GetCommandForPlatform")]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
           Console.WriteLine($"--> Hit GetCommandForPlatform {platformId} / {commandId}");
           
           if(!_repo.PlatformExist(platformId))
           {
              return NotFound();
           }
           var commands = _repo.GetCommand(platformId, commandId);
           if(commands == null) 
           {
              return NotFound();
           }
           return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

         [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
           Console.WriteLine($"--> Hit CreateCommandForPlatform {platformId}");
           
           if(!_repo.PlatformExist(platformId))
           {
              return NotFound();
           }
            var commands = _mapper.Map<Command>(commandCreateDto);

            _repo.CreateCommand(platformId, commands);

            _repo.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(commands);

           return CreatedAtRoute(nameof(GetCommandForPlatform), new {platformId =platformId,
                commandId = commandReadDto.Id}, commandReadDto);
        }

    }
}