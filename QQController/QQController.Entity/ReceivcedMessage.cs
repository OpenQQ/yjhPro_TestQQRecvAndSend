using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQController.Entity
{
    public class ReceivcedMessage
    {
        [Key]
        public int Id { get; set; }
        public bool IsD { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public int Type { set; get; }

        [MaxLength(20)]
        public string From { set; get; }

        [MaxLength(20)]
        public string To { set; get; }

        [MaxLength(20)]
        public string Owner { set; get; }

        [MaxLength(20)]
        public string GroupNum { set; get; }
        
        [MaxLength(5000)]
        public string Message { set; get; }
    }
}
