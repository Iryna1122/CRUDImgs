using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CRUDImgs.Class
{
    public class AppContextt : DbContext
    {
        public DbSet<Images> Images { get; set; } = null!;

        public AppContextt(DbContextOptions<AppContextt> options): base(options)
        {
            Database.EnsureCreated();
        }


}
}
