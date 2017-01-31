using System;
using System.Collections.Generic;

namespace SportsStore2.API.Models
{
    public partial class User
    {
        public User()
        {
            Address = new HashSet<Address>();
        }

        public long Id { get; set; }
        public string ASPNETUsersId { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string HomeNo { get; set; }
        public string MobNo { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }

    }
}
