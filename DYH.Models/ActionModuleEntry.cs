using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DYH.Data;

namespace DYH.Models
{
    [TableName("actionmodules")]
    [PrimaryKey("actionmoduleid")]
    public class ActionModuleEntry
    {
        [Column("actionmoduleid")]
        public int ActionModuleId { get; set; }
        [Column("actionid")]
        public int ActionId { get; set; }
        [Column("moduleid")]
        public int ModuleId { get; set; }
        [Column("status")]
        public bool Status { get; set; }
    }
}
