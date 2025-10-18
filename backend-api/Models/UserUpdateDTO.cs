using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    public sealed class UserUpdateRequestDTO
    {
        [Required]
        public int EntityId { get; set; } = default!;
        [Required]
        public string Username { get; set; } = default!;
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = default!;
        public string? Image { get; set; }
        [Required, Phone, MaxLength(20)]
        public string NumberPhone { get; set; } = default!;
        [Required, MaxLength(200)]
        public string Address { get; set; } = default!;
        [MaxLength(20)]
        public string? Birthday { get; set; } = default!;
        [MaxLength(20)]
        public string? Nationality { get; set; } = default!;
        [MaxLength(20)]
        public string? Gender { get; set; } = default!;
    }
}