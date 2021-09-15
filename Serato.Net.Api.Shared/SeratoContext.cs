using Microsoft.EntityFrameworkCore;
using Serato.Net.Api.Shared.Models;
using Serato.Net.Structs;

namespace Serato.Net.Api.Shared
{
    public class SeratoContext : DbContext
    {
        public SeratoContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<TrackInfoEntity> TrackInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrackInfoEntity>().OwnsOne(t => t.TrackInfo);
            modelBuilder.Owned<TrackInfo>();
        }
    }
}