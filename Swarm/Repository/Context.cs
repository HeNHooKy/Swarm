using Microsoft.EntityFrameworkCore;
using Swarm.Models.EFModel;

namespace Swarm.Repository
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flat>(e =>
            {
                e.HasKey(f => new { f.Street, f.Building, f.FlatNumber });
                e.HasOne(f => f.Meter);
            });

            modelBuilder.Entity<Meter>()
                .HasKey(m => m.FactoryNumber);

            modelBuilder.Entity<MeterRecords>(mr =>
            {
                mr.HasKey(mr => new { mr.MeterFactoryNumber, mr.CheckDate });
                mr.HasOne(mr => mr.Meter)
                .WithMany(m => m.MeterRecords);
            });

            modelBuilder.Entity<MeterReplacementHistory>()
                .HasKey(mrh => new { mrh.Street, mrh.Building, mrh.FlatNumber, mrh.SetupDate });
        }

        public DbSet<Flat> Flats { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<MeterRecords> MeterRecords { get; set; }
        public DbSet<MeterReplacementHistory> MeterReplacementHistory { get; set; }
    }
}
