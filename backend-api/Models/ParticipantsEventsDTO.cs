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
    //Dto Request to insert grade on individual student
    public sealed class ParticipantsEventGradeRequestDTO
    {
        [Required]
        public int Id { get; init; } = default!;
        [Required]
        public int EventId { get; init; } = default!;
        [Required]
        public string Grade { get; set; } = default!;
        [MaxLength(200)]
        public string? Comments { get; set; }
    }
    //Dto Request to update status of individual student
    public sealed class ParticipantsEventUpdateStatusRequestDTO        
    {
        [Required]
        public int Id { get; init; } = default!;
        [Required]
        public int EventId { get; init; } = default!;
        [Required]
        public int EntityId { get; set; } = default!;
    }
}