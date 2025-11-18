using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{   
    //Dto Request for Teacher create a Event 
    public sealed class EventCreateTeacherRequestDTO
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public int TopicsId { get; set; } = default!;
        [Required]
        public int? CreateById { get; set; } = default!;
    }
    //Dto Request for Admin create a Event
    public sealed class EventCreateAdminRequestDTO
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public int TopicsId { get; set; } = default!;
        [Required]
        public int CreateById { get; set; } = default!;
        [Required]
        public int? AdminId { get; set; } = default!;
    }

    //Dto Response for all Event without any filter
    public sealed class EventListResponseDTO
    {
        public int Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string TopicName { get; init; } = default!;
        public string DateCreate { get; init; } = default!;
        public string DateClose { get; init; } = default!;
        public string Status { get; init; } = default!;
        public string CreateBy { get; init; } = default!;
    }

    //Dto Response for all Event without any filter
    public sealed class EventListStudentResponseDTO
    {
        public int Id { get; init; } = default!;
        public int EventId { get; init; } = default!; 
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string TopicName { get; init; } = default!;
        public string DateCreate { get; init; } = default!;
        public string DateClose { get; init; } = default!;
        public string Status { get; init; } = default!;
        public string Grade { get; init; } = default!;
        public string ParticipantDescription { get; init; } = default!;
    }
    //DTO Request for all Event from a specific Topics
    public sealed class EventListByTopicsRequestDTO
    {
        [Required]
        public string Topic { get; set; } = default!;
    }

    //DTO Request for all Event from a specific Status
    public sealed class EventListByStatusRequestDTO
    {
        [Required]
        public string Status { get; set; } = default!;
    }

    //DTO Request for all Event from a specific CreateById
    public sealed class EventByCreateByIdRequestDTO
    {
        [Required]
        public int? CreateById { get; set; } = default!;
    }
    //DTO Request for close a event with a specific id
    public sealed class EventChangeStatusRequestDTO
    {
        [Required]
        public int Id { get; set; } = default!;
        [Required]
        public int? CreateById { get; set; } = default!;
    }

    //DTO Request for all Event from a specific EntityId
    public sealed class EventStudentByEntityIdRequestDTO
    {
        [Required]
        public int? EntityId { get; set; } = default!;
    }

    //DTO Request for all Event from a specific EntityId
    public sealed class EventByEventIdRequestDTO
    {
        [Required]
        public int? Id { get; set; } = default!;
    }
}