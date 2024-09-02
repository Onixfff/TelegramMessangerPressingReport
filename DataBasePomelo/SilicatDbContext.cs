using DataBasePomelo.Models.material_costumer_manufactur;
using DataBasePomelo.Models.silikat;
using Microsoft.EntityFrameworkCore;

namespace DataBasePomelo
{
    public class SilicatDbContext : DbContext
    {
        public SilicatDbContext(DbContextOptions options) : base(options) { }

        public DbSet<ReportEntity> Reports { get; set; }

        public DbSet<ReceptEntity> Recepts { get; set; }

        public DbSet<MaterialEntity> Material { get; set; }

    }
}
