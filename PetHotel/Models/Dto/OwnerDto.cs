using System.ComponentModel.DataAnnotations;

namespace PetHotel.Models.Dto
{
    public class OwnerDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Pet> Pets { get; set; }
    }

    public class OwnerCreateDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
