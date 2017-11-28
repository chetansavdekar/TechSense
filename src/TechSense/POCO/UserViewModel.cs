using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.POCO
{
    public class UserViewModel
    {
        public string CurrentUsernameHash;
        public IEnumerable<UserEntity> Users;
    }
}
