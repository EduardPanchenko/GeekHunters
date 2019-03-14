using Microsoft.EntityFrameworkCore;
using System.Linq;

//Scaffold-DbContext "Filename=GeekHunter.sqlite" Microsoft.EntityFrameworkCore.Sqlite

namespace GeekHunters.Models
{
    public partial class GeekHunterContext : DbContext
    {
        //public GeekHunterContext() => Database.EnsureCreated();

        public GeekHunterContext(DbContextOptions<GeekHunterContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();

            if (Skill.Any()) return;

            var sk = new[] { "All", "SQL", "JavaScript", "C#", "Java", "Python", "React" }
                  .Select((x, i) => new Skill { Name = x, Filter = i % 3 == 0 })
                  .ToArray();
            AddRange(sk);

            var cd1 = new Candidate { FirstName = "Jon", LastName = "Snow" };
            var cd2 = new Candidate { FirstName = "Tyrion", LastName = "Lannister" };
            Add(cd1);
            Add(cd2);
            
            sk = sk.Skip(1).ToArray();
            AddRange(sk.Where((_, i) => (i & 1) == 0)
                .Select(x => new CandidateSkill { Candidate = cd1, Skill = x }));
            AddRange(sk.Where((_, i) => (i & 1) == 1)
                .Select(x => new CandidateSkill { Candidate = cd2, Skill = x }));
            SaveChanges();
        }

        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<Skill> Skill { get; set; }
        public virtual DbSet<CandidateSkill> CandidateSkill { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (optionsBuilder.IsConfigured) return;
        //    //To protect potentially sensitive information in your connection string,
        //    //you should move it out of source code.
        //    //See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //    optionsBuilder.UseSqlite("Filename=GeekHunter.sqlite");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            //modelBuilder.Entity<Candidate>(entity => entity.Property(en => en.CandidateId).ValueGeneratedNever());

            modelBuilder.Entity<CandidateSkill>(entity =>
            {
                entity.HasKey(e => new { e.CandidateId, e.SkillId });

                entity.HasIndex(e => e.SkillId);

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.CandidateSkill)
                    .HasForeignKey(d => d.CandidateId);

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.CandidateSkill)
                    .HasForeignKey(d => d.SkillId);
            });

            //modelBuilder.Entity<Skill>(entity => entity.Property(e => e.SkillId).ValueGeneratedOnAdd());
        }
    }
}
