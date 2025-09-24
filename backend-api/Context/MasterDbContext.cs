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
        public DbSet<Events> Events { get; set; }
        public DbSet<ParticipantsEvents> ParticipantsEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ParticipantsEvents → Events (N:1) / If someone drop the Event id, the FK EventId will be full dropped
            modelBuilder.Entity<ParticipantsEvents>()   //Fluent API
                .HasOne(pe => pe.Event)
                .WithMany(e => e.ParticipantsEvents)
                .HasForeignKey(pe => pe.EventId)
                .OnDelete(DeleteBehavior.Cascade);  // ON DELETE CASCADE

            // ParticipantsEvents → Entities (N:1) / If someone drop the Event id, the FK EventId will impossible
            modelBuilder.Entity<ParticipantsEvents>()
                .HasOne(pe => pe.Entity)
                .WithMany(en => en.ParticipantsEvents)
                .HasForeignKey(pe => pe.EntityId)
                .OnDelete(DeleteBehavior.Restrict);    // ON DELETE RESTRICT
        }
    }



    [Table("Entities")] //Maps the class User for table User in Database
    [Index(nameof(Email), IsUnique = true)]
    public class Entities
    { //Class Entities
        [Key]
        public int Id { get; set; } //Primary key
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!; //Name of entity
        [Required, MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!; //Email of entity
        public string Image { get; set; } //Image of entity
        [MaxLength(20)]
        [Phone]
        public string NumberPhone { get; set; } //Number phone of entity
        [MaxLength(200)]
        public string Address { get; set; } //Address of entity
        [MaxLength(20)]
        public string Gender { get; set; } //Gender of entity
        [Required]
        public bool Auth { get; set; } = false; //Authorization of entity
        public int RoleId { get; set; } //Type role of entity
        [ForeignKey("RoleId")]
        public Roles Roles { get; set; } //Relationship with id of table Roles
        public ICollection<ParticipantsEvents> ParticipantsEvents { get; set; } = new List<ParticipantsEvents>();

    }

    [Table("Roles")] //Maps the class User for table User in Database
    [Index(nameof(Type), IsUnique = true)]
    public class Roles
    { //Class Entities
        [Key]
        public int Id { get; set; } //Primary key
        [Required]
        public string Type { get; set; } //Type of role
    }

    [Table("Users")]
    [Index(nameof(Username), IsUnique = true)]
    public class Users
    {
        [Key]
        public int Id { get; set; } //Id of User
        [Required]
        public int EntityId { get; set; } //Entity_id of User
        [ForeignKey("EntityId")]
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
        public int CreateById { get; set; } //Event created by id?
        [ForeignKey("CreateById")]
        public Entities Entities { get; set; } //Relationship with id of table Entities
        public string DateCreate { get; set; } //Event created in date
        public bool Status { get; set; } //Status of event, True = event online / False = event finish
        public int Results { get; set; } //Results of event, grades
        public ICollection<ParticipantsEvents> ParticipantsEvents { get; set; } = new List<ParticipantsEvents>();


    }

    [Table("ParticipantsEvents")] //Maps the class User for table User in Database
    public class ParticipantsEvents
    { //Class Entities
        [Key]
        public int Id { get; set; } //Primary key
        public int EventId { get; set; } //Event id?
        [ForeignKey("EventId")]
        public Events Event { get; set; } = null!; //Relationship with id of table Events
        public int EntityId { get; set; } //Entity id?
        [ForeignKey("EntityId")]
        public Entities Entity { get; set; } //Relationship with id of table Entities
        public bool status { get; set; } //Status of participant in event
        public string Grade { get; set; } //Grade in 1 event of 1 participant
        public string CertificateData { get; set; } //Certificate Data in 1 event of 1 participant
        public string Progress { get; set; } //Progress in 1 event of 1 participant
    }
}
