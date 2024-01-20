using System.ComponentModel.DataAnnotations;

namespace PetHotel.Models.Dto
{
    public class PetDTO : BasePet
    {
        public int Id { get; set; }
    }

    public class PetCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public PetType Type { get; set; }

    }

    public class PetUpdateDto : BasePet
    {

    }
    public class BasePet
    {
        public string Name { get; set; }

        public PetType Type { get; set; }

        public bool CheckedIn { get; set; }
    }
}
