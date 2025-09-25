using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backend_api.Models
{
    public sealed class UserRegisterRequestModel //User Register Model DTO
    {
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "The password need a minimum of 8 characters, including lowercase, uppercase and single digit.")]
        public string PasswordHash { get; set; } = default!;
        [Required, Compare(nameof(PasswordHash), ErrorMessage = "Password need be the same.")]
        public string? ConfirmPasswordHash { get; set; } = default!;

    }

    public sealed class EntityRegisterRequestModel //Entity register Model DTO
    {
        [Required, MaxLength(100)]
        public string? Name { get; set; }
        [Required, EmailAddress, MaxLength(150)]
        public string? Email { get; set; }
        [Required, Phone, MaxLength(20)]
        public string? NumberPhone { get; set; }
        [Required, MaxLength(200)]
        public string? Address { get; set; }
        [JsonIgnore, BindNever]
        public bool Auth { get; } = false;
    }

    public sealed class EntityRegisterResponseModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string NumberPhone { get; init; } = default!;
        public string Address { get; init; } = default!;
    }
}