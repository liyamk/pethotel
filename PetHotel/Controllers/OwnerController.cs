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
    public class OwnerController : ControllerBase
    {
        private readonly IRepository<Owner> _ownerRepo;
        private readonly IMapper _mapper;
        public OwnerController(IRepository<Owner> ownerRepo, IMapper mapper)
        {
            _ownerRepo = ownerRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OwnerDto>>> GetAsync()
        {
            var owners = await _ownerRepo.GetAllAsync();
            return Ok(_mapper.Map<List<OwnerDto>>(owners));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Owner>> GetAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var owner = await _ownerRepo.GetAsync(x => x.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return Ok(owner);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult<int>> CreateAsync([FromBody] OwnerCreateDto ownerDto)
        {
            if (ownerDto == null)
            {
                return BadRequest(ownerDto);
            }

            var owner = _mapper.Map<Owner>(ownerDto);
            await _ownerRepo.CreateAsync(owner);
            return Ok(owner);
        }

        // TODO: implement 
        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public ActionResult PutAsync(int id, [FromBody] Owner owner)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var owner = await _ownerRepo.GetAsync(x => x.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            await _ownerRepo.DeleteAsync(owner);
            return Ok();
        }
    }
}
