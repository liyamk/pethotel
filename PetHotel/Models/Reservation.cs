using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetHotel.Models
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public RoomType RoomType { get; set; }

        [Required]
        public DateTime ExpectedCheckInTime { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        // [ForeignKey("Pet")]
        public int PetId { get; set; } // foreign key to tie Pet and Reservation table
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoomType
    {
        Standard,

        Deluxe
    }

}
