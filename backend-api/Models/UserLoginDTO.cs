using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend_api.Models
{
    //Dto Request for Login user
    public sealed class UserLoginRequestDTO
    {
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        public string PasswordHash { get; set; } = default!;
    }

    //Dto Response for Login user
    public sealed class UserLoginResponseDTO
    {
        public int EntityId { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Username { get; init; } = default!;
        public string RoleType { get; init; } = default!;
    }
}