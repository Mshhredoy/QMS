using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QmsApp.Models
{
    [Table("Counters")]
    public class Counter
    {
        [Key]
        public int CounterId { get; set; }

        [DisplayName("Count No")]
        public string CountNo { get; set; }

        public int? UserId { get; set; }
         [ForeignKey("UserId")]
        public virtual User User { get; set; }

         public int? Status { get; set; }

         public int? CreateBy { get; set; }
        [ForeignKey("CreateBy")]
         public virtual User CreateUser { get; set; }

        public DateTime? CreateTime { get; set; } 
        public int? UpdateBy { get; set; }
        [ForeignKey("UpdateBy")]
         public virtual User UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

    }
}