namespace backend_api.Models
{
    public sealed class TopicsResponseDTO
    {
        public int Id { get; init; } = default!;
        public string Type { get; init; } = default!;
        public string Description { get; init; } = default!;
    }
}