using System.ComponentModel.DataAnnotations;

namespace backend_api.Models
{
    public class UserModel
    {
        public int? id { get; set; }
        [Required]
        public string? username { get; set; }
        public string? passwordHash { get; set; }
    }
}