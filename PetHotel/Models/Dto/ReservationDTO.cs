using System.ComponentModel.DataAnnotations;

namespace PetHotel.Models.Dto
{
    public class ReservationCreateDto
    {
        [Required]
        public RoomType RoomType { get; set; }

        [Required]
        public DateTime ExpectedCheckInTime { get; set; }

        [Required]
        public int PetId { get; set; }
    }

    public class ReservationUpdateDto : BaseReservation
    {
        [Required]
        public int Id { get; set; }
    }

    public class ReservationDto : BaseReservation
    {
        public int Id { get; set; }
    }

    public class BaseReservation
    {
        public RoomType RoomType { get; set; }

        public DateTime ExpectedCheckInTime { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public int PetId { get; set; } 
    }
}
