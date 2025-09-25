using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend_api.Models
{
    public sealed class UserLoginRequestModel
    {
        public int? Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required, JsonIgnore]
        public string? PasswordHash { get; set; }
    }

    public sealed class UserLoginResponseModel
    {
        public int Id { get; init; }
        public string Username { get; init; } = default!;
    }
}