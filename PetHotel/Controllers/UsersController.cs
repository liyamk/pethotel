using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHotel.IServices;
using PetHotel.Models;

namespace PetHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepo;
        public UsersController(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
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
