using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backend_api.Models
{
    //Dto Request to create a new user
    public sealed class UserRegisterRequestDTO
    {
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "The password need a minimum of 8 characters, including lowercase, uppercase and single digit.")]
        public string PasswordHash { get; set; } = default!;
        [Required, Compare(nameof(PasswordHash), ErrorMessage = "Password need be the same.")]
        public string ConfirmPasswordHash { get; set; } = default!;
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = default!;
        public string? Image { get; set; }
        [Required, Phone, MaxLength(20)]
        public string NumberPhone { get; set; } = default!;
        [Required, MaxLength(200)]
        public string Address { get; set; } = default!;
        [MaxLength(20)]
        public string? Birthday { get; set; } = default!;
        [MaxLength(20)]
        public string? Nationality { get; set; } = default!;
        [MaxLength(20)]
        public string? Gender { get; set; } = default!;

    }
}