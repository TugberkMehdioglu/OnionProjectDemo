using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ENTITIES.Models
{
    public class Order : BaseEntity
    {
        public string ShippedAddress { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public string AppUserID { get; set; } = null!;

        //Navigation Properties
        public AppUser AppUser { get; set; } = null!;
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
 