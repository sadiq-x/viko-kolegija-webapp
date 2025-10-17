using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    //Dto Request to get a specific Profile User
    public sealed class UserProfileRequestDTO
    {
        [Required]
        public int? EntityId { get; set; } = default!;
        [Required]
        public string? Username { get; set; } = default!;
    }

    //Dto Response to get a specific Profile User
    public sealed class UserProfileResponseDTO
    {
        public int Id { get; init; } = default!;
        public string Username { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string? Image { get; init; }
        public string NumberPhone { get; init; } = default!;
        public string Address { get; init; } = default!;
    }
}