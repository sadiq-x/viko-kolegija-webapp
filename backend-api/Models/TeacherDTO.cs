using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    public sealed class TeacherRequestDTO
    {
        [Required]
        public int? EntityId { get; set; } = default!;
    }

    public sealed class TeachersListResponseDTO
    {
        public int Id { get; init; } = default!;
        public string Name { get; init; } = default!;
    }
}