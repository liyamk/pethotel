using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        private readonly IMapper _mapper;

        public PetsController(IRepository<Pet> petRepo, IMapper mapper)
        {
            _petRepo = petRepo;
            _mapper = mapper;
        }

        // GET: api/<PetsController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetAsync()
        {
            IEnumerable<Pet> pets = await _petRepo.GetAllAsync();
            return Ok(_mapper.Map<List<PetDTO>>(pets));
        }

        // GET api/<PetsController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PetDTO>> GetAsync(int id)
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

            return Ok(_mapper.Map<PetDTO>(pet));
        }

        // POST api/<PetsController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult<int>> CreateAsync([FromBody] PetCreateDto petDTO)
        {
            if (petDTO == null)
            {
                return BadRequest(petDTO);
            }

            Pet pet = _mapper.Map<Pet>(petDTO);
            await _petRepo.CreateAsync(pet);
            return Ok(pet.Id);
        }

        // PUT api/<PetsController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult> PutAsync(int id, [FromBody] PetDTO updateDto)
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

