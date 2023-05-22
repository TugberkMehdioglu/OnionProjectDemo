using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MAP.Configurations
{
    public class CategoryConfiguration : BaseConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            base.Configure(builder);

            HashSet<Category> categories = new HashSet<Category>()
            {
                new()
                {
                    ID = 1,
                    CreatedDate = DateTime.Now,
                    Status=ENTITIES.Enums.DataStatus.Inserted,
                    Name="İşlemci",
                    Description="İşlemci fiyatları, modelleri ve güvenilir işlemci markaları uygun ödeme seçenekleriyle şimdi inceleyin ve hemen satın alın."
                },
                new()
                {
                    ID = 2,
                    CreatedDate = DateTime.Now,
                    Status=ENTITIES.Enums.DataStatus.Inserted,
                    Name="Ekran Kartı",
                    Description="Ekran kartı fiyatları, modelleri ve güvenilir ekran kartı markaları uygun ödeme seçenekleriyle şimdi inceleyin ve hemen satın alın."
                }
            };

            builder.HasData(categories);
        }
    }
}
