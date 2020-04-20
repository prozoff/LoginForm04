using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginForm04.Models
{
    public class User : IdentityUser
    {
        public string lastLogin { get; set; }

        public string createDate { get; set; }

        public bool isBlocked { get; set; }

    }
}
