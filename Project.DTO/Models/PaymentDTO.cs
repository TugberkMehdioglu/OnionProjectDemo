using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DTO.Models
{
    //For BankAPI relationship
    public class PaymentDTO
    {
        public int ID { get; set; }
        public string CardUserName { get; set; } = null!;
        public string SecurityNumber { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public int CardExpiryMonth { get; set; }
        public int CardExpiryYear { get; set; }
        public decimal ShoppingPrice { get; set; }

    }
}
 