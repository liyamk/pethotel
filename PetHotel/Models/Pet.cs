using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetHotel.Models
{
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public PetType Type { get; set; }

        public bool CheckedIn { get; set; }
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PetType
    {
        Cat,

        Dog
    }

}
