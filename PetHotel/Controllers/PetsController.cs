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
    public class PetsController : ControllerBase
    {
        private readonly IRepository<Pet> _petRepo;
        private readonly IRepository<Owner> _ownerRepo;
        private readonly IMapper _mapper;

        public PetsController(IRepository<Pet> petRepo, IRepository<Owner> ownerRepo, IMapper mapper)
        {
            _petRepo = petRepo;
            _ownerRepo = ownerRepo;
            _mapper = mapper;
        }

        // GET: api/<PetsController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PetDto>>> GetAsync()
        {
            IEnumerable<Pet> pets = await _petRepo.GetAllAsync();
            return Ok(_mapper.Map<List<PetDto>>(pets));
        }

        // GET api/<PetsController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDto>> GetAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var pet = await _petRepo.GetAsync(x => x.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PetDto>(pet));
        }

        // POST api/<PetsController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult<Pet>> CreateAsync([FromBody] PetCreateDto petDto)
        {
            if (petDto == null)
            {
                return BadRequest(petDto);
            }

            var owner = await _ownerRepo.GetAsync(x => x.Id == petDto.OwnerId);
            if (owner == null)
            {
                return BadRequest(petDto);
            }

            // an owner can't have two pets with same name and type
            var matched = owner.Pets.Where(x => x.Name == petDto.Name && x.Type == petDto.Type).FirstOrDefault();
            if (matched != null) 
            {
                return Conflict(petDto);
            }

            Pet pet = _mapper.Map<Pet>(petDto);
            await _petRepo.CreateAsync(pet);
            return Ok(pet);
        }

        // PUT api/<PetsController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult> PutAsync(int id, [FromBody] PetDto updateDto)
        {
            if (id == 0 && id != updateDto.Id)
            {
                return BadRequest();
            }

            var pet = await _petRepo.GetAsync(x => x.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            await _petRepo.UpdateAsync(_mapper.Map<Pet>(updateDto));

            return Ok(StatusCode(StatusCodes.Status204NoContent));
        }

        // DELETE api/<PetsController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var pet = await _petRepo.GetAsync(x => x.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            await _petRepo.DeleteAsync(pet);
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}

