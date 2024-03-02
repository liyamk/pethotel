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
        private readonly IHashService _hashService;
        public UsersController(IRepository<User> userRepo, IMapper mapper, IHashService hashService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _hashService = hashService;
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
        public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] UserCreateDto userCreate)
        {
            if (userCreate == null)
            {
                return BadRequest();
            }

            var existing = await _userRepo.GetAsync(x => x.UserName == userCreate.UserName);
            if (existing != null) 
            {
                return new ConflictObjectResult("Existing user");
            }

            var user = _mapper.Map<User>(userCreate);

            var passHash = _hashService.GetHashedValue(userCreate.Password, out string salt);
            user.Password = passHash;
            user.Salt = salt;
            user.CreatedDate = user.ModifiedDate = DateTime.UtcNow;

            await _userRepo.CreateAsync(user);
            return Ok(_mapper.Map<UserDto>(user));
        }
    }
}
