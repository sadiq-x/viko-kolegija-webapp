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
        public DbSet<Topics> Topics { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ParticipantsEvents → Events (N:1) / If someone drop the Event, the FK EventId will be full dropped
            modelBuilder.Entity<ParticipantsEvents>()   //Fluent API
                .HasOne(pe => pe.Event)
                .WithMany(e => e.ParticipantsEvents)
                .HasForeignKey(pe => pe.EventId)
                .OnDelete(DeleteBehavior.Cascade); // ON DELETE CASCADE

            // ParticipantsEvents → Entities (N:1) / If someone drop the Event id, the FK EventId will impossible
            modelBuilder.Entity<ParticipantsEvents>()
                .HasOne(pe => pe.Entity)
                .WithMany(en => en.ParticipantsEvents)
                .HasForeignKey(pe => pe.EntityId)
                .OnDelete(DeleteBehavior.Restrict); // ON DELETE RESTRICT

            // Events → Topics (N:1) / If someone drop the Event id, the FK EventId will impossible
            modelBuilder.Entity<Events>()
                .HasOne(pe => pe.Topics)
                .WithMany(en => en.Events)
                .HasForeignKey(pe => pe.TopicsId)
                .OnDelete(DeleteBehavior.Restrict); // ON DELETE RESTRICT
            // Events → Entities (N:1)
            modelBuilder.Entity<Events>()
                .HasOne(e => e.Entities)
                .WithMany(ent => ent.Events)
                .HasForeignKey(e => e.CreateById)
                .OnDelete(DeleteBehavior.Restrict); // ON DELETE RESTRICT
            // Events → Status (N:1)
            modelBuilder.Entity<Events>()
                .HasOne(e => e.StatusEvents)
                .WithMany(ent => ent.Events)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict); // ON DELETE RESTRICT
        }
    }

    [Table("Entities")] //Maps the class User for table User in Database
    [Index(nameof(Email), IsUnique = true)]
    public class Entities
    { //Class Entities
        [Key]
        public int Id { get; set; } 
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!; 
        [Required, MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!; 
        public string? Image { get; set; }
        [MaxLength(20)]
        [Phone]
        public string NumberPhone { get; set; } 
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(20)]
        public string? Birthday { get; set; } 
        [MaxLength(20)]
        public string? Nationality { get; set; } 
        [MaxLength(20)]
        public string? Gender { get; set; } 
        public int RoleId { get; set; } 
        [ForeignKey("RoleId")]
        public Roles Roles { get; set; }
        public ICollection<ParticipantsEvents> ParticipantsEvents { get; set; } = new List<ParticipantsEvents>();
        public ICollection<Events> Events { get; set; } = new List<Events>();
    }

    [Table("Roles")] 
    [Index(nameof(Type), IsUnique = true)]
    public class Roles
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        public string Type { get; set; }  
    }

    [Table("Users")]  
    [Index(nameof(Username), IsUnique = true)]
    public class Users
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EntityId { get; set; }  
        [ForeignKey("EntityId")]
        public Entities Entity { get; set; }  
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }  
        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; } 
    }

    [Table("Events")]  
    public class Events
    { 
        [Key]
        public int Id { get; set; }  
        [Required]
        public string Name { get; set; }  
        public string Description { get; set; }  
        public int TopicsId { get; set; }  
        [ForeignKey("TopicsId")]
        public Topics Topics { get; set; }  
        public int CreateById { get; set; }  
        [ForeignKey("CreateById")]
        public Entities Entities { get; set; } 
        public string DateCreate { get; set; } 
        public string? DateClose { get; set; } 
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]    
        public StatusEvents StatusEvents { get; set; } 
        public ICollection<ParticipantsEvents> ParticipantsEvents { get; set; } = new List<ParticipantsEvents>();
    }

    [Table("ParticipantsEvents")] 
    public class ParticipantsEvents
    { 
        [Key]
        public int Id { get; set; }  
        public int EventId { get; set; }  
        [ForeignKey("EventId")]
        public Events Event { get; set; } = null!;  
        public int EntityId { get; set; }  
        [ForeignKey("EntityId")]
        public Entities Entity { get; set; } 
        public bool Status { get; set; }
        public string? Grade { get; set; }  
        public string? Comments { get; set; }  
        public string? ParticipantDescription { get; set; }  
    }

    [Table("Topics")] 
    [Index(nameof(Type), IsUnique = true)]
    public class Topics
    { 
        [Key]
        public int Id { get; set; } 
        [Required]
        public string Type { get; set; } = null!; 
        [Required]
        public string Description { get; set; } = null!; 
        public ICollection<Events> Events { get; set; } = new List<Events>();
    }

    [Table("StatusEvents")] 
    [Index(nameof(Type), IsUnique = true)]
    public class StatusEvents
    { 
        [Key]
        public int Id { get; set; } 
        [Required]
        public string Type { get; set; } = null!; 
        public ICollection<Events> Events { get; set; } = new List<Events>();
    }
}
