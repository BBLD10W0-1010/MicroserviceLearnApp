using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/k/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDTO>> GetCommandsForPlatform(int platformID)
        {
            Console.WriteLine($" -->  HIT GetCommandsForPlatform : {platformID}");
            if (_repository.PlatformExists(platformID))
            {

            }

            var commandItems = _repository.GetCommandsForPlatform(platformID);
            return Ok(_mapper.Map<IEnumerable<CommandReadDTO>>(commandItems));
        }


    }
}