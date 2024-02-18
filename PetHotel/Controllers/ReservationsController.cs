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
    public class ReservationsController : ControllerBase
    {
        private readonly IRepository<Reservation> _reservationRepo;
        private readonly IMapper _mapper;
        private readonly IRepository<Pet> _petRepo;

        // TODO: 
        public ReservationsController(IRepository<Reservation> reservationRepo, IMapper mapper, IRepository<Pet> petRepo)
        {
            _reservationRepo = reservationRepo;
            _mapper = mapper;
            _petRepo = petRepo;
        }

        // GET: api/<ReservationsController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAsync()
        {
            IEnumerable<Reservation> reservations = await _reservationRepo.GetAllAsync();
            return Ok(_mapper.Map<List<ReservationDto>>(reservations));
        }

        // GET api/<ReservationsController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservationDto>> GetAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var reservation = await _reservationRepo.GetAsync(x => x.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ReservationDto>(reservation));
        }

        // POST api/<ReservationsController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult<int>> CreateAsync([FromBody] ReservationCreateDto reservationDto)
        {
            if (reservationDto == null)
            {
                return BadRequest(reservationDto);
            }

            var pet = await _petRepo.GetAsync(x => x.Id == reservationDto.PetId);
            if (pet == null)
            {
                return BadRequest(reservationDto.PetId);
            }

            Reservation reservation = _mapper.Map<Reservation>(reservationDto);
            await _reservationRepo.CreateAsync(reservation);

            return Ok(reservation.Id);
        }

        // POST api/<ReservationsController>/CheckIn/<ID>
        [HttpPost("CheckIn/{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult> CheckInAsync(int id)
        {
            // TODO validate check in time is on same day as expected
            if (id == 0)
            {
                return BadRequest();
            }

            var reservation = await _reservationRepo.GetAsync(x => x.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            var pet = await _petRepo.GetAsync(x => x.Id == reservation.PetId);
            if (pet == null)
            {
                return BadRequest(reservation.PetId);
            } 

            reservation.CheckInTime = DateTime.UtcNow;
            pet.CheckedIn = true;
           // reservation.Pet = pet;

            await _reservationRepo.UpdateAsync(reservation); // create reservation
            await _petRepo.UpdateAsync(pet); // update Pet as checked in
            return Ok(StatusCode(StatusCodes.Status201Created));
        }


        [HttpPost("CheckOut/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<IActionResult> CheckOutAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var reservation = await _reservationRepo.GetAsync(x => x.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
   
            reservation.CheckOutTime = DateTime.UtcNow;
            await _reservationRepo.UpdateAsync(reservation);

            var pet = reservation.Pet;
            if (pet != null)
            {
                pet.CheckedIn = false;
                await _petRepo.UpdateAsync(pet); // update pet as checked out
            }

            return Ok("Check-out successful");
        }

        // DELETE api/<ReservationsController>/5
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

            var reservation = await _reservationRepo.GetAsync(x => x.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            await _reservationRepo.DeleteAsync(reservation);
            // send cancellation 
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
