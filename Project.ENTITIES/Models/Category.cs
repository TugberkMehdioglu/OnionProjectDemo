using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ENTITIES.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Category()
        {
            //Products = new List<Product>(); //For StrategyPattern
        }

        //Navigation Properties
        public ICollection<Product>? Products { get; set; }
    }
}
