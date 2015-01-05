using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using DYH.Data;

namespace DYH.Models
{
    [TableName("modules")]
    [PrimaryKey("moduleid")]
    public class ModuleEntry
    {
        [Column("moduleid")]
        public int ModuleId { get; set; }
        [Column("parentid")]
        public int ParentId { get; set; }

        [Required]
        [DisplayName("Module/Menu Code")]
        [Column("modulecode")]
        public string ModuleCode { get; set; }

        [Required]
        [DisplayName("Display Name")]
        [Column("displayname")]
        public string DisplayName { get; set; }

        [DisplayName("Display As Menu")]
        [Column("displayasmenu")]
        public bool? DisplayAsMenu { get; set; }

        [DisplayName("Class Name")]
        [Column("classname")]
        public string ClassName { get; set; }

        [DisplayName("Url")]
        [Column("url")]
        public string Url { get; set; }

        [DisplayName("Sequence")]
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
