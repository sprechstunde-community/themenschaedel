using System;
using System.Collections.Generic;
using System.Text;

namespace Themenschaedel.Shared.Models
{
    public enum UserRole
    {
        RegularUser,
        VerifiedUser,
        Froid,
        Moderator,
        Admin
    }

    public class Roles
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RolesExtended : Roles
    {
        public RolesExtended(){}

        public UserRole Role
        {
            get
            {
                switch (Id)
                {
                    case 1:
                        return UserRole.RegularUser;
                    case 2:
                        return UserRole.VerifiedUser;
                    case 3:
                        return UserRole.Froid;
                    case 4:
                        return UserRole.Moderator;
                    case 5:
                        return UserRole.Admin;
                    default:
                        return UserRole.RegularUser;
                }
            }
        }
    }
}
