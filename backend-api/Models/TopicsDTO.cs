using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    public sealed class TopicsResponseDTO
    {
        public int Id { get; init; } = default!;
        public string Type { get; init; } = default!;
        public string Description { get; init; } = default!;
    }
    public sealed class TopicsCreateRequestDTO
    {
        [Required]
        public string Type { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public int? EntityId { get; set; } = default!;
    }
    public sealed class TopicsDeleteRequestDTO
    {
        [Required]
        public int Id { get; set; } = default!;
        [Required]
        public string Type { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public int? EntityId { get; set; } = default!;
    }
}