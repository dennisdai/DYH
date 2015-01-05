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
    [TableName("actions")]
    [PrimaryKey("actionid")]
    public class ActionEntry
    {
        [Column("actionid")]
        public int ActionId { get; set; }
        [DisplayName("Action Code")]
        [Required]
        [Remote("CheckCode", "Actions", ErrorMessage = "Action code has been used, please change one.")]
        [Column("actioncode")]
        public string ActionCode { get; set; }
        [DisplayName("Display Name")]
        [Required]
        [Column("displayname")]
        public string DisplayName { get; set; }
        [DisplayName("Sequence")]
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid number.")]
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
