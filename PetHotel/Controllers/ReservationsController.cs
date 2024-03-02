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
        private readonly IReservationEventSender _eventSender;

        // TODO: 
        public ReservationsController(IRepository<Reservation> reservationRepo, IMapper mapper, IRepository<Pet> petRepo, IReservationEventSender eventSender)
        {
            _reservationRepo = reservationRepo;
            _mapper = mapper;
            _petRepo = petRepo;
            _eventSender = eventSender;
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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.User)}")]
        public async Task<ActionResult<Reservation>> CreateAsync([FromBody] ReservationCreateDto reservationDto)
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

            // prevent multiple reservations for same pet
            var reservations = await _reservationRepo.GetAllAsync(x => x.PetId == pet.Id && x.CheckOutTime == null);
            if (reservations != null)
            {
                return Conflict(reservationDto);
            }

            Reservation reservation = _mapper.Map<Reservation>(reservationDto);
            reservation.CreatedDate = reservation.ModifiedDate = DateTime.UtcNow;
            await _reservationRepo.CreateAsync(reservation);
            await _eventSender.SendCreatedEventAsync(reservation);

            return Ok(reservation);
        }

        // POST api/<ReservationsController>/CheckIn/<ID>
        [HttpPost("CheckIn/{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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
            if (pet == null || pet.CheckedIn)
            {
                return Conflict(reservation);
            } 

            reservation.CheckInTime = reservation.ModifiedDate =  DateTime.UtcNow;
            pet.CheckedIn = true;

            await _reservationRepo.UpdateAsync(reservation); // create reservation
            await _petRepo.UpdateAsync(pet); // update Pet as checked in
            await _eventSender.SendCheckedInEventAsync(reservation);
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
   
            reservation.CheckOutTime = reservation.ModifiedDate = DateTime.UtcNow;
            await _reservationRepo.UpdateAsync(reservation);

            var pet = await _petRepo.GetAsync(x => x.Id == reservation.PetId);
            if (pet != null)
            {
                pet.CheckedIn = false;
                await _petRepo.UpdateAsync(pet); // update pet as checked out
            }

            // TODO: implement reservation cleanup
            //  await _reservationRepo.DeleteAsync(reservation); // delete reservation when checking out
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
