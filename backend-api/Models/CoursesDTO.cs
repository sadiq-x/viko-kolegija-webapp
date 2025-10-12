using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    //Dto Request for all courses without any filter
    public sealed class CoursesListResponseDTO
    {
        public int Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string Topic { get; init; } = default!;
        public int CreateById { get; init; } = default!;
        public string DateCreate { get; init; } = default!;
        public bool Status { get; init; } = default!;
    }

    //DTO Request for all courses from a specific Topics
    public sealed class CoursesListByTopicsRequestDTO
    {
        [Required]
        public string Topic { get; set; } = default!;
    }

    //DTO Request for all courses from a specific Status
    public sealed class CoursesListByStatusRequestDTO
    {
        [Required]
        public string Topic { get; set; } = default!;
    }

    //DTO Request for all courses from a specific CreateById
    public sealed class CoursesListByCreateByIdRequestDTO
    {
        [Required]
        public string CreateById { get; set; } = default!;
    }

    //DTO Request for all courses from a specific Date
    public sealed class CoursesListByDateCreateRequestDTO
    {
        [Required]
        public string DateCreate { get; init; } = default!;
    }
}