using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepo repository, IMapper mapper)
        {
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
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("--> creating a platform...");
             
             var plateformModel = _mapper.Map<Platform>(platformCreateDto);

              _repository.CreatePlatform(plateformModel);
              _repository.SaveChanges();

           
               var platformReadDto = _mapper.Map<PlatformReadDto>(plateformModel);

               return CreatedAtRoute(nameof(GetPlatformById), 
               new {Id = platformReadDto.Id}, platformReadDto);
             
        }
    }
}