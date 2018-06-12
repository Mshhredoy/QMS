using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QmsApp.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public int? ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
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