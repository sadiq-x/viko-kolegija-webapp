using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend_api.Models
{
    public class UserModel
    {
        public int? id { get; set; }
        [Required]
        public string? username { get; set; }
        [JsonIgnore]
        public string? passwordHash { get; set; }
    }
}