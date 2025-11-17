using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    //Dto Response to get all Roles
    public sealed class RolesResponseDTO
    {
        public string Type { get; init; } = default!; 
    }

    public sealed class RolesRequestDTO
    {
        [Required]
        public int? EntityId { get; set; } = default!; 
        public string? Username { get; set; } = default!;
        public string? Type { get; set; } = default!; 
    }
    public sealed class RolesUpdateUserRequestDTO
    {
        [Required]
        public int Id { get; set; } = default!;
        [Required, EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Type { get; set; } = default!;
        [Required]
        public int? EntityId { get; set; } = default!;
    }
}