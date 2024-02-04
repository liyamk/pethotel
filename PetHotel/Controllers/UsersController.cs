using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHotel.IServices;
using PetHotel.Models;
using PetHotel.Models.Dto;

namespace PetHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepo;
        private readonly IMapper _mapper;
        public UsersController(IRepository<User> userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersAsync()
        {
            IEnumerable<User> users = await _userRepo.GetAllAsync();
            return Ok(_mapper.Map<UserDto>(users));
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<ActionResult> CreateUserAsync([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var existing = await _userRepo.GetAsync(x => x.UserName == user.UserName);
            if (existing != null) 
            {
                return new ConflictObjectResult("Existing user");
            }

            await _userRepo.CreateAsync(user);
            return Ok();
        }
    }
}
