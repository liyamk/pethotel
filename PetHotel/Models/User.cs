using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace PetHotel.Models
{
    public class User
    {
        [Key]
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role
    {
        Admin,
        User,
        Reader
    }

    public class LoginUser
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class JwtToken
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
