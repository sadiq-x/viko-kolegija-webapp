namespace backend_api.Models
{
    //Dto Response to get all Roles
    public sealed class RolesResponseDTO
    {
        public int Id { get; init; } = default!; //Id of Role
        public string Type { get; init; } = default!; //Type of role
    }
}