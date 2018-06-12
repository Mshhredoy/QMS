using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QmsApp.Models
{
    [Table("Services")]
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        [DisplayName("Service Name")]
        public string ServiceName { get; set; }

        [DisplayName("Possible Service Time")]
        public TimeSpan? PossibleServiceTime { get; set; }
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

    public class CounterRelation
    {
        [Key]
        public int CounterRelationId { get; set; }
        public int? CounterId { get; set; }
        public int? Status { get; set; }

        [ForeignKey("CounterId")]
        public virtual Counter Counter { get; set; } 
        public int? ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
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