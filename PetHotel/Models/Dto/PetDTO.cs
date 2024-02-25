using System.ComponentModel.DataAnnotations;

namespace PetHotel.Models.Dto
{
    public class PetDto : BasePet
    {
        public int Id { get; set; }
    }

    public class PetCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public PetType Type { get; set; }

        [Required]
        public int OwnerId { get; set; } 
    }

    public class PetUpdateDto : BasePet
    {

    }
    public class BasePet
    {
        public string Name { get; set; }

        public PetType Type { get; set; }

        public bool CheckedIn { get; set; }

        public int OwnerId { get; set; }
    }
}
