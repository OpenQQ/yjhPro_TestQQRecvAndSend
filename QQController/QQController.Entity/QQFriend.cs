using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQController.Entity
{
    public class QQFriend
    {
        [Key]
        public int Id { get; set; }
        public bool IsD { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        [MaxLength(20)]
        public string QQNum { set; get; }

        [MaxLength(100)]
        public string Nick { set; get; }

        [MaxLength(20)]
        public string Owner { set; get; }
    }
}
