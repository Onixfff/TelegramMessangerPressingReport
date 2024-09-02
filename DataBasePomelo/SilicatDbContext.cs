using DataBasePomelo.Models.Context;
using DataBasePomelo.Models.material_costumer_manufactur;
using DataBasePomelo.Models.silikat;
using Microsoft.EntityFrameworkCore;

namespace DataBasePomelo
{
    public class SilicatDbContext : DbContext
    {
        private readonly MaterialCostumerManufacturContext _materialContext;
        private readonly SilikatContext _silikatContext;
        
        public SilicatDbContext(MaterialCostumerManufacturContext materialContext, SilikatContext silicatContext)
        {
            _materialContext = materialContext;
            _silikatContext = silicatContext;
        }

        // Свойства для доступа к нужным DbSet
        public DbSet<MaterialEntity> Materials => _materialContext.Materials;
        public DbSet<ManufacturerEntity> Manufacturers => _materialContext.Manufacturers;
        public DbSet<GroupMaterialEntity> GroupMaterials => _materialContext.GroupMaterials;
        public DbSet<ReceptEntity> Recepts => _silikatContext.Recepts;
        public DbSet<ReportEntity> Reports => _silikatContext.Reports;

        // Метод для сохранения изменений в обоих контекстах
        public async Task SaveChangesAsync()
        {
            await _materialContext.SaveChangesAsync();
            await _silikatContext.SaveChangesAsync();
        }
    }
}
