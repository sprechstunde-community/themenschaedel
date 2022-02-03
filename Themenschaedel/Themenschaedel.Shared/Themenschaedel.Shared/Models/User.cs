using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Themenschaedel.Shared.Models
{
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("uuid")]
        public string UUID { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("email_verified_at")]
        public DateTime EmailVerifiedAt { get; set; }
        [Column("email_verification_id")]
        public string EmailVerificationId { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("salt")]
        public byte[] Salt { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("id_roles")]
        public int RolesId { get; set; }
    }

    public class UserMinimal
    {
        [Column("uud")]
        public string UUID { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("id_roles")]
        public int RolesId { get; set; }

        public UserRole Role
        {
            get
            {
                return RoleMisc.GetUserRoleByRoleId(RolesId);
            }
        }
    }
}
