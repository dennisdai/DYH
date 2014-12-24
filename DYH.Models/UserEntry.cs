using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DYH.Data;

namespace DYH.Models
{
    [TableName("users")]
    [PrimaryKey("userid")]
    public class UserEntry
    {
        [Column("userid")]
        public int UserId { get; set; }
        [Column("username")]
        public string UserName { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("firstname")]
        public string FirstName { get; set; }
        [Column("lastname")]
        public string LastName { get; set; }
        [Column("language")]
        public string Language { get; set; }
        [Column("createdby")]
        public string CreatedBy { get; set; }
        [Column("createdtime")]
        public string CreatedTime { get; set; }
        [Column("changedby")]
        public string ChangedBy { get; set; }
        [Column("changedtime")]
        public string ChangedTime { get; set; }
    }
}
