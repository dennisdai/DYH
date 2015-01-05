using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DYH.Data;

namespace DYH.Models
{
    [TableName("users")]
    [PrimaryKey("userid")]
    public class UserEntry
    {
        [Column("userid")]
        public int UserId { get; set; }

        [Required]
        [DisplayName("User Name")]
        [Remote("CheckUserName", "Users", ErrorMessage = "User name has been used, please change one.")]
        [Column("username")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("Password")]
        [StringLength(20, MinimumLength = 6)]
        [Column("password")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Email")]
        [Remote("CheckEmail", "Users", ErrorMessage = "Email has been used, please change one.")]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [DisplayName("First Name")]
        [Column("firstname")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [Column("lastname")]
        public string LastName { get; set; }
        
        [DisplayName("language")]
        [Column("language")]
        public string Language { get; set; }
        
        [Column("createdby")]
        public string CreatedBy { get; set; }
        
        [Column("createdtime")]
        public DateTime? CreatedTime { get; set; }
        
        [Column("changedby")]
        public string ChangedBy { get; set; }
        
        [Column("changedtime")]
        public DateTime? ChangedTime { get; set; }

        [DisplayName("Re-Password")]
        [StringLength(20, MinimumLength = 6)]
        [Compare("Password")]
        [Ignore]
        public string NonPassword { get; set; }
    }
}
