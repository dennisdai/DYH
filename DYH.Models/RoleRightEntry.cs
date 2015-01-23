using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DYH.Data;

namespace DYH.Models
{
    [TableName("rolerights")]
    [PrimaryKey("rightid")]
    public class RoleRightEntry
    {
        [Column("rightid")]
        public int RightId { get; set; }
        [Column("roleid")]
        public int RoleId { get; set; }
        [Column("actionmoduleid")]
        public int ActionModuleId { get; set; }
        [Column("status")]
        public bool Status { get; set; }
    }
}
