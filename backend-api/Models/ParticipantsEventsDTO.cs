using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    //Dto Request for a specific eventId
    public sealed class ParticipantsListFromEventIdRequestDTO
    {
        [Required]
        public int EventId { get; set; } = default!;
    }
    //Dto Response for a specific eventId
    public sealed class ParticipantsListFromEventIdResponseDTO
    {
        public int Id { get; init; } = default!;
        public int EntityId { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Email { get; init; } = default!;
        public bool Status { get; init; } = default!;
        public string? Grade { get; init; } = default!;
        public string? Comments { get; init; } = default!;
    }
}