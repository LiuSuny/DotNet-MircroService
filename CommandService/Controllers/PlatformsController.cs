using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatfromReadDto>> GetPlatform()
        {
           Console.WriteLine("--> Getting platforms");

           var platformItems = _repo.GetAllPlatform();
           return Ok(_mapper.Map<IEnumerable<PlatfromReadDto>>(platformItems));
        }

        [HttpPost]
        public ActionResult TestInBoundConnection()
        {
            Console.WriteLine("--> InBound POST # Command service");

            return Ok("InBound test connection from platforms controller");
        }
    }
}