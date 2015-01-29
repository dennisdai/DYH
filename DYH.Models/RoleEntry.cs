using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DYH.Data;

namespace DYH.Models
{
    [TableName("roles")]
    [PrimaryKey("roleid")]
    public class RoleEntry
    {
        [Column("roleid")]
        public int RoleId { get; set; }

        [Required]
        [DisplayName("Role Code")]
        [Remote("CheckCode", "Roles", ErrorMessage = "Role code has been used, please changed one.")]
        [Column("rolecode")]
        public string RoleCode { get; set; }

        [Required]
        [DisplayName("Display Name")]
        [Column("displayname")]
        public string DisplayName { get; set; }

        [Required]
        [DisplayName("Sequence")]
        //mono中不能启用
        //[RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid number.")]
        [Column("seqno")]
        public int SeqNo { get; set; }

        [DisplayName("Description")]
        [Column("description")]
        public string Description { get; set; }
        [Column("createdby")]
        public string CreatedBy { get; set; }
        [Column("createdtime")]
        public DateTime? CreatedTime { get; set; }
        [Column("changedby")]
        public string ChangedBy { get; set; }
        [Column("changedtime")]
        public DateTime? ChangedTime { get; set; }
    }
}
