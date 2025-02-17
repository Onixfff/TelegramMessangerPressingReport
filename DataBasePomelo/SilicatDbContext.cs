﻿using DataBasePomelo.Models.Context;
using DataBasePomelo.Models.silikat;
using Microsoft.EntityFrameworkCore;

namespace DataBasePomelo
{
    public class SilicatDbContext : DbContext
    {
        private readonly SilikatContext _silikatContext;

        public SilicatDbContext(SilikatContext silicatContext)
        {
            _silikatContext = silicatContext;
        }

        // Свойства для доступа к нужным DbSet
        public DbSet<ReportPress> reportPresses => _silikatContext.ReportPresses1Entity;
        public DbSet<ReportPress2> reportPresses2 => _silikatContext.ReportPress2Entity;
        public DbSet<Nomenklatura> Nomenklaturas => _silikatContext.NomenklaturasEntity;

        // Метод для сохранения изменений в обоих контекстах
        public async Task SaveChangesAsync()
        {
            await _silikatContext.SaveChangesAsync();
        }
    }
}
