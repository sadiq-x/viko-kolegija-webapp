using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    public sealed class UserUpdatePasswordRequestDTO
    {
        [Required]
        public int Id { get; set; } = default!;
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        public string PasswordHash { get; set; } = default!;
    }
}