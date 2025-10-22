using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    //Dto Request to create a Event
    public sealed class EventCreateRequestDTO
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
        public string DateCreate { get; set; } = default!;
        public bool Status { get; set; } = true;
        public int Results { get; set; } = 0;
    }

    //Dto Response for all Event without any filter
    public sealed class EventListResponseDTO
    {
        public int Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string TopicName { get; init; } = default!;
        public int CreateById { get; init; } = default!;
        public string DateCreate { get; init; } = default!;
        public bool Status { get; init; } = default!;
        public int Results { get; init; } = default!;
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
    public sealed class EventListByIdRequestDTO
    {
        [Required]
        public int CreateById { get; set; } = default!;
    }

    //DTO Request for all Event from a specific Date
    public sealed class EventListByDateCreateRequestDTO
    {
        [Required]
        public string DateCreate { get; init; } = default!;
    }
}