using System.ComponentModel.DataAnnotations;

namespace PetHotel.Models.Dto
{
    public class UserDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public Role Role { get; set; }
    }
}
