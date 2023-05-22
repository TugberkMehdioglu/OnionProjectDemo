using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MAP.Configurations
{
    public class ProductConfiguration : BaseConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");

            HashSet<Product> products = new HashSet<Product>()
            {
                new()
                {
                    ID = 1,
                    CreatedDate = DateTime.Now,
                    Status=ENTITIES.Enums.DataStatus.Inserted,
                    ImagePath=null,
                    Name="AMD Ryzen™ 9 7950X3D Soket AM5 4.2GHz 128MB 120W 5nm İşlemci",
                    Price=19.828m,
                    Stock=100,
                    CategoryID=1
                },
                new()
                {
                    ID = 2,
                    CreatedDate = DateTime.Now,
                    Status=ENTITIES.Enums.DataStatus.Inserted,
                    ImagePath=null,
                    Name="Intel Core i9 13900 Soket 1700 13.Nesil 2GHz 32MB Önbellek 10nm İşlemci",
                    Price=17.334m,
                    Stock=100,
                    CategoryID=1
                },
                new()
                {
                    ID = 3,
                    CreatedDate = DateTime.Now,
                    Status=ENTITIES.Enums.DataStatus.Inserted,
                    ImagePath=null,
                    Name="GIGABYTE GeForce RTX 3090 Ti GAMING 24GB GDDR6X 384Bit Nvidia Ekran Kartı",
                    Price=64.304m,
                    Stock=100,
                    CategoryID=2
                },
                new()
                {
                    ID = 4,
                    CreatedDate = DateTime.Now,
                    Status=ENTITIES.Enums.DataStatus.Inserted,
                    ImagePath=null,
                    Name="MSI GeForce RTX 4090 SUPRIM X 24GB GDDR6X 384Bit DLSS 3 Ekran Kartı",
                    Price=55.730m,
                    Stock=100,
                    CategoryID=2
                }
            };

            builder.HasData(products);
        }
    }
}
