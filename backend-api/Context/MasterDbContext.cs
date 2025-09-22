using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace backend_api.Context
{
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }
        public DbSet<Entities> Entities { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }

    }

    [Table("Entities")] //Maps the class User for table User in Database
    public class Entities
    { //Class Entities
        [Key]
        public int Id { get; set; } //Primary key
        [Required]
        public string Name { get; set; } //Name of entity
        [Required]
        [EmailAddress]
        public string Email { get; set; } //Email of entity
        public string Image { get; set; } //Image of entity
        [Phone]
        public string NumberPhone { get; set; } //Number phone of entity
        public string Address { get; set; } //Address of entity
        public string Gender { get; set; } //Gender of entity
        public string Auth { get; set; } //Authorization of entity
        public int Role_id { get; set; } //Type role of entity
        [ForeignKey("Role_id")]
        public Roles Roles { get; set; } //Relationship with id of table Roles
    }

    [Table("Roles")] //Maps the class User for table User in Database
    public class Roles
    { //Class Entities
        [Key]
        public int Id { get; set; } //Primary key
        public string Type { get; set; } //Type of role
    }

    [Table("Users")]
    public class Users
    {
        [Key]
        public int Id { get; set; } //Id of User
        public int Entity_id { get; set; } //Entity_id of User
        [ForeignKey("Entity_id")]
        public Entities Entity { get; set; } //Relationship with id of table Entities
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } //Username of User, required
        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; } //Password of User, required and jsonIgnore
    }

    [Table("Events")] //Maps the class User for table User in Database
    public class Events
    { //Class Entities
        [Key]
        public int Id { get; set; } //Primary key
        [Required]
        public string Name { get; set; } //Name of event
        public string Description { get; set; } //Description of event
        public string Topic { get; set; } //Topic type of event
        public string Create_by_id { get; set; } //Event created by id?
        [ForeignKey("Create_by_id")]
        public Entities Entities { get; set; } //Relationship with id of table Entities
        public string DateCreate { get; set; } //Event created in date
        public bool Status { get; set; } //Status of event, True = event online / False = event finish
        public int Results { get; set; } //Results of event, grades
        
    }
}
