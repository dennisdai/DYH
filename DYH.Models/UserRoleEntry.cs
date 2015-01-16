using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DYH.Data;

namespace DYH.Models
{
    [TableName("userroles")]
    [PrimaryKey("userroleid")]
    public class UserRoleEntry
    {
        [Column("userroleid")]
        public int UserRoleId { get; set; }
        [Column("userid")]
        public int UserId { get; set; }
        [Column("roleid")]
        public int RoleId { get; set; }
        [Column("status")]
        public bool Status { get; set; }
    }
}
