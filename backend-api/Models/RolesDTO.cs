using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    //Dto Response to get all Roles
    public sealed class RolesResponseDTO
    {
        public string Type { get; init; } = default!; //Type of role
    }

    public sealed class RolesRequestDTO
    {
        [Required]
        public int? EntityId { get; set; } = default!; //Type of role
        public string? Username { get; set; } = default!; //Type of role
        public string? Type { get; set; } = default!; //Type of role

    }
}