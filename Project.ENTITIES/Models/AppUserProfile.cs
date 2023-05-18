using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ENTITIES.Models
{
    public class AppUserProfile : BaseEntity
    {
        public new string ID { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get { return $"{FirstName} {LastName}"; } } //will not be in database
        public string Address { get; set; } = null!;
        public string? ImagePath { get; set; }

        //Navigation Properties
        public AppUser AppUser { get; set; } = null!;
    }
}
 