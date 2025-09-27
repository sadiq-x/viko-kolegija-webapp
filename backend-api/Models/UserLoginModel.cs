using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend_api.Models
{
    public sealed class UserLoginRequestModel
    {
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        public string PasswordHash { get; set; } = default!;
    }

    public sealed class UserLoginResponseModel
    {
        public int? EntityId { get; init; }
        public string? Username { get; init; } = default!;
    }
}