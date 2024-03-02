using System.ComponentModel.DataAnnotations;

namespace PetHotel.Models.Dto
{
    public class UserDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public Role Role { get; set; }
    }

    public class UserCreateDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}
